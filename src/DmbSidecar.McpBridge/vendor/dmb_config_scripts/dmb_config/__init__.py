"""Deterministic DMB team-config generator.

Encodes the Diamond Mind Baseball configuration decision trees (lineup ordering,
platoon selection, rotation/bullpen roles, team/player instructions, IR) as pure
Python so that ANY model -- or no model at all -- produces the same
IMPLEMENTATION_PLAN.md from the same inputs.

Source of truth:
  - Live roster scrape  -> who is on the team, active/IR, salary, cash, park, DH
  - Raw IS CSV exports   -> player ratings & platoon splits (RC600, OPS/OBP vs L/R,
                            FLD range/error, Run, Jump, SPDur, RPDur, Hold, ERA, ERC)

The rule modules mirror the knowledge base in
New-DMO/knowledge/decision_trees/ and the advisor docs.
"""

__version__ = "1.0"
