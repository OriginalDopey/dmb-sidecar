"""Lineup analysis vs LHP/RHP using IS SearchResults CSV player pool."""

from __future__ import annotations

import csv
import re
from itertools import combinations
from pathlib import Path

RANGE_RUNS = {"Ex": 10, "Vg": 7, "Av": 4, "Fr": 1, "Pr": -2}
LINEUP_POSITIONS = ("C", "1B", "2B", "3B", "SS", "LF", "CF", "RF", "DH")
RHP_PCT, LHP_PCT = 0.70, 0.30

_pool: dict[str, dict] | None = None


def _repo_root() -> Path:
    return Path(__file__).resolve().parents[2]


def _norm_name(raw: str) -> str:
    s = re.sub(r"\s+", " ", (raw or "").strip())
    s = re.sub(r"\s+[LRS]\s*$", "", s, flags=re.I)
    s = re.sub(r"\s+INJ.*$", "", s, flags=re.I)
    return s.strip()


def _parse_money(raw: str) -> int:
    digits = re.sub(r"[^\d]", "", raw or "")
    return int(digits) if digits else 0


def _parse_fld(cell: str) -> tuple[str | None, int | None]:
    if not cell or cell in ("&nbsp;", ""):
        return None, None
    m = re.match(r"([A-Za-z]{2})\s*/\s*(\d+)", cell.strip())
    if not m:
        return None, None
    return m.group(1), int(m.group(2))


def _def_runs(range_grade: str | None, err: int | None, pos: str) -> float:
    if not range_grade:
        return 0.0
    base = RANGE_RUNS.get(range_grade, 0)
    if pos == "C":
        return round(base * 0.2, 1)  # catcher range nearly worthless
    if err is not None:
        # ~50 error points ≈ 1 range band at 1B/OF; IF errors cost more (SS avg err 29)
        err_scale = {"SS": 35, "2B": 40, "3B": 42}.get(pos, 50)
        base += (100 - err) / err_scale
    return round(base, 1)


def _load_pool() -> dict[str, dict]:
    global _pool
    if _pool is not None:
        return _pool

    root = _repo_root() / "data" / "player-pool"
    fielding_path = root / "hitters-fielding.csv"
    splits_path = root / "hitters-splits.csv"

    players: dict[str, dict] = {}

    if fielding_path.exists():
        with fielding_path.open(newline="", encoding="utf-8-sig") as f:
            for row in csv.DictReader(f):
                name = _norm_name(row.get("Player", ""))
                if not name:
                    continue
                players[name] = {
                    "name": name,
                    "positions": [p.strip() for p in row.get("Pos", "").split(",") if p.strip()],
                    "salary": _parse_money(row.get("Salary", "")),
                    "rc600": float(row.get("RC600") or 0),
                    "ops_l": float(row.get("OPS vs L") or 0),
                    "ops_r": float(row.get("OPS vs R") or 0),
                    "bat_plat": int(row.get("BatPlat") or 0),
                    "run": row.get("Run", "").strip(),
                    "injury": row.get("Injury", "").strip(),
                    "obp_l": float(row.get("OBP vs L") or 0),
                    "obp_r": float(row.get("OBP vs R") or 0),
                    "hrf": float(row.get("HRF") or 0),
                    "fielding": {},
                }
                for pos in ("C", "1B", "2B", "3B", "SS", "LF", "CF", "RF"):
                    rg, er = _parse_fld(row.get(f"FLD:{pos}", ""))
                    if rg:
                        players[name]["fielding"][pos] = {"range": rg, "err": er}

    if splits_path.exists():
        with splits_path.open(newline="", encoding="utf-8-sig") as f:
            for row in csv.DictReader(f):
                name = _norm_name(row.get("Player", ""))
                if name not in players:
                    continue
                p = players[name]
                p["obp_l"] = float(row.get("OBP vs L") or p.get("obp_l") or 0)
                p["obp_r"] = float(row.get("OBP vs R") or p.get("obp_r") or 0)
                p["slg_l"] = float(row.get("SLG vs L") or 0)
                p["slg_r"] = float(row.get("SLG vs R") or 0)
                p["hrf_l"] = float(row.get("HRF vs L") or 0)
                p["hrf_r"] = float(row.get("HRF vs R") or 0)
                if row.get("Injury"):
                    p["injury"] = row.get("Injury", "").strip()
                if row.get("Run"):
                    p["run"] = row.get("Run", "").strip()

    _pool = players
    return players


