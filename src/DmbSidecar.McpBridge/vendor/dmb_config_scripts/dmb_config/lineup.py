"""Lineup construction + platoon selection (KB lineup_construction.md)."""

from __future__ import annotations

from .model import Batter, Team, grade_rank
from .rules_tables import (
    BASE_POSITIONS,
    HEART_RANK_BY,
    LINEUP_SLOTS,
    MIN_OPS_TO_START,
    OBP_TIEBREAK,
    PLATOON_OPS_GAP,
    PLATOON_POSITION_PRIORITY,
)


def _prod_vs(b: Batter, hand: str) -> float:
    ops = b.ops_vs(hand)
    return ops if ops is not None else (b.slg or 0) + (b.obp or 0)


def _heart_vs(b: Batter, hand: str) -> float:
    if HEART_RANK_BY == "rc600" and b.rc600 is not None:
        return b.rc600
    return _prod_vs(b, hand)


def _obp_vs(b: Batter, hand: str) -> float:
    o = b.obp_vs(hand)
    return o if o is not None else (b.obp or 0)


def _eligible_pool(team: Team, hand: str) -> list[Batter]:
    pool: list[Batter] = []
    for b in team.active_batters():
        ops = b.ops_vs(hand)
        if ops is not None and ops < MIN_OPS_TO_START:
            team.add_warning(
                f"{b.name} benched vs {'LHP' if hand == 'L' else 'RHP'}: OPS {ops:.3f}"
            )
            continue
        pool.append(b)
    return pool


def _platoon_priority_positions(team: Team, hand: str) -> set[str]:
    """Positions with a platoon split (weak side benched this hand or other hand)."""
    other = "R" if hand == "L" else "L"
    priority: set[str] = set()
    for pos in BASE_POSITIONS:
        at_pos = [b for b in team.active_batters() if b.can_play(pos)]
        weak_this = [
            b
            for b in at_pos
            if b.ops_vs(hand) is not None and b.ops_vs(hand) < MIN_OPS_TO_START
        ]
        weak_other = [
            b
            for b in at_pos
            if b.ops_vs(other) is not None and b.ops_vs(other) < MIN_OPS_TO_START
        ]
        strong_this = [
            b
            for b in at_pos
            if b.ops_vs(hand) is not None and b.ops_vs(hand) >= MIN_OPS_TO_START
        ]
        if not strong_this:
            continue
        for w in weak_this + weak_other:
            w_hand = w.ops_vs(hand) or 0
            w_other = w.ops_vs(other) or 0
            if abs(w_other - w_hand) >= PLATOON_OPS_GAP:
                priority.add(pos)
                break
    return priority


def _position_fill_order(team: Team, hand: str, pool: list[Batter]) -> list[str]:
    platoon_first = _platoon_priority_positions(team, hand)

    def eligible_count(pos: str) -> int:
        return len([b for b in pool if b.can_play(pos)])

    platoon_sorted = sorted(
        platoon_first,
        key=lambda p: (PLATOON_POSITION_PRIORITY.get(p, 99), eligible_count(p)),
    )
    rest = [p for p in BASE_POSITIONS if p not in platoon_first]
    rest_sorted = sorted(rest, key=eligible_count)
    return platoon_sorted + rest_sorted


def select_starters(team: Team, hand: str) -> dict[str, Batter]:
    """Best eligible active batter per position vs pitching hand."""
    pool = _eligible_pool(team, hand)
    assigned: dict[str, Batter] = {}
    used: set[int] = set()

    def eligible(pos: str) -> list[Batter]:
        return [b for b in pool if b.can_play(pos) and id(b) not in used]

    for pos in _position_fill_order(team, hand, pool):
        cands = eligible(pos)
        if not cands:
            team.add_warning(
                f"No eligible active batter for {pos} vs {'LHP' if hand == 'L' else 'RHP'}"
            )
            continue
        other = "R" if hand == "L" else "L"
        specialists = [
            b
            for b in team.active_batters()
            if b.can_play(pos)
            and b.ops_vs(hand) is not None
            and b.ops_vs(hand) < MIN_OPS_TO_START
        ]
        if pos in _platoon_priority_positions(team, hand) and specialists:
            spec_ids = {id(b) for b in specialists}
            strong_side = [b for b in cands if id(b) in spec_ids]
            if strong_side:
                best = max(strong_side, key=lambda b: _prod_vs(b, hand))
            else:
                partners = [b for b in cands if id(b) not in spec_ids]
                if partners:
                    if pos in ("CF", "LF", "RF"):

                        def _cf_corner(b: Batter) -> tuple:
                            idx = b.positions.index("CF") if "CF" in b.positions else 99
                            return (idx, b.salary, -_prod_vs(b, hand))

                        best = min(partners, key=_cf_corner)
                    else:
                        best = max(partners, key=lambda b: _prod_vs(b, hand))
                else:
                    best = max(cands, key=lambda b: _prod_vs(b, hand))
        else:
            best = max(cands, key=lambda b: _prod_vs(b, hand))
        assigned[pos] = best
        used.add(id(best))

    if team.rules.dh:
        rest = [b for b in pool if id(b) not in used]
        if rest:
            # Prefer 1B/DH platoon: if 1B starter is best DH vs this hand, swap to DH slot
            one_b = assigned.get("1B")
            dh = max(rest, key=lambda b: _prod_vs(b, hand))
            if one_b and _prod_vs(one_b, hand) >= _prod_vs(dh, hand):
                alt_1b = [
                    b
                    for b in pool
                    if b.can_play("1B")
                    and id(b) not in used
                    and id(b) != id(one_b)
                ]
                if alt_1b:
                    replacement = max(alt_1b, key=lambda b: _prod_vs(b, hand))
                    assigned["1B"] = replacement
                    used.discard(id(one_b))
                    used.add(id(replacement))
                    dh = one_b
            assigned["DH"] = dh
            used.add(id(dh))
    return assigned


