# Team vs Player Settings - Complete Relationship Map
## Understanding Team-Only vs Team-Default-with-Override Settings

**Purpose:** Define which settings are team-only vs. which act as defaults that can be overridden per-player  
**Critical for:** Building bottom-up advisor that explains when/how player overrides work

---

## SETTING ARCHITECTURE

### Two Types of Settings

**Type A: TEAM-ONLY SETTINGS**
- No player-level override available
- Apply equally to all players
- Cannot be customized per-player

**Type B: TEAM-DEFAULT-WITH-PLAYER-OVERRIDE**
- Team setting = baseline/default behavior
- Each player can override with individual setting
- Player override takes precedence when set
- "(Team Default)" option available at player level

---

## COMPLETE SETTING BREAKDOWN

### TEAM-ONLY SETTINGS (No Player Override)

These settings exist ONLY at the team level and apply universally:

| # | Setting | Location | Effect | Cannot Override |
|---|---------|----------|--------|-----------------|
| 1 | **Squeeze Bunting** | Team Instructions | Frequency of squeeze attempts | ✓ No player override |
| 2 | **Templates** | Team Instructions | Save/load configurations | ✓ Not applicable to players |

**Why No Override:**
- **Squeeze:** Situational tactic (runner on 3rd) - Computer Manager decides based on team setting + bunt rating
- **Templates:** Infrastructure feature, not tactical decision

---

### TEAM-DEFAULT-WITH-PLAYER-OVERRIDE SETTINGS

These exist at BOTH team and player levels. Player setting overrides team when specified:

#### OFFENSIVE TACTICS (6 settings)

| # | Team Setting | Player Override | How Override Works | Player Scale |
|---|-------------|-----------------|-------------------|--------------|
| 1 | **Bunting for Hit** | Bunting | Player setting overrides team default | 1-5, **6=Never** |
| 2 | **Sacrifice Bunting** | Bunting | Same override as "Bunting for Hit" | 1-5, **6=Never** |
| 3 | **Base Running** | Baserunning | Player baserunning aggressiveness | 1-5 |
| 4 | **Hit-and-Run** | Hit-and-Run | Player H&R frequency | 1-5, **6=Never** |
| 5 | **Taking Pitches** | Taking Pitches | Player plate patience | 1-5 |
| 6 | **Stealing** | Stealing | Player steal attempt frequency | 1-5, **6=Never** |

**Critical Note - Bunting:**
- Team has TWO bunting settings (Bunt for Hit + Sacrifice)
- Player has ONE bunting setting that overrides BOTH team settings
- Player "Bunting = 6 (Never)" prevents all bunts (hit, sacrifice, squeeze)

**Official Guidance (Stealing):**
> *"On neutral setting (3), top base stealer will steal at virtually every opportunity. Setting to 1-2 makes him MORE aggressive, taking risks you may not want. Lowering team setting keeps weaker runners from going (but not best stealers)."*

#### PITCHING TACTICS (4 settings)

| # | Team Setting | Player Override | How Override Works | Player Scale |
|---|-------------|-----------------|-------------------|--------------|
| 1 | **Intentional Walks** | Intentional Walks | Pitcher IBB frequency | 1-5, **6=Never** |
| 2 | **Pitch-Around** | Pitching Around | Pitcher nibbling frequency | 1-5, **6=Never** |
| 3 | **Pitchouts** | Pitchouts | Pitcher pitchout frequency | 1-5, **6=Never** |
| 4 | **Pickoffs** | Pickoffs | Pitcher pickoff attempt frequency | 1-5, **6=Never** |

**Note:** These are PITCHER-specific player overrides (not in standard "Player Instructions" for hitters)

#### SUBSTITUTION/USAGE SETTINGS (Player-Only, No Team Equivalent)

These exist ONLY at player level - no team default:

