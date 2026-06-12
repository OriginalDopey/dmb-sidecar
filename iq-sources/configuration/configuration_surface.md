# Diamond Mind Baseball — Configuration Surface (Discovery Reference)

**Purpose:** Inventory of DMB configuration screens and settings for documentation/integration projects.  
**Source:** ImagineSports.com (DMB online), community docs, and this repo’s rules/team guides.  
**Last updated:** Jan 2025

---

## Which approach fits?

**Option C applies:** We already have documentation and structured references you can use. Below is the configuration surface in one place. Option B (web search for official docs) is still useful for primary sources like [Imagine Sports Reference](https://imaginesports.com/bball/reference/popup).

---

## Configuration surface overview

DMB team configuration is done on **8 main UI areas**. Paths are relative to the league/team context (e.g. `https://imaginesports.com/bball/...`).

| # | Screen / Area | URL Path | What it controls |
|---|----------------|----------|-------------------|
| 1 | **Edit Lineup** | `/manage/edit_lineup` | Batting order (1–9), defensive positions, lineups vs RHP and vs LHP |
| 2 | **Manage My Bench** | `/manage/field_subs` | Pinch hitters (vs LHP / vs RHP), utility players, platoon pairs, defensive subs — **per lineup** |
| 3 | **Pitching Rotation** | `/manage/pitch_rotation` | SP slots 1–5, Ace/Double Ace/Spot Starter options, 4- vs 5-man rotation |
| 4 | **Bullpen Assignments** | `/manage/pitch_relief` | Roles: Closers (LHB/RHB), Set-up (LHB/RHB), Long relief, Mop-up, Injury-replacement starters (up to 3 per role) |
| 5 | **Team Instructions** | `/manage/team_tendencies` | Team-wide tactics 1–5: bunting, base running, hit-and-run, taking pitches, using closer, IBB, pitch-around, pitchouts, etc. |
| 6 | **Player Instructions** | `/manage/player_tendencies` | Per-player overrides: bunting, stealing, hit-and-run, baserunning, pull-for-PH (vs LHP/RHP/platoon), pull-for-reliever/closer; **6 = Never** |
| 7 | **Bank Account / Loans** | `/frontoffice/loans` | Cash balance, interest, loans, payment history |
| 8 | **Cash Worksheet** | `/frontoffice/cash_worksheet` | Sign/release combos, trade-in values, loan need, bookmarking players for transactions |

**Important:** Every screen has an explicit **Save** (e.g. “Save this Lineup”, “Save Bench Roles”, “Save Team Instructions”). Changes are not applied until that button is used.

---

## 1. Edit Lineup (`/manage/edit_lineup`)

- **Lineup dropdown:** e.g. “Primary vs. RHP”, “Primary vs. LHP”
- **Batting positions 1–9:** Player dropdown per slot; shows position and handedness (* L, # switch)
- **Defensive position** shown per slot (LF, CF, RF, etc.)
- **Tabs:** Batting, Defensive, Batter ratings
- **Save:** “Save this Lineup”

---

## 2. Manage My Bench (`/manage/field_subs`)

- **Per lineup:** Bench is configured separately for each lineup (e.g. vs RHP vs vs LHP)
- **Pinch hitters:** Up to 3 vs LHP, up to 3 vs RHP
- **Utility players:** Up to 3 for injury/defensive subs
- **Platoon players:** L/R pair per position
- **Defensive subs:** One per position for late-game
- **Save:** “Save Bench Roles”

---

## 3. Pitching Rotation (`/manage/pitch_rotation`)

- **Slots 1–5:** One SP per slot; leave #5 empty for 4-man rotation
- **Options:** Use Ace (3 days rest), Use Double Ace, Use Spot Starter (#5 only when others have &lt;4 days rest)
- **Save:** “Save Pitching Rotation”

---

## 4. Bullpen Assignments (`/manage/pitch_relief`)

- **Roles (up to 3 pitchers each):** Injury replacement starters, Mop-up, Long relievers, Set-up vs LHB, Set-up vs RHB, Closers vs LHB, Closers vs RHB
- **Save:** “Save Bullpen Roles”

---

## 5. Team Instructions (`/manage/team_tendencies`)

- **Scale 1–5:** 1 = most frequent/aggressive, 3 = average, 5 = least
- **Offensive:** Bunting for hit, Sacrifice bunt, Squeeze, Base running, Hit-and-run, Taking pitches
- **Pitching:** Using closer, Intentional walks, Pitch-around, Pitchouts
- **Templates:** Load / Save / Delete instruction templates
- **Save:** “Save Team Instructions”

---

## 6. Player Instructions (`/manage/player_tendencies`)

- **Overrides:** Per-player; “(Team Default)” or 1–5 or **6 = Never**
- **Toggles:** Show Hitters | Pitchers
- **Hitter options:** Bunting, Stealing, Hit-and-run, Baserunning, Taking pitches, Pull for PH (vs LHP/RHP/Platoon), Double switch, PH in blowout
- **Pitcher options:** Pitching around, IBB, Pickoffs, Pitchouts, Pull for reliever, Pull for closer
- **Save:** “Save Player Instructions”

---

## 7. Bank Account (`/frontoffice/loans`)

- Balance, interest (5% weekly on positive balance), loan terms (15% per week in advance), payment schedule

---

## 8. Cash Worksheet (`/frontoffice/cash_worksheet`)

- Equal count of sign/release; trade-in (e.g. 75%) per release; loan calculation; bookmark players for sign/release

---

## Docs in this repo you can share

| Doc | Path | Use for |
|-----|------|--------|
| **Unified rules (config + game rules)** | `docs/rules/dmb_unified_rules.md` | Team Management, Lineup, Pitching, Tactical Settings, validation rules |
| **UI screen reference** | `.cursor/rules/dm-live-cursorrules.mdc` → “UI Screen Reference Guide” | Canonical list of 8 paths and what each screen is for |
| **Full team config walkthrough** | `Teams/The Perfecter Plan/TEAM_CONFIG_GUIDE.md` | Step-by-step for Lineups, Bench, Rotation, Bullpen, Team/Player Instructions, including exact fields and examples |
| **Another full config example** | `Teams/The Consensus Replacements/Complete_Team_Configuration.md` | Same structure with UI locations and checklists |

---

## Suggested next step for discovery

1. **Use Option C:** Send or point to `TEAM_CONFIG_GUIDE.md` and the “UI Screen Reference Guide” section of the cursor rules (or this file) as the configuration surface.
2. **Use Option B where needed:** Imagine Sports reference and FAQ for official naming and behavior of each setting.
3. **Option A (screenshots / walkthrough):** Still useful for UI layout and exact labels, especially for front-office screens (`/frontoffice/loans`, `/frontoffice/cash_worksheet`) which are less described in our markdown.

If you want a single “configuration inventory” artifact, this file plus the UI paths table above is a minimal, shareable summary.