def _pick_obp(cands: list[tuple[str, Batter]], hand: str) -> tuple[str, Batter]:
    """Highest OBP with speed tie-break (Ex > Vg > Av)."""
    if not cands:
        raise ValueError("empty candidate list")
    best_obp = max(_obp_vs(b, hand) for _, b in cands)

    tier = [(p, b) for p, b in cands if _obp_vs(b, hand) >= best_obp - OBP_TIEBREAK]
    return max(tier, key=lambda kv: (grade_rank(kv[1].run), _obp_vs(kv[1], hand)))


def order_lineup(
    starters: dict[str, Batter], hand: str, team: Team
) -> list[tuple[int, str, Batter]]:
    """Apply LINEUP_SLOTS: heart #3-5 by RC600, table-setters #1-2 by OBP."""
    if len(starters) < 9:
        team.add_warning(
            f"Lineup vs {'LHP' if hand == 'L' else 'RHP'} incomplete "
            f"({len(starters)}/9 positions); check roster scrape + CSV join"
        )
        return [
            (i + 1, pos, batter)
            for i, (pos, batter) in enumerate(
                sorted(starters.items(), key=lambda kv: _heart_vs(kv[1], hand), reverse=True)
            )
        ]
    entries = list(starters.items())
    by_heart = sorted(entries, key=lambda kv: _heart_vs(kv[1], hand), reverse=True)
    heart = by_heart[:3]
    heart_ids = {id(b) for _, b in heart}

    remaining = [(p, b) for p, b in entries if id(b) not in heart_ids]
    by_prod = sorted(remaining, key=lambda kv: _prod_vs(kv[1], hand), reverse=True)

    slot_assignments: dict[int, tuple[str, Batter]] = {}
    used_ids: set[int] = set()

    def take_best_obp(from_list: list[tuple[str, Batter]]) -> tuple[str, Batter]:
        avail = [(p, b) for p, b in from_list if id(b) not in used_ids]
        if not avail:
            avail = [(p, b) for p, b in entries if id(b) not in used_ids]
        chosen = _pick_obp(avail, hand)
        used_ids.add(id(chosen[1]))
        return chosen

    def take_best_prod(from_list: list[tuple[str, Batter]]) -> tuple[str, Batter]:
        avail = sorted(
            [(p, b) for p, b in from_list if id(b) not in used_ids],
            key=lambda kv: _prod_vs(kv[1], hand),
            reverse=True,
        )
        if not avail:
            avail = sorted(
                [(p, b) for p, b in entries if id(b) not in used_ids],
                key=lambda kv: _prod_vs(kv[1], hand),
                reverse=True,
            )
        chosen = avail[0]
        used_ids.add(id(chosen[1]))
        return chosen

    heart_sorted = sorted(heart, key=lambda kv: _heart_vs(kv[1], hand), reverse=True)
    cleanup = heart_sorted[0]
    h_rest = [h for h in heart if id(h[1]) != id(cleanup[1])]
    h_by_obp = sorted(h_rest, key=lambda kv: _obp_vs(kv[1], hand), reverse=True)
    three = h_by_obp[0] if h_by_obp else heart_sorted[1]
    five = next(
        (h for h in heart if id(h[1]) not in {id(cleanup[1]), id(three[1])}),
        heart_sorted[-1],
    )

    slot_assignments[3] = three
    slot_assignments[4] = cleanup
    slot_assignments[5] = five
    used_ids.update({id(three[1]), id(cleanup[1]), id(five[1])})

    slot_assignments[1] = take_best_obp(remaining)
    slot_assignments[2] = take_best_obp(remaining)

    for slot in (6, 7, 8):
        slot_assignments[slot] = take_best_prod(remaining)

    slot_assignments[9] = take_best_obp(remaining)

    out: list[tuple[int, str, Batter]] = []
    for slot in range(1, 10):
        pos, batter = slot_assignments[slot]
        out.append((slot, pos, batter))
    return out


def platoon_map(team: Team) -> dict[str, dict]:
    starters_l = select_starters(team, "L")
    starters_r = select_starters(team, "R")
    positions = BASE_POSITIONS + (["DH"] if team.rules.dh else [])
    out: dict[str, dict] = {}
    for pos in positions:
        bl = starters_l.get(pos)
        br = starters_r.get(pos)
        out[pos] = {
            "vs_l": bl,
            "vs_r": br,
            "platoon": bool(bl and br and bl.name != br.name),
        }
    return out


def build_lineups(team: Team) -> dict[str, list[tuple[int, str, Batter]]]:
    return {
        "L": order_lineup(select_starters(team, "L"), "L", team),
        "R": order_lineup(select_starters(team, "R"), "R", team),
    }


def starter_names_for_hand(team: Team, hand: str) -> set[str]:
    return {b.name for b in select_starters(team, hand).values()}
