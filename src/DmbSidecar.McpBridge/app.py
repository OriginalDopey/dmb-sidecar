"""
HTTP bridge over dmb-mcp-server repository layer.
Avoids stdio MCP from C# — imports dmb_mcp directly.

Env (same as dmb-mcp-server):
  DMB_DB_PATH, DMB_SESSION_PATH, DMB_CONFIG_PATH, DMB_ENTRY_TEAM_ID
  PYTHONPATH must include dmb-mcp-server/src
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

# Allow local dev without installing package
_MCP_SRC = os.environ.get(
    "DMB_MCP_SRC",
    "/Users/originaldopey/Documents/CursonProjects/dmb-mcp-server/src",
)
if _MCP_SRC not in sys.path:
    sys.path.insert(0, _MCP_SRC)

_ctx = None


def _get_ctx():
    global _ctx
    if _ctx is None:
        from dmb_mcp.context import AppContext

        _ctx = AppContext.create()
    return _ctx


@asynccontextmanager
async def lifespan(_app: FastAPI):
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


class ReportResponse(BaseModel):
    text: str


@app.get("/health")
def health() -> dict[str, str]:
    return {"status": "ok", "service": "mcp-bridge"}


@app.get("/config/status")
def config_status() -> dict[str, Any]:
    ctx = _get_ctx()
    status = ctx.session.auth_status()
    return {
        "db_path": str(ctx.settings.db_path),
        "entry_team_id": ctx.settings.entry_team_id,
        "session_valid": bool(status.get("valid")),
        "session_message": status.get("message"),
    }


@app.get("/standings")
def standings(league_id: str = "mine") -> list[dict]:
    from dmb_mcp.db.repository import Repository

    ctx = _get_ctx()
    repo = Repository(ctx.db)
    lid = ctx.resolve_league_id(league_id)
    return [r.model_dump() for r in repo.standings(lid)]


@app.get("/roster")
def roster(team_id: str = "mine") -> list[dict]:
    from dmb_mcp.db.repository import Repository

    ctx = _get_ctx()
    repo = Repository(ctx.db)
    tid = ctx.resolve_team_id(team_id)
    return [r.model_dump() for r in repo.roster(tid)]


@app.get("/financials")
def financials(team_id: str = "mine") -> dict:
    from dmb_mcp.db.repository import Repository

    ctx = _get_ctx()
    repo = Repository(ctx.db)
    tid = ctx.resolve_team_id(team_id)
    fin = repo.financials(tid)
    return fin.model_dump() if fin else {}


@app.get("/injuries")
def injuries(team_id: str = "mine") -> list[dict]:
    from dmb_mcp.db.repository import Repository

    ctx = _get_ctx()
    repo = Repository(ctx.db)
    tid = ctx.resolve_team_id(team_id)
    return repo.injuries(tid)


@app.get("/report/team_snapshot")
def team_snapshot(team_id: str = "mine") -> ReportResponse:
    from dmb_mcp.db.repository import Repository

    ctx = _get_ctx()
    repo = Repository(ctx.db)
    tid = ctx.resolve_team_id(team_id)
    roster_rows = repo.roster(tid)
    fin = repo.financials(tid)
    lines = [f"Team {tid}", f"Roster ({len(roster_rows)} players):"]
    for p in roster_rows[:28]:
        lines.append(f"  {p.player} ({p.position}) {p.salary}")
    if fin:
        lines.append(
            f"Finance: cash {fin.balance}, roster ${fin.roster_salary_num or 0:,}, park {fin.park}"
        )
    return ReportResponse(text="\n".join(lines))


@app.get("/report/league_summary")
def league_summary(league_id: str = "mine") -> ReportResponse:
    from dmb_mcp.db.repository import Repository

    ctx = _get_ctx()
    repo = Repository(ctx.db)
    lid = ctx.resolve_league_id(league_id)
    return ReportResponse(text=repo.league_summary_text(lid))


@app.post("/scrape/refresh")
def scrape_refresh(entry_team_id: str | None = None) -> dict:
    ctx = _get_ctx()
    target = ctx.resolve_entry_team_id(entry_team_id)

    def progress(_pct: float, _msg: str) -> None:
        pass

    result = ctx.scraper.run(target, mode="refresh", verbose=False, progress=progress)
    return result


if __name__ == "__main__":
    import uvicorn

    uvicorn.run(app, host="127.0.0.1", port=8765)
