"""Integration tests for lineup_engine.analyze using repo player-pool CSVs."""

from __future__ import annotations

import lineup_engine as le


def setup_function() -> None:
    le._pool = None  # reload from data/player-pool each test


def _demo_roster() -> list[str]:
    """Nine names that exist in data/player-pool/hitters-fielding.csv."""
    return [
        "Cobb, Ty",
        "Ruth, Babe",
        "Gehrig, Lou",
        "Mantle, Mickey",
        "Bonds, Barry",
        "Wagner, Honus",
        "Collins, Eddie",
        "Gibson, Josh",
        "Speaker, Tris",
    ]


def test_rc_for_side_applies_platoon() -> None:
    player = {"rc600": 100, "ops_l": 1.0, "ops_r": 0.8}
    assert le._rc_for_side(player, "vs_lhp") == 111.1
    assert le._rc_for_side(player, "vs_rhp") == 88.9


def test_can_play_dh_and_positions() -> None:
    player = {"name": "Cobb, Ty", "positions": ["CF", "RF"]}
    assert le._can_play(player, "DH")
    assert le._can_play(player, "CF")
    assert not le._can_play(player, "C")


def test_score_at_pos_combines_rc_and_def() -> None:
    player = {
        "name": "Test",
        "rc600": 90,
        "ops_l": 0.8,
        "ops_r": 0.8,
        "fielding": {"SS": {"range": "Fr", "err": 152}},
    }
    score = le._score_at_pos(player, "SS", "vs_lhp")
    assert score < 90  # Fr/152 SS penalty


def test_merged_positions_uses_eligibility_override() -> None:
    player = {"name": "Cobb, Ty", "positions": ["CF"]}
    elig = {"Cobb, Ty": ["RF", "DH"]}
    merged = le._merged_positions(player, elig)
    assert "RF" in merged and "CF" in merged


def test_side_stat_helpers() -> None:
    player = {
        "obp_l": 0.4,
        "obp_r": 0.35,
        "ops_l": 0.9,
        "ops_r": 0.85,
        "hrf_l": 10.0,
        "hrf_r": 12.0,
    }
    assert le._obp_for_side(player, "vs_lhp") == 0.4
    assert le._ops_for_side(player, "vs_rhp") == 0.85
    assert le._hrf_for_side(player, "vs_lhp") == 10.0


def test_analyze_returns_comparison_for_lhp() -> None:
    roster = _demo_roster()
    positions = ["DH", "LF", "1B", "CF", "RF", "SS", "2B", "3B", "C"]
    current = [
        {"order": i + 1, "position": positions[i], "playerName": roster[i]}
        for i in range(9)
    ]
    result = le.analyze(
        pitcher_side="lhp",
        current_lineup=current,
        roster_names=roster,
        lineup_name="Primary vs. LHP",
    )
    assert result["lineupName"] == "Primary vs. LHP"
    assert result["pitcherSide"] == "vs LHP"
    assert len(result["currentLineup"]) == 9
    assert len(result["recommendedLineup"]) >= 8
    assert "delta" in result
    assert isinstance(result["swaps"], list)
    assert result["engine"] in ("dmb-config", "rc-def-fallback")
