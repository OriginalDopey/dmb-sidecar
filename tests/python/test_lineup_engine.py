"""
Unit tests for ``lineup_engine`` parsing and defensive value helpers.

Covers pure functions that do not require the full player-pool CSV load:
name normalization, salary/fielding parsing, and position-specific defensive
run estimates (including catcher range discount and SS error penalty).
"""

from __future__ import annotations

from lineup_engine import _def_runs, _norm_name, _parse_fld, _parse_money


def test_norm_name_strips_bats_and_inj() -> None:
    """``_norm_name`` removes trailing bats-hand suffixes and injury markers."""
    assert _norm_name("Cobb, Ty L") == "Cobb, Ty"
    assert _norm_name("Player X INJ") == "Player X"


def test_parse_money() -> None:
    """``_parse_money`` strips currency formatting and returns integer dollars."""
    assert _parse_money("$8,500,000") == 8_500_000
    assert _parse_money("") == 0


def test_parse_fld() -> None:
    """``_parse_fld`` parses range/error cells and returns nulls for blanks."""
    assert _parse_fld("Vg / 139") == ("Vg", 139)
    assert _parse_fld("") == (None, None)


def test_def_runs_ss_penalizes_high_errors() -> None:
    """Higher error ratings at SS reduce defensive run value vs lower errors."""
    ss_bad = _def_runs("Fr", 152, "SS")
    ss_good = _def_runs("Fr", 128, "SS")
    at_2b = _def_runs("Vg", 139, "2B")
    assert ss_bad < ss_good
    assert at_2b > ss_bad


def test_def_runs_catcher_range_discounted() -> None:
    """Catcher range contributes less than the same range grade at SS."""
    c_range = _def_runs("Vg", 100, "C")
    ss_range = _def_runs("Vg", 100, "SS")
    assert c_range < ss_range
