"""
HTTP bridge over the dmb-mcp-server repository layer.

This module exposes a FastAPI application that lets the C# DmbSidecar.Api host
query cached ImagineSports league data without spawning a stdio MCP process.
It imports ``dmb_mcp`` directly and mirrors the data-access surface used by
dmb-mcp-server tools (standings, roster, financials, injuries, reports).

Environment variables (same contract as dmb-mcp-server):

    DMB_DB_PATH
        Path to the SQLite cache populated by MCP scrapes.
    DMB_SESSION_PATH
        Browser session cookie store for authenticated scrapes.
    DMB_CONFIG_PATH
        Optional MCP configuration file path.
    DMB_ENTRY_TEAM_ID
        Default entry-team scope when callers pass ``"mine"``.
    DMB_MCP_SRC
        Optional path to ``dmb-mcp-server/src``; inserted onto ``sys.path`` at
        startup when set (typically via ``start-dev.sh`` or ``.env.local``).
    PYTHONPATH
        Must include ``dmb-mcp-server/src`` for league-data routes when
        ``DMB_MCP_SRC`` is not set.

Entry point::

    uvicorn app:app --host 127.0.0.1 --port 8765
"""

from __future__ import annotations

import json
import os
import sys
from contextlib import asynccontextmanager
from typing import Any

from fastapi import FastAPI, HTTPException, Query
from fastapi.middleware.cors import CORSMiddleware
from pydantic import BaseModel

# ---------------------------------------------------------------------------
# Imports & runtime path configuration
# ---------------------------------------------------------------------------

# dmb-mcp-server/src on PYTHONPATH for league-data routes (set via start-dev.sh / .env.local)
_MCP_SRC = os.environ.get("DMB_MCP_SRC", "").strip()
if _MCP_SRC and _MCP_SRC not in sys.path:
    sys.path.insert(0, _MCP_SRC)

# Lazily initialized singleton; closed on application shutdown.
_ctx = None


def _get_ctx():
    """
    Return the shared ``AppContext`` instance, creating it on first use.

    ``AppContext`` wires settings, database access, session auth, and the
    scraper. The instance is cached for the lifetime of the process unless
    ``lifespan`` tears it down.
    """
    global _ctx
    if _ctx is None:
        from dmb_mcp.context import AppContext

        _ctx = AppContext.create()
    return _ctx


# ---------------------------------------------------------------------------
# Application lifecycle
# ---------------------------------------------------------------------------


@asynccontextmanager
async def lifespan(_app: FastAPI):
    """
    FastAPI lifespan hook that releases MCP resources on shutdown.

    Yields immediately after startup; on shutdown closes the database
    connection held by ``AppContext`` and clears the module-level cache.
    """
    yield
    global _ctx
    if _ctx is not None:
        _ctx.close()
        _ctx = None


app = FastAPI(title="DMB Sidecar MCP Bridge", version="0.1.0", lifespan=lifespan)
app.add_middleware(
    CORSMiddleware,
    allow_origins=["*"],
    allow_methods=["GET", "POST"],
    allow_headers=["*"],
)


# ---------------------------------------------------------------------------
# Request / response models
# ---------------------------------------------------------------------------


class ReportResponse(BaseModel):
    """Plain-text report payload returned by snapshot and summary endpoints."""

    text: str


class LineupAnalyzeRequest(BaseModel):
    """
    Body for ``POST /lineup/analyze``.

    Field names mirror the C# API contract (camelCase) for JSON interop.
    """

    pitcherSide: str = "rhp"
    currentLineup: list[dict[str, Any]] = []
    rosterNames: list[str] = []
    lineupName: str = ""
    positionEligibility: dict[str, list[str]] | None = None


# ---------------------------------------------------------------------------
# Health & configuration endpoints
# ---------------------------------------------------------------------------


@app.get("/health")
def health() -> dict[str, str]:
    """Liveness probe used by the sidecar host and integration tests."""
    return {"status": "ok", "service": "mcp-bridge"}


@app.get("/config/status")
def config_status() -> dict[str, Any]:
    """
    Report database path, scoped entry team, and browser session validity.

    Useful for diagnosing missing scrapes or expired ImagineSports cookies
    before calling data endpoints.
    """
    ctx = _get_ctx()
    status = ctx.session.auth_status()
    return {
        "db_path": str(ctx.settings.db_path),
        "entry_team_id": ctx.settings.entry_team_id,
        "session_valid": bool(status.get("valid")),
        "session_message": status.get("message"),
    }


# ---------------------------------------------------------------------------
# League data endpoints (Repository-backed)
# ---------------------------------------------------------------------------


@app.get("/standings")
def standings(league_id: str = "mine") -> list[dict]:
    """
    Return cached division standings for a league.

    Args:
        league_id: League identifier or ``"mine"`` to resolve from entry team.
    """
    from dmb_mcp.db.repository import Repository

    ctx = _get_ctx()
    repo = Repository(ctx.db)
    lid = ctx.resolve_league_id(league_id)
    return [r.model_dump() for r in repo.standings(lid)]


@app.get("/roster")
def roster(team_id: str = "mine") -> list[dict]:
    """
    Return cached roster rows for a team.

    Args:
        team_id: Owner team id or ``"mine"`` to resolve from entry team.
    """
    from dmb_mcp.db.repository import Repository

    ctx = _get_ctx()
    repo = Repository(ctx.db)
    tid = ctx.resolve_team_id(team_id)
    return [r.model_dump() for r in repo.roster(tid)]


