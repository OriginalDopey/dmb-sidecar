"""Literal rule tables — single source of truth for config generation.

Do not re-derive these values in skills, prompts, or markdown prose.
Source: New-DMO/knowledge/decision_trees/ and team-instructions advisors.
"""

from __future__ import annotations

# ---------------------------------------------------------------------------
# Lineup ordering (KB lineup_construction.md)
# prod_vs_hand = OPS vs L or OPS vs R — NOT overall RC600
# NEVER sort all 9 batters by OBP only
# ---------------------------------------------------------------------------
LINEUP_SLOTS = {
    1: {"rank_by": "obp_vs_hand", "note": "leadoff — highest OBP + speed tie-break"},
    2: {"rank_by": "obp_vs_hand", "note": "second OBP"},
    3: {"rank_by": "prod_vs_hand", "note": "heart — top producer"},
    4: {"rank_by": "prod_vs_hand", "note": "cleanup — best slugger"},
    5: {"rank_by": "prod_vs_hand", "note": "heart — third producer"},
    6: {"rank_by": "prod_vs_hand", "note": "descending"},
    7: {"rank_by": "prod_vs_hand", "note": "descending"},
    8: {"rank_by": "prod_vs_hand", "note": "descending"},
    9: {"rank_by": "obp_vs_hand", "note": "second leadoff — best remaining OBP"},
}

OBP_TIEBREAK = 0.010          # slots 1-2: prefer faster runner if OBP within this
MIN_OPS_TO_START = 0.500        # e.g. .499 vs RHP => never start that hand
PLATOON_OPS_GAP = 0.060
WEAK_SIDE_OPS_FLOOR = 0.550
PH_OPS_VS_R_MIN = 0.680         # protect strong PH vs RHP from IR (Throneberry rule)
HEART_RANK_BY = "rc600"         # slots 3-5: KB "top RC600 producers" (not overall OBP sort)

# ---------------------------------------------------------------------------
# Team instructions (KB team-instructions advisor)
# ---------------------------------------------------------------------------
TEAM_INSTRUCTIONS_DEFAULT = 3

TEAM_INSTRUCTIONS = {
    "Bunting for Hit": 4,
    "Sacrifice Bunting": 6,
    "Squeeze Bunting": 5,
    "Base Running": 4,
    "Hit and Run": 3,
    "Base Stealing": 4,
    "Taking Pitches": 3,
    "Using Closer": 1,
    "Intentional Walks": 4,
    "Pitch-Around": 4,
    "Pickoff Throws": 5,
    "Pitchouts": 5,
    "Using Relievers": 2,
    "Infield In": 3,
    "Guarding Lines": 3,
    "Hold Runners": 3,
    "Double Switch": 5,
}

TEAM_INSTRUCTIONS_NO_DH = {"Double Switch": 3, "PH for Pitchers": 2}
TEAM_INSTRUCTIONS_FAST_TEAM = {"Base Running": 2}

# ---------------------------------------------------------------------------
# Player instructions
# ---------------------------------------------------------------------------
ALL_STARTERS_PULL_PH = 5
CLOSER_PULL_FOR_CLOSER = 5
SETUP_PULL_FOR_CLOSER = 5
RELIEVER_PULL_FOR_RELIEVER = 1
MULTI_INNING_PULL_FOR_RELIEVER = 4

STARTER_PULL_FOR_RELIEVER = {
    "Ex": 4,
    "Vg": 3,
    "Av": 3,
    "Fr": 1,
    "Pr": 1,
}

# ---------------------------------------------------------------------------
# Bullpen UI sections (IS screen order)
# ---------------------------------------------------------------------------
BULLPEN_SECTIONS = [
    "injury_replacement_starters",
    "mop_up",
    "long_relief",
    "setup_vs_lhb",
    "setup_vs_rhb",
    "closer_vs_lhb",
    "closer_vs_rhb",
]

BULLPEN_SECTION_LABELS = {
    "injury_replacement_starters": "Injury Replacement Starters",
    "mop_up": "Mop-Up Men",
    "long_relief": "Long Relievers",
    "setup_vs_lhb": "Set-Up Men vs. Left-Handed Batters",
    "setup_vs_rhb": "Set-Up Men vs. Right-Handed Batters",
    "closer_vs_lhb": "Closers vs. Left-Handed Batters",
    "closer_vs_rhb": "Closers vs. Right-Handed Batters",
}

ROTATION_SIZE = 4

# Positions filled in lineups
BASE_POSITIONS = ["C", "1B", "2B", "3B", "SS", "LF", "CF", "RF"]

# When multiple platoon slots compete, fill outfield before infield (CF platoon before 3B).
PLATOON_POSITION_PRIORITY = {"CF": 0, "RF": 1, "LF": 2, "DH": 3, "1B": 4, "3B": 5, "2B": 6, "SS": 7, "C": 8}