def _rc_for_side(player: dict, side: str) -> float:
    """side: 'vs_rhp' or 'vs_lhp' (batter perspective)."""
    rc = player.get("rc600") or 0
    ops_l, ops_r = player.get("ops_l") or 0, player.get("ops_r") or 0
    mid = (ops_l + ops_r) / 2 if (ops_l + ops_r) else 0
    if mid <= 0:
        return rc
    if side == "vs_rhp":
        return round(rc * (ops_r / mid), 1)
    return round(rc * (ops_l / mid), 1)


def _obp_for_side(player: dict, side: str) -> float:
    if side == "vs_rhp":
        return player.get("obp_r") or 0
    return player.get("obp_l") or 0


def _ops_for_side(player: dict, side: str) -> float:
    if side == "vs_rhp":
        return player.get("ops_r") or 0
    return player.get("ops_l") or 0


def _hrf_for_side(player: dict, side: str) -> float:
    if side == "vs_rhp":
        return player.get("hrf_r") or player.get("hrf") or 0
    return player.get("hrf_l") or player.get("hrf") or 0


def _slot_stats(player: dict | None, pos: str, side: str) -> dict:
    if not player:
        return {
            "ops": 0.0,
            "obp": 0.0,
            "hrf": 0.0,
            "batPlat": 0,
            "run": "",
            "injury": "",
            "rangeGrade": "",
            "err": None,
        }
    fld = player.get("fielding", {}).get(pos, {}) if pos != "DH" else {}
    return {
        "ops": round(_ops_for_side(player, side), 3),
        "obp": round(_obp_for_side(player, side), 3),
        "hrf": round(_hrf_for_side(player, side), 1),
        "batPlat": int(player.get("bat_plat") or 0),
        "run": player.get("run") or "",
        "injury": player.get("injury") or "",
        "rangeGrade": fld.get("range") or "",
        "err": fld.get("err"),
    }


def _merged_positions(player: dict, eligibility: dict[str, list[str]] | None) -> list[str]:
    positions = list(player.get("positions", []))
    if not eligibility:
        return positions
    name = player.get("name", "")
    for key, extra in eligibility.items():
        if _norm_name(key) == _norm_name(name):
            return list({*positions, *extra})
    return positions


def _can_play(player: dict, pos: str, eligibility: dict[str, list[str]] | None = None) -> bool:
    if pos == "DH":
        return True
    return pos in _merged_positions(player, eligibility)


def _score_at_pos(player: dict, pos: str, side: str) -> float:
    rc = _rc_for_side(player, side)
    fld = player.get("fielding", {}).get(pos, {})
    d = _def_runs(fld.get("range"), fld.get("err"), pos) if pos != "DH" else 0
    return rc + d


def _slot_dict(player: dict, pos: str, side: str) -> dict:
    sc = _score_at_pos(player, pos, side)
    fld = player.get("fielding", {}).get(pos, {}) if pos != "DH" else {}
    return {
        "position": pos,
        "player": player["name"],
        "rc600": _rc_for_side(player, side),
        "def": _def_runs(fld.get("range"), fld.get("err"), pos) if pos != "DH" else 0,
        "salary": player.get("salary", 0),
        "total": round(sc, 1),
        **_slot_stats(player, pos, side),
    }


def _best_assignment(
    players: list[dict], side: str, eligibility: dict[str, list[str]] | None = None
) -> list[dict]:
    """Exhaustive assignment for up to 9 players × 9 positions."""
    positions = list(LINEUP_POSITIONS)
    best_slots: list[dict] = []
    best_score = -1.0

    def dfs(idx: int, used: set[str], acc: list[dict]) -> None:
        nonlocal best_slots, best_score
        if idx == len(positions):
            total = sum(s["total"] for s in acc)
            if total > best_score:
                best_score = total
                best_slots = [dict(s) for s in acc]
            return
        pos = positions[idx]
        for p in players:
            if p["name"] in used or not _can_play(p, pos, eligibility):
                continue
            acc.append(_slot_dict(p, pos, side))
            used.add(p["name"])
            dfs(idx + 1, used, acc)
            acc.pop()
            used.remove(p["name"])

    dfs(0, set(), [])
    return best_slots


