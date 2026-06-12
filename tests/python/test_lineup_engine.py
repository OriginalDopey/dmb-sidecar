"""Unit tests for lineup_engine pure functions."""

from __future__ import annotations

from lineup_engine import _def_runs, _norm_name, _parse_fld, _parse_money


def test_norm_name_strips_bats_and_inj() -> None:
    assert _norm_name("Cobb, Ty L") == "Cobb, Ty"
    assert _norm_name("Player X INJ") == "Player X"


def test_parse_money() -> None:
    assert _parse_money("$8,500,000") == 8_500_000
    assert _parse_money("") == 0


def test_parse_fld() -> None:
    assert _parse_fld("Vg / 139") == ("Vg", 139)
    assert _parse_fld("") == (None, None)


def test_def_runs_ss_penalizes_high_errors() -> None:
    ss_bad = _def_runs("Fr", 152, "SS")
    ss_good = _def_runs("Fr", 128, "SS")
    at_2b = _def_runs("Vg", 139, "2B")
    assert ss_bad < ss_good
    assert at_2b > ss_bad


def test_def_runs_catcher_range_discounted() -> None:
    c_range = _def_runs("Vg", 100, "C")
    ss_range = _def_runs("Vg", 100, "SS")
    assert c_range < ss_range