@app.get("/financials")
def financials(team_id: str = "mine") -> dict:
    """
    Return cached bankroll and salary summary for a team.

    Returns an empty dict when no financial row exists in the cache.
    """
    from dmb_mcp.db.repository import Repository

    ctx = _get_ctx()
    repo = Repository(ctx.db)
    tid = ctx.resolve_team_id(team_id)
    fin = repo.financials(tid)
    return fin.model_dump() if fin else {}


@app.get("/injuries")
def injuries(team_id: str = "mine") -> list[dict]:
    """Return cached injury list for a team."""
    from dmb_mcp.db.repository import Repository

    ctx = _get_ctx()
    repo = Repository(ctx.db)
    tid = ctx.resolve_team_id(team_id)
    return repo.injuries(tid)


# ---------------------------------------------------------------------------
# Report helpers & endpoints
# ---------------------------------------------------------------------------


def _owner_team_for_scope(ctx, scope: str | None) -> str | None:
    """
    Map an entry-team scope to the owner ``team_id`` stored in the cache.

    The MCP scrape keys roster/finance rows by owner team id; entry team id
    alone is not always sufficient for repository lookups.
    """
    entry = ctx.resolve_entry_team_id(scope)
    row = ctx.db.execute(
        "SELECT owner_team_id FROM leagues WHERE entry_team_id = ? LIMIT 1",
        [entry],
    ).fetchone()
    if row and row["owner_team_id"]:
        return str(row["owner_team_id"])
    return None


@app.get("/report/team_snapshot")
def team_snapshot(team_id: str = "mine") -> ReportResponse:
    """
    Build a human-readable roster and finance snapshot for the scoped team.

    When no cached data exists, returns guidance to scrape or use on-page
    roster context instead of raising an error.
    """
    from dmb_mcp.db.repository import Repository

    ctx = _get_ctx()
    repo = Repository(ctx.db)
    entry = ctx.resolve_entry_team_id(team_id)
    tid = _owner_team_for_scope(ctx, team_id)
    if not tid:
        return ReportResponse(
            text=(
                f"No cached MCP data for entry team {entry}. "
                "Use roster data from the browser page, or run POST /scrape/refresh "
                f"with entry_team_id={entry}."
            )
        )
    roster_rows = repo.roster(tid)
    fin = repo.financials(tid)
    lines = [f"Team {tid} (entry {entry})", f"Roster ({len(roster_rows)} players):"]
    for p in roster_rows[:28]:
        lines.append(f"  {p.player} ({p.position}) {p.salary}")
    if fin:
        lines.append(
            f"Finance: cash {fin.balance}, roster ${fin.roster_salary_num or 0:,}, park {fin.park}"
        )
    return ReportResponse(text="\n".join(lines))


@app.get("/report/league_summary")
def league_summary(team_id: str = "mine") -> ReportResponse:
    """
    Return the formatted league summary text from the repository cache.

    Detects empty standings (zero teams) and returns a scrape hint instead of
    misleading blank output.
    """
    from dmb_mcp.db.repository import Repository

    ctx = _get_ctx()
    repo = Repository(ctx.db)
    entry = ctx.resolve_entry_team_id(team_id)
    try:
        lid = ctx.resolve_league_id(entry)
    except ValueError as exc:
        return ReportResponse(text=str(exc))
    text = repo.league_summary_text(lid)
    if "Standings (0 teams)" in text or text.strip().endswith("Standings (0 teams):"):
        return ReportResponse(
            text=f"No cached standings for entry {entry}. Scrape this league or use on-page roster data."
        )
    return ReportResponse(text=text)


# ---------------------------------------------------------------------------
# Lineup analysis (delegates to lineup_engine)
# ---------------------------------------------------------------------------


@app.post("/lineup/analyze")
def lineup_analyze(body: LineupAnalyzeRequest) -> dict:
    """
    Compare a user's lineup against an optimal RC+def recommendation.

    Delegates to ``lineup_engine.analyze``; response shape matches the C#
    ``LineupAnalyzeResponse`` contract.
    """
    from lineup_engine import analyze

    return analyze(
        pitcher_side=body.pitcherSide,
        current_lineup=body.currentLineup,
        roster_names=body.rosterNames,
        lineup_name=body.lineupName,
        position_eligibility=body.positionEligibility,
    )


# ---------------------------------------------------------------------------
# Scrape operations
# ---------------------------------------------------------------------------


@app.post("/scrape/refresh")
def scrape_refresh(entry_team_id: str | None = None) -> dict:
    """
    Trigger an MCP refresh scrape for the given entry team.

    Args:
        entry_team_id: Entry team scope; defaults to configured entry team.

    Returns:
        Scraper result dict (status, counts, timing) from ``AppContext.scraper``.
    """
    ctx = _get_ctx()
    target = ctx.resolve_entry_team_id(entry_team_id)

    def progress(_pct: float, _msg: str) -> None:
        pass

    result = ctx.scraper.run(target, mode="refresh", verbose=False, progress=progress)
    return result


# ---------------------------------------------------------------------------
# Entry point (local development)
# ---------------------------------------------------------------------------

if __name__ == "__main__":
    import uvicorn

    uvicorn.run(app, host="127.0.0.1", port=8765)