| # | Player Setting | Effect | Scale |
|---|---------------|--------|-------|
| 1 | **Pull for PH vs LHP** | How often CM may **pinch-hit *for* this player** (lift him) when opp. is LHP | 1-5, **6=Never** (see DMB FAQ: high values **protect** stars from being PH’d for) |
| 2 | **Pull for PH vs RHP** | Same, when opposing pitcher is RHP | 1-5, **6=Never** |
| 3 | **Pull for PH (Platoon)** | Same, in platoon-disadvantage spots | 1-5, **6=Never** |
| 4 | **Double Switch** | Use in double-switch situations | 1-5, **6=Never** |
| 5 | **PH in Blowout** | Pinch-hit in lopsided games | 1-5, **6=Never** |
| 6 | **Pull for Reliever** | Remove pitcher for reliever | 1-5, **6=Never** |
| 7 | **Pull for Closer** | Remove pitcher for closer | 1-5, **6=Never** |

**Why No Team Equivalent:**
- These control Computer Manager's usage of SPECIFIC players
- Team-wide setting wouldn't make sense (each player has different role)
- Example: Can't have team-wide "Pull for Closer" - only specific pitchers are closers

---

## CRITICAL INTERACTION: USING CLOSER

**Special Case:** Team setting affects ALL player defaults

### The Interaction

**Team Setting:** "Using Closer" (1-5)
- Controls how aggressively closer is brought into games
- **ALSO automatically changes ALL pitchers' "Pull for Closer" defaults**

**Official DMB FAQ:**
> *"If you set team 'Using Closer' to 1, individual 'Pull for Closer' for EACH pitcher (including closer) also changes to 1."*

### The Problem

**What You Want:**
- Closer enters games aggressively (8th inning, tie games)
- Closer STAYS in game once he enters

**What Team "Using Closer" = 1 Does:**
- ✓ Closer enters early (GOOD)
- ✗ All pitchers' "Pull for Closer" = 1 (BAD)
- ✗ Closer gets quick hook because his "Pull for Closer" = 1 (BAD)

### The Solution

**Two-Step Configuration:**
1. **Team "Using Closer"** = 1 (aggressive entry)
2. **Closer's "Pull for Closer"** = 4-5 (keep him in)

**Result:** Closer enters early AND stays in game

### Why This Is Unique

This is the ONLY team setting that automatically changes player defaults. All other team settings are independent of player settings.

---

## HOW OVERRIDES WORK

### Override Logic

```
IF player setting = "(Team Default)" OR not set
  THEN use team setting value
ELSE
  use player setting value (overrides team)
END
```

### Setting Priority

1. **Player Setting (if specified)** ← HIGHEST PRIORITY
2. Team Setting (if player = "Team Default")
3. Computer Manager logic (within setting bounds)

### Scale Differences

**Team Settings:** 1-5 only
- 1 = Most frequent/aggressive
- 3 = Neutral (MLB average)
- 5 = Least frequent/conservative

**Player Settings:** 1-5 + **6 = Never**
- 1-5 = Same as team scale
- **6 = Never** = Absolute prohibition (only at player level)

**Why 6 Exists:**
- Allows absolute control for specific players
- Example: Setting star slugger's "Bunting" to 6 prevents ALL bunts
- Cannot set team-wide to 6 (would prevent all team bunting in all situations)

---

## PRACTICAL APPLICATIONS

### When to Use Team Setting Only

**Set team baseline, leave players at "(Team Default)":**
- Most players don't need special treatment
- Team setting handles 80%+ of cases
- Examples:
  - Taking Pitches = 3 for most teams
  - Hit-and-Run = 3-4 for average roster
  - Base Running = 3 for average speed team

### When to Override Individual Players

**Override when player has specific characteristic:**

**1. ELITE SKILL** (use it more)
- Ex bunters: Override "Bunting" to 2-3
- Elite stealers: Override "Stealing" to 3 (NOT 1-2, per official guidance)
- High-walk hitters: Override "Taking Pitches" to 2

**2. POOR SKILL** (prevent usage)
- Poor bunters (Fr/Pr): Override "Bunting" to 6 (Never)
- Slow runners: Override "Stealing" to 5-6
- High-K hitters: Override "Hit-and-Run" to 6 (Never)

**3. STRATEGIC ROLE** (player-specific usage)
- Platoon players: "Pull for PH (Platoon)" = 1
- Star players: "Pull for PH vs LHP/RHP" = 5-6 (keep in)
- Closer: "Pull for Closer" = 4-5 (keep in when entered)
- Setup men: "Pull for Reliever" = 1 (quick hook if struggling)

