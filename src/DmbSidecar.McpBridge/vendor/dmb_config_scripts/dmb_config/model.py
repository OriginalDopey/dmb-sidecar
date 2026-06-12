"""Data model for the DMB config generator.

These dataclasses are the reconciled view of a team: roster membership comes from
the live scrape, ratings/splits come from the CSV export. Fields that the raw IS
"SearchResults" export does not contain (e.g. batter BuntSac/BuntHit/Steal) are
left as None and the rule layer degrades to Team Default with a flag.
"""

from __future__ import annotations

from dataclasses import dataclass, field
from typing import Optional


# Rating grade ordering (worst -> best) for comparisons.
GRADE_ORDER = {"Pr": 0, "Fr": 1, "Av": 2, "Vg": 3, "Ex": 4}


def grade_rank(grade: Optional[str]) -> int:
    """Numeric rank for a rating grade; unknown/blank -> -1."""
    if not grade:
        return -1
    return GRADE_ORDER.get(grade.strip(), -1)


@dataclass
class FieldRating:
    """Range grade + error number at one position, e.g. 'Av/82'."""

    pos: str
    grade: Optional[str] = None
    error: Optional[int] = None

    @property
    def known(self) -> bool:
        return self.grade is not None


@dataclass
class Batter:
    name: str
    hand: str = "R"               # 'L', 'R', or 'S'
    salary: int = 0
    positions: list[str] = field(default_factory=list)
    active: bool = True           # False => Inactive Reserve
    # Rate stats (overall)
    rc600: Optional[float] = None
    obp: Optional[float] = None
    slg: Optional[float] = None
    hrf: Optional[float] = None
    bbf: Optional[float] = None
    # Splits
    ops_vs_l: Optional[float] = None
    ops_vs_r: Optional[float] = None
    obp_vs_l: Optional[float] = None
    obp_vs_r: Optional[float] = None
    bat_plat: Optional[int] = None
    # Ratings
    run: Optional[str] = None
    jump: Optional[str] = None
    injury: Optional[str] = None
    of_throw: Optional[str] = None
    fielding: dict[str, FieldRating] = field(default_factory=dict)
    # Not present in raw SearchResults export (scrape-only) -> may be None
    bunt_sac: Optional[str] = None
    bunt_hit: Optional[str] = None
    steal: Optional[str] = None

    def ops_vs(self, hand: str) -> Optional[float]:
        return self.ops_vs_l if hand == "L" else self.ops_vs_r

    def obp_vs(self, hand: str) -> Optional[float]:
        return self.obp_vs_l if hand == "L" else self.obp_vs_r

    def can_play(self, pos: str) -> bool:
        return pos in self.positions

    def field_at(self, pos: str) -> Optional[FieldRating]:
        return self.fielding.get(pos)


@dataclass
class Pitcher:
    name: str
    hand: str = "R"
    salary: int = 0
    role: str = "RP"              # 'SP', 'RP', or 'SP/RP'
    active: bool = True
    era: Optional[float] = None
    erc: Optional[float] = None
    k9: Optional[float] = None
    bb9: Optional[float] = None
    hold: Optional[str] = None
    sp_dur: Optional[str] = None
    rp_dur: Optional[str] = None
    obp_vs_l: Optional[float] = None
    obp_vs_r: Optional[float] = None
    slg_vs_l: Optional[float] = None
    slg_vs_r: Optional[float] = None
    injury: Optional[str] = None

    @property
    def is_sp(self) -> bool:
        return "SP" in self.role.upper()

    @property
    def is_rp(self) -> bool:
        return "RP" in self.role.upper() or not self.is_sp


@dataclass
class LeagueRules:
    league_name: str = ""
    cap: int = 50_000_000
    dh: bool = True
    era_name: str = "Standard"
    park: str = ""
    surface: str = ""
    cover: str = ""
    injuries_on: bool = True
    trades_on: bool = True


@dataclass
class Team:
    name: str = ""
    cur_team: str = ""
    owner: str = ""
    cash: int = 0
    total_salary: int = 0
    rules: LeagueRules = field(default_factory=LeagueRules)
    batters: list[Batter] = field(default_factory=list)
    pitchers: list[Pitcher] = field(default_factory=list)
    warnings: list[str] = field(default_factory=list)

    def add_warning(self, msg: str) -> None:
        if msg not in self.warnings:
            self.warnings.append(msg)

    def active_batters(self) -> list[Batter]:
        return [b for b in self.batters if b.active]

    def active_pitchers(self) -> list[Pitcher]:
        return [p for p in self.pitchers if p.active]

    def ir(self) -> list:
        return [x for x in self.batters + self.pitchers if not x.active]
