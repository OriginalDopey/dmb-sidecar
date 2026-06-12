"""Lineup recommendations via DiamondMind scripts/dmb_config (implementation-plan source of truth).

Uses select_starters + order_lineup from scripts/dmb_config/lineup.py — same path as
generate_team_config.py / IMPLEMENTATION_PLAN.md. Do not re-derive slot rules here.
"""

from __future__ import annotations

import os
import sys
from pathlib import Path

_CONFIG_ROOT: Path | None = None


def _resolve_config_scripts() -> Path | None:
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


def _import_config():
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


def config_available() -> bool:
    return _resolve_config_scripts() is not None


def _pool_player_to_batter(player: dict, eligibility: dict[str, list[str]] | None) -> Batter:
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


def _hand_char(side: str) -> str:
    return "L" if side in ("vs_lhp", "lhp", "L") else "R"


def order_assigned_lineup(
    assigned: list[dict],
    side: str,
    eligibility: dict[str, list[str]] | None = None,
) -> list[dict]:
    """Apply implementation-plan batting order to an existing position assignment."""
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


def recommend_lineup(
    roster_names: list[str],
    side: str,
    eligibility: dict[str, list[str]] | None = None,
    *,
    dh: bool = True,
) -> tuple[list[dict], list[str]]:
    """
    Build lineup using implementation-plan rules.
    Returns (slot dicts compatible with lineup_engine.analyze, generator warnings).
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