def _assign_lineup(
    roster_names: list[str], side: str, eligibility: dict[str, list[str]] | None = None
) -> tuple[list[dict], str]:
    """Returns (slots, engine tag: 'dmb-config' | 'rc-def-fallback')."""
    pool = _load_pool()
    available = []
    for n in roster_names:
        key = _norm_name(n)
        if key in pool:
            available.append(pool[key])

    try:
        from lineup_config import config_available, order_assigned_lineup, recommend_lineup

        if config_available() and len(available) >= 9:
            positioned = _best_positioned_nine(available, side, eligibility)
            if len(positioned) >= 9:
                # RC+def positions + implementation-plan batting order (order_lineup)
                return order_assigned_lineup(positioned, side, eligibility), "dmb-config"
    except Exception:
        pass

    if len(available) < 9:
        return _order_batting(_greedy_fill(available, side, eligibility), side), "rc-def-fallback"

    positioned = _best_positioned_nine(available, side, eligibility)
    return (
        _order_batting(positioned or _greedy_fill(available, side, eligibility), side),
        "rc-def-fallback",
    )


def _best_positioned_nine(
    available: list[dict], side: str, eligibility: dict[str, list[str]] | None = None
) -> list[dict]:
    """Best RC+def assignment for 9 hitters (exactly 9 or best subset from larger pool)."""
    if len(available) == 9:
        return _best_assignment(available, side, eligibility)

    ranked = sorted(
        available,
        key=lambda p: max(
            _score_at_pos(p, pos, side)
            for pos in LINEUP_POSITIONS
            if _can_play(p, pos, eligibility)
        ),
        reverse=True,
    )
    candidates = ranked[: min(13, len(ranked))]
    best_slots: list[dict] = []
    best_score = -1.0
    for combo in combinations(candidates, 9):
        slots = _best_assignment(list(combo), side, eligibility)
        if len(slots) < 9:
            continue
        total = sum(s["total"] for s in slots)
        if total > best_score:
            best_score = total
            best_slots = slots
    return best_slots


def _greedy_fill(
    available: list[dict], side: str, eligibility: dict[str, list[str]] | None = None
) -> list[dict]:
    """Fallback when fewer than 9 pool matches or assignment fails."""
    used: set[str] = set()
    slots: list[dict] = []
    for pos in LINEUP_POSITIONS:
        best = None
        best_score = -1.0
        for p in available:
            if p["name"] in used or not _can_play(p, pos, eligibility):
                continue
            sc = _score_at_pos(p, pos, side)
            if sc > best_score:
                best_score = sc
                best = p
        if best:
            used.add(best["name"])
            slots.append(_slot_dict(best, pos, side))
    return slots


def _order_batting(slots: list[dict], side: str) -> list[dict]:
    pool = _load_pool()
    if not slots:
        return slots

    enriched = []
    for s in slots:
        p = pool.get(s["player"], {})
        enriched.append({**s, "obp": _obp_for_side(p, side), "run": p.get("run", "")})

    by_obp = sorted(enriched, key=lambda x: x["obp"], reverse=True)
    by_rc = sorted(enriched, key=lambda x: x["total"], reverse=True)

    lead = by_obp[0]
    two = by_obp[1] if len(by_obp) > 1 else by_obp[0]
    heart = [x for x in by_rc if x["player"] not in {lead["player"], two["player"]}][:2]
    rest = [x for x in by_rc if x not in [lead, two, *heart]]
    ordered = [lead, two, *heart, *rest]

    out = []
    for i, s in enumerate(ordered[:9], start=1):
        out.append({**s, "order": i})
    return out


def _match_current(current: list[dict], side: str) -> list[dict]:
    pool = _load_pool()
    out = []
    for slot in current:
        name = _norm_name(slot.get("playerName") or slot.get("player") or "")
        pos = (slot.get("position") or "DH").strip()
        p = pool.get(name, {})
        rc = _rc_for_side(p, side) if p else 0
        fld = p.get("fielding", {}).get(pos, {}) if p else {}
        d = _def_runs(fld.get("range"), fld.get("err"), pos) if p and pos != "DH" else 0
        out.append(
            {
                "order": slot.get("order", len(out) + 1),
                "position": pos,
                "player": name or "?",
                "rc600": rc,
                "def": d,
                "total": round(rc + d, 1),
                "salary": p.get("salary", 0) if p else 0,
                "inPool": bool(p),
                **_slot_stats(p if p else None, pos, side),
            }
        )
    return out