**4. ROSTER CONSTRUCTION** (team strategy)
- Anti-bunting: Team = 6, then pitchers/weak hitters = 5 individually
- Aggressive baserunning: Team = 2, then slow runners = 4-5 individually
- Conservative closer usage: Team = 5, but elite closer can still enter early if needed

---

## COMMON MISCONFIGURATION PATTERNS

### Mistake #1: Not Overriding Closer's Pull Setting
**Problem:**
- Team "Using Closer" = 1
- Closer's "Pull for Closer" = (Team Default) = 1
- **Result:** Closer enters early but gets quick hook

**Fix:**
- Closer's "Pull for Closer" = 4-5 explicitly

### Mistake #2: Setting Elite Stealers to 1-2
**Problem:**
- Elite stealer already steals optimally at setting 3
- Setting 1-2 = too aggressive, lowers success rate below 70%
- **Result:** More caught stealing, hurts offense

**Fix:**
- Elite stealers: Leave at 3 or use 4 for higher success rate
- Only ~30 players in entire game deserve setting 2

### Mistake #3: Using Team Bunting = 5 Instead of 6
**Problem:**
- Computer Manager has "1960s mentality"
- Setting 5 ≠ "only late/close games"
- **Result:** Still bunts with quality hitters early in games

**Fix:**
- Team "Sacrifice Bunting" = 6 (Never)
- Then pitchers/weak hitters = 5 individually

### Mistake #4: Not Using "6 = Never" for Power Hitters
**Problem:**
- Power hitter set to "Bunting = 5" (conservative)
- Computer Manager still bunts occasionally ("keep defense honest")
- **Result:** Wastes at-bat with slugger

**Fix:**
- Power hitters: "Bunting = 6 (Never)" to absolutely prevent

### Mistake #5: Overriding Too Many Players
**Problem:**
- Overriding 20+ players individually
- Difficult to manage, easy to make mistakes
- Loses benefit of team baseline

**Fix:**
- Override only players with extreme characteristics
- Let team setting handle average players
- Target: Override 5-10 players per team maximum

---

## DECISION FRAMEWORK: TEAM VS PLAYER SETTING

### Step 1: Configure Team Baseline

**For each of 11 team settings, ask:**
1. What is my roster's overall characteristic? (fast/slow, power/contact, etc.)
2. What is my general strategy? (aggressive/conservative)
3. Set team baseline accordingly (1-5)

**Result:** Team settings reflect general approach

### Step 2: Identify Override Candidates

**For each player, ask:**
1. Does this player have ELITE skill to exploit?
2. Does this player have POOR skill to prevent?
3. Does this player have SPECIFIC ROLE that requires special treatment?
4. Is this player's characteristic OPPOSITE of team baseline?

**If YES to any:** Override player setting  
**If NO to all:** Leave at "(Team Default)"

### Step 3: Configure Overrides

**For players identified in Step 2:**
1. Set individual player override (1-5 or 6)
2. Document reasoning (for future reference)
3. Verify setting makes sense with player ratings

**Example Decision Tree (Bunting):**
```
Player: Mike Trout
- Bunting rating: Fr (Fair)
- Role: Star slugger
- Team bunting: 6 (Never)

Decision:
- Elite skill to exploit? NO (Fr bunting)
- Poor skill to prevent? YES (Fr bunting, power hitter)
- Specific role? YES (star, should never waste at-bat)
- Opposite team baseline? NO (team already set to Never)

Override: 6 (Never) ← Reinforces team setting, ensures no bunts
```

```
Player: Pitcher (NL, weak hitter)
- Bunting rating: Vg (Very Good)
- Role: Pitcher, weak hitter
- Team bunting: 6 (Never)

Decision:
- Elite skill to exploit? YES (Vg bunting)
- Poor skill to prevent? NO
- Specific role? YES (pitcher, appropriate to sacrifice)
- Opposite team baseline? YES (team = Never, but pitcher should bunt)

Override: 5 (Conservative) ← Allows rare sacrifice situations
```

---

## SUMMARY REFERENCE TABLE

### Complete Setting Architecture

