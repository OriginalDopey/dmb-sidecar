"""
Bridge to DiamondMind ``dmb_config`` lineup rules.

Delegates starter selection and batting-order logic to the same
``scripts/dmb_config/lineup.py`` module used by ``generate_team_config.py`` and
``IMPLEMENTATION_PLAN.md``. This sidecar does not re-derive slot rules locally;
when config scripts are present, batting order matches the implementation-plan
source of truth.

Resolution order for config scripts:

    1. ``DMB_CONFIG_SCRIPTS`` environment variable
    2. ``../../DiamondMind/scripts`` (sibling workspace layout)
    3. ``vendor/dmb_config_scripts`` (synced copy via ``sync-dmb-config.sh``)

Public API:

    config_available()
        Whether ``dmb_config.lineup`` can be imported.
    order_assigned_lineup(...)
        Apply ``order_lineup`` to an existing position assignment.
    recommend_lineup(...)
        Full ``select_starters`` + ``order_lineup`` pipeline.
"""

from __future__ import annotations

import os
import sys
from pathlib import Path

# ---------------------------------------------------------------------------
# Config script path resolution
# ---------------------------------------------------------------------------

_CONFIG_ROOT: Path | None = None
"""Cached root directory containing the ``dmb_config`` package."""


def _resolve_config_scripts() -> Path | None:
    """
    Locate the directory that contains ``dmb_config/lineup.py``.

    Checks ``DMB_CONFIG_SCRIPTS``, sibling DiamondMind layout, and vendored
    copies. Result is cached in ``_CONFIG_ROOT``.
    """
    global _CONFIG_ROOT
    if _CONFIG_ROOT is not None:
        return _CONFIG_ROOT

    candidates: list[Path] = []
    if env := os.environ.get("DMB_CONFIG_SCRIPTS"):
        candidates.append(Path(env).expanduser())
    here = Path(__file__).resolve()
    candidates.extend(
        [
            here.parents[3] / "DiamondMind" / "scripts",
            here.parents[2] / "vendor" / "dmb_config_scripts",
            here.parent / "vendor" / "dmb_config_scripts",
        ]
    )
    for root in candidates:
        if (root / "dmb_config" / "lineup.py").is_file():
            _CONFIG_ROOT = root
            return root
    return None


# ---------------------------------------------------------------------------
# Dynamic import of dmb_config
# ---------------------------------------------------------------------------


def _import_config():
    """
    Import ``select_starters``, ``order_lineup``, and model types from dmb_config.

    Raises:
        ImportError: When no config scripts root is found on disk.

    Returns:
        Tuple of (select_starters, order_lineup, Batter, FieldRating,
        LeagueRules, Team).
    """
    root = _resolve_config_scripts()
    if not root:
        raise ImportError(
            "dmb_config not found — set DMB_CONFIG_SCRIPTS to DiamondMind/scripts "
            "or run scripts/sync-dmb-config.sh"
        )
    path = str(root)
    if path not in sys.path:
        sys.path.insert(0, path)
    from dmb_config.lineup import order_lineup, select_starters
    from dmb_config.model import Batter, FieldRating, LeagueRules, Team

    return select_starters, order_lineup, Batter, FieldRating, LeagueRules, Team


# ---------------------------------------------------------------------------
# Availability probe
# ---------------------------------------------------------------------------


def config_available() -> bool:
    """Return True when ``dmb_config.lineup`` can be resolved and imported."""
    return _resolve_config_scripts() is not None


# ---------------------------------------------------------------------------
# Pool player → dmb_config model conversion
# ---------------------------------------------------------------------------


def _pool_player_to_batter(player: dict, eligibility: dict[str, list[str]] | None) -> Batter:
    """
    Convert a lineup_engine pool dict into a ``dmb_config.model.Batter``.

    Merges position eligibility overrides and maps fielding grades to
    ``FieldRating`` instances expected by ``select_starters`` / ``order_lineup``.
    """
    from lineup_engine import _merged_positions

    _, _, Batter, FieldRating, _, _ = _import_config()
    positions = _merged_positions(player, eligibility)
    fielding = {
        pos: FieldRating(pos, fd.get("range"), fd.get("err"))
        for pos, fd in player.get("fielding", {}).items()
    }
    ops_l, ops_r = player.get("ops_l") or 0, player.get("ops_r") or 0
    obp_l, obp_r = player.get("obp_l") or 0, player.get("obp_r") or 0
    mid_ops = (ops_l + ops_r) / 2 if (ops_l + ops_r) else 0
    mid_obp = (obp_l + obp_r) / 2 if (obp_l + obp_r) else 0
    slg_l = player.get("slg_l") or max(0, ops_l - obp_l) if obp_l else 0
    slg_r = player.get("slg_r") or max(0, ops_r - obp_r) if obp_r else 0
    mid_slg = (slg_l + slg_r) / 2 if (slg_l + slg_r) else 0

    return Batter(
        name=player["name"],
        salary=player.get("salary", 0),
        positions=positions,
        active=True,
        rc600=player.get("rc600"),
        obp=mid_obp or None,
        slg=mid_slg or None,
        ops_vs_l=ops_l or None,
        ops_vs_r=ops_r or None,
        obp_vs_l=obp_l or None,
        obp_vs_r=obp_r or None,
        bat_plat=player.get("bat_plat"),
        run=player.get("run"),
        fielding=fielding,
    )