def analyze(
    *,
    pitcher_side: str,
    current_lineup: list[dict],
    roster_names: list[str],
    lineup_name: str = "",
    position_eligibility: dict[str, list[str]] | None = None,
) -> dict:
    """
    pitcher_side: 'rhp' or 'lhp' (who the lineup faces).
    Returns comparison + chart data.
    """
    side = "vs_rhp" if pitcher_side.lower() in ("rhp", "vs_rhp", "right") else "vs_lhp"
    label = "vs RHP" if side == "vs_rhp" else "vs LHP"

    current = _match_current(current_lineup, side)
    recommended, engine = _assign_lineup(roster_names, side, position_eligibility)

    cur_total = round(sum(s["total"] for s in current), 1)
    rec_total = round(sum(s["total"] for s in recommended), 1)
    delta = round(rec_total - cur_total, 1)

    swaps = []
    rec_by_pos = {s["position"]: s for s in recommended}
    for c in current:
        r = rec_by_pos.get(c["position"])
        if r and _norm_name(r["player"]) != _norm_name(c["player"]):
            swaps.append(
                {
                    "position": c["position"],
                    "from": c["player"],
                    "to": r["player"],
                    "gain": round(r["total"] - c["total"], 1),
                }
            )

    pool = _load_pool()
    missing = [_norm_name(n) for n in roster_names if _norm_name(n) not in pool]
    not_in_pool = [s["player"] for s in current if not s.get("inPool")]
    seen: set[str] = set()
    dupes: list[str] = []
    for s in current:
        if s["player"] in seen:
            dupes.append(s["player"])
        seen.add(s["player"])

    notes = []
    if engine == "dmb-config":
        notes.append(
            "Positions: best RC+def nine from roster pool. "
            "Batting order: generate_team_config rules (OBP #1–2, RC600 heart #3–5)."
        )
    else:
        notes.append("dmb_config unavailable — using RC+def fallback; run scripts/sync-dmb-config.sh.")
    if dupes:
        notes.append(f"Duplicate in lineup: {', '.join(sorted(set(dupes)))} — totals use each slot as set.")
    if len(available := [n for n in roster_names if _norm_name(n) in pool]) < 9:
        notes.append(f"Only {len(available)} roster hitters matched player pool (need 9 for full DH).")
    if missing:
        notes.append(f"{len(missing)} roster player(s) not in local pool CSV — sync SearchResults exports.")
    if not_in_pool:
        notes.append(f"Current slots missing pool data: {', '.join(not_in_pool[:3])}{'…' if len(not_in_pool) > 3 else ''}.")

    if delta > 3:
        notes.append(f"Recommended lineup is ~{delta:.0f} runs better (RC600+def) than current for {label}.")
    elif delta < -1:
        notes.append(f"Your current lineup is already strong for {label} (+{-delta:.0f} vs model optimal).")
    else:
        notes.append(f"Current lineup is close to optimal for {label} (Δ{delta:+.1f}).")

    for sw in sorted(swaps, key=lambda x: -x["gain"])[:4]:
        if sw["gain"] > 0.5:
            notes.append(f"{sw['position']}: consider {sw['to']} over {sw['from']} (+{sw['gain']:.1f}).")
    for sw in sorted(swaps, key=lambda x: x["gain"])[:2]:
        if sw["gain"] < -0.5:
            notes.append(f"{sw['position']}: keep {sw['from']} over {sw['to']} ({sw['gain']:.1f}).")

    platoon_hints = []
    for s in recommended[:3]:
        p = pool.get(s["player"], {})
        bp = p.get("bat_plat", 0)
        if abs(bp) >= 2:
            platoon_hints.append(f"{s['player']}: BatPlat {bp:+d} — strong {label} side.")

    chart = {
        "labels": [f"{s['order']}. {s['player'].split(',')[0]}" for s in current[:9]],
        "current": [s["total"] for s in current[:9]],
        "recommended": [
            next((r["total"] for r in recommended if r["position"] == s["position"]), 0)
            for s in current[:9]
        ],
    }

    return {
        "lineupName": lineup_name,
        "pitcherSide": label,
        "currentLineup": current,
        "recommendedLineup": recommended,
        "currentTotal": cur_total,
        "recommendedTotal": rec_total,
        "delta": delta,
        "swaps": swaps,
        "notes": notes,
        "platoonHints": platoon_hints[:3],
        "chart": chart,
        "poolSize": len(_load_pool()),
        "engine": engine,
    }