| Setting Category | Team Setting | Player Override | Override Scale | Notes |
|-----------------|-------------|-----------------|----------------|-------|
| **TEAM-ONLY** | | | | |
| Squeeze Bunting | 1-5 | ❌ None | N/A | Team only |
| Templates | N/A | ❌ None | N/A | Infrastructure |
| **OFFENSIVE (Team Default + Override)** | | | | |
| Bunting for Hit | 1-5 | ✓ Bunting | 1-5, 6=Never | One player setting for both team bunting types |
| Sacrifice Bunting | 1-5 | ✓ Bunting | 1-5, 6=Never | Same as above |
| Base Running | 1-5 | ✓ Baserunning | 1-5 | Separate from Stealing |
| Hit-and-Run | 1-5 | ✓ Hit-and-Run | 1-5, 6=Never | |
| Taking Pitches | 1-5 | ✓ Taking Pitches | 1-5 | |
| Stealing | 1-5 | ✓ Stealing | 1-5, 6=Never | Part of Base Running system |
| **PITCHING (Team Default + Override)** | | | | |
| Using Closer | 1-5 | ❌ None* | N/A | *Affects all "Pull for Closer" defaults |
| Intentional Walks | 1-5 | ✓ Intentional Walks | 1-5, 6=Never | Pitcher override |
| Pitch-Around | 1-5 | ✓ Pitching Around | 1-5, 6=Never | Pitcher override |
| Pitchouts | 1-5 | ✓ Pitchouts | 1-5, 6=Never | Pitcher override |
| Pickoffs | ❌ None | ✓ Pickoffs | 1-5, 6=Never | Player-only (no team setting) |
| **USAGE/SUBSTITUTION (Player-Only)** | | | | |
| Pull for PH vs LHP | ❌ None | ✓ Pull for PH vs LHP | 1-5, 6=Never | No team equivalent |
| Pull for PH vs RHP | ❌ None | ✓ Pull for PH vs RHP | 1-5, 6=Never | No team equivalent |
| Pull for PH (Platoon) | ❌ None | ✓ Pull for PH (Platoon) | 1-5, 6=Never | No team equivalent |
| Double Switch | ❌ None | ✓ Double Switch | 1-5, 6=Never | No team equivalent |
| PH in Blowout | ❌ None | ✓ PH in Blowout | 1-5, 6=Never | No team equivalent |
| Pull for Reliever | ❌ None | ✓ Pull for Reliever | 1-5, 6=Never | No team equivalent |
| Pull for Closer | ❌ None | ✓ Pull for Closer | 1-5, 6=Never | No team equivalent (but affected by team "Using Closer") |

---

## ADVISOR IMPLICATIONS

### Bottom-Up Approach

**Phase 1: Team Instructions Advisor**
- Configure all 11 team settings first
- Establish baseline behavior
- No player-specific knowledge required

**Phase 2: Player Instructions Advisor**
- Requires Phase 1 complete (team baseline established)
- Requires player roster knowledge (ratings, roles)
- Identifies override candidates based on:
  - Player ratings (Ex/Vg/Av/Fr/Pr in relevant skills)
  - Player role (starter, platoon, closer, etc.)
  - Team strategy (aggressive/conservative)

### Advisor Workflow

```
1. User completes Team Instructions Advisor
   ↓
2. System saves team baseline configuration
   ↓
3. User provides roster (players + ratings + roles)
   ↓
4. Player Instructions Advisor analyzes:
   - Which players need overrides?
   - What override values?
   - Why override needed?
   ↓
5. System recommends player-specific overrides
   ↓
6. User reviews and applies recommendations
```

---

## SOURCES

**Official:**
- Diamond Mind Baseball FAQ (imaginesports.com)
- Player Instructions documentation
- Team Instructions documentation

**Community:**
- Barry Gillis - Anti-bunting strategy
- Steve Mutzu - Stealing threshold, closer configuration
- Community consensus - Using Closer + Pull for Closer interaction

**Documented In:**
- docs/rules/dmb_unified_rules.md (Tactical Settings section)
- Teams/The Perfecter Plan/TEAM_CONFIG_GUIDE.md (Section 6: Player Instructions)

---

**Version:** 1.0  
**Last Updated:** January 27, 2026  
**Next:** Player Instructions Advisor (requires team baseline + roster data)