# ---------------------------------------------------------------------------
# Handedness mapping
# ---------------------------------------------------------------------------


def _hand_char(side: str) -> str:
    """
    Map lineup_engine side string to dmb_config pitcher hand character.

    Returns ``L`` for left-handed pitcher exposure, else ``R``.
    """
    return "L" if side in ("vs_lhp", "lhp", "L") else "R"


# ---------------------------------------------------------------------------
# Batting order on fixed assignment
# ---------------------------------------------------------------------------


def order_assigned_lineup(
    assigned: list[dict],
    side: str,
    eligibility: dict[str, list[str]] | None = None,
) -> list[dict]:
    """
    Apply implementation-plan batting order to an existing position assignment.

    Position assignment (RC+def) is performed by ``lineup_engine``; this function
    only runs ``order_lineup`` and re-attaches side-specific stats for the API.

    Args:
        assigned: Nine slots with ``position`` and ``player`` already set.
        side: ``vs_lhp`` or ``vs_rhp`` batter perspective.
        eligibility: Optional extra positions per player.

    Returns:
        Ordered slot dicts compatible with ``lineup_engine.analyze`` output.
    """
    from lineup_engine import _def_runs, _load_pool, _rc_for_side, _slot_stats

    _, order_lineup, _, _, LeagueRules, Team = _import_config()
    pool = _load_pool()
    hand = _hand_char(side)

    batters = []
    for s in assigned:
        p = pool.get(s["player"], {})
        if p:
            batters.append(_pool_player_to_batter(p, eligibility))

    starters: dict = {}
    for s in assigned:
        name = s["player"]
        batter = next((b for b in batters if b.name == name), None)
        if batter:
            starters[s["position"]] = batter

    team = Team(batters=batters, rules=LeagueRules(dh=True))
    ordered = order_lineup(starters, hand, team)

    out: list[dict] = []
    for order, pos, batter in ordered:
        p = pool.get(batter.name, {})
        fld = p.get("fielding", {}).get(pos, {}) if pos != "DH" else {}
        rc = _rc_for_side(p, side)
        d = _def_runs(fld.get("range"), fld.get("err"), pos) if pos != "DH" else 0
        out.append(
            {
                "order": order,
                "position": pos,
                "player": batter.name,
                "rc600": rc,
                "def": d,
                "salary": p.get("salary", 0),
                "total": round(rc + d, 1),
                **_slot_stats(p, pos, side),
            }
        )
    return out


# ---------------------------------------------------------------------------
# Full recommendation pipeline
# ---------------------------------------------------------------------------


def recommend_lineup(
    roster_names: list[str],
    side: str,
    eligibility: dict[str, list[str]] | None = None,
    *,
    dh: bool = True,
) -> tuple[list[dict], list[str]]:
    """
    Build a lineup using implementation-plan starter and order rules.

    Args:
        roster_names: Hitter names to consider (must exist in player pool).
        side: ``vs_lhp`` or ``vs_rhp`` batter perspective.
        eligibility: Optional extra positions per player.
        dh: Whether DH league rules apply (Classic Standard default True).

    Returns:
        Tuple of (slot dicts compatible with ``lineup_engine.analyze``,
        generator warnings from ``Team.warnings``).
    """
    from lineup_engine import _def_runs, _load_pool, _norm_name, _rc_for_side, _slot_stats

    select_starters, order_lineup, _, _, LeagueRules, Team = _import_config()
    pool = _load_pool()
    hand = _hand_char(side)

    batters = []
    for raw in roster_names:
        key = _norm_name(raw)
        p = pool.get(key)
        if not p:
            continue
        batters.append(_pool_player_to_batter(p, eligibility))

    if len(batters) < 9:
        return [], [f"Only {len(batters)} roster hitters in player pool (need 9)."]

    team = Team(batters=batters, rules=LeagueRules(dh=dh))
    starters = select_starters(team, hand)
    ordered = order_lineup(starters, hand, team)

    slots: list[dict] = []
    for order, pos, batter in ordered:
        p = pool.get(batter.name, {})
        fld = p.get("fielding", {}).get(pos, {}) if pos != "DH" else {}
        rc = _rc_for_side(p, side)
        d = _def_runs(fld.get("range"), fld.get("err"), pos) if pos != "DH" else 0
        slots.append(
            {
                "order": order,
                "position": pos,
                "player": batter.name,
                "rc600": rc,
                "def": d,
                "salary": p.get("salary", 0),
                "total": round(rc + d, 1),
                **_slot_stats(p, pos, side),
            }
        )

    return slots, list(team.warnings)
