# Lineup Construction (Decision Tree)

> **IMPLEMENTATION:** Do not apply manually. Use `scripts/generate_team_config.py`.
> For platoon teams, rank heart (#3–5) by RC600 among starters; #1–2 by OBP vs that hand with speed tie-break; never sort all nine batters by OBP only.

**Purpose:** Batting order and defensive assignment logic for config intelligence.  
**Scope:** Edit Lineup (`/manage/edit_lineup`) — vs RHP and vs LHP lineups, 1–9 order, defensive positions.

---

## Quick reference (from knowledge base)

- **Leadoff:** Highest OBP + speed bonus  
- **#2:** Second-highest OBP, contact ability  
- **#3–5:** Top 3 RC600 players (heart of order)  
- **#6–9:** Descending RC600  

---

## Extended logic

For full lineup and bench workflow (including platoon pairs, defensive subs, and bench roles), see:

- This repo: `knowledge/team-instructions-configurator.md` (example team context and PH/platoon settings)
- dmo repo (if available): `Teams/The Perfecter Plan/TEAM_CONFIG_GUIDE.md` for step-by-step lineup + bench + defensive position guidance

---

**To implement:** Add lineup slot rules (OBP/RC600 by slot), defensive spectrum, and platoon ordering when building the config generator.
