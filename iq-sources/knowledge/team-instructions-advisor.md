# Team Instructions Advisor
## Diamond Mind Baseball - Tactical Settings Guide

**Purpose:** Provide evidence-based recommendations for all 11 team-wide tactical settings  
**Scope:** Team Instructions (`/manage/team_tendencies`) - Scale 1-5  
**Sources:** Official DMB documentation + Community research (Barry Gillis, Steve Mutzu, expert consensus)

---

## How to Use This Guide

### Understanding the 1-5 Scale
- **1** = Most Aggressive (maximum frequency/risk-taking)
- **3** = Neutral (MLB average, historical play-by-play data)
- **5** = Most Conservative (minimum frequency/risk-averse)

### Official Default Behavior (DMB FAQ)
*"If you leave your team and player tendency settings unchanged, the Computer Manager decisions will follow the patterns found in major league play-by-play data. Managers will not necessarily do the same thing every time—they might have a player bunt two-thirds of the time and swing away one-third. Adjusting settings alters this ratio."*

---

## TEAM INSTRUCTIONS SETTINGS

### 1. BUNTING FOR HIT (Scale: 1-5)

**What It Controls:** Frequency of drag bunts and surprise bunt attempts for base hits

**Official Behavior (Setting 3):**
- Computer Manager attempts bunts for hits only with best bunters (Ex/Vg rated)
- Considers game situation, pitcher quality, defensive positioning
- Even at neutral, bunt-for-hit is rare tactic

**Community Research:**
- **IS_cwolfson & Tom Tippett Analysis:**
  - Maximum frequency (1): Ex bunters attempt ~1 per 3 PA
  - Success rate: Ex bunters achieve ~40% against average defenses
  - Situational factors reduce attempts (strikes, multiple outs, deficit)
  - Contact rate: Only ~50% of attempts put ball in play
  
- **Low Cap League Finding (SSG):**
  - Bunt-for-hit ability unaffected by PTL (pumpkin) exhaustion
  - Minimum salary players with Ex bunt-for-hit can exploit this
  - Examples: '11 Jarrod Dyson, '27 Adam Comorosky
  - Reality check: Even perfect execution rarely exceeds replacement level

**Recommended Settings:**

| Team Type | Setting | Reasoning |
|-----------|---------|-----------|
| **Most teams** | **4-5** | Low-value tactic; waste outs |
| Ex bunters + speed | 2-3 | Only if roster supports (5+ Ex bunters) |
| Power-oriented | 5 | Completely avoid |
| Contact/speed team | 3 | Allow in optimal situations |

**Player Override Priority:** 
- Set Ex bunters individually to 2-3 if you want more attempts
- Set poor bunters (Fr/Pr) to 5-6 to prevent attempts

**Key Insight:** *Even at maximum aggressive setting, Ex bunters only attempt ~33% of PA. Computer Manager never forgets to bring corners in, limiting effectiveness.*

---

### 2. SACRIFICE BUNTING (Scale: 1-5)

**What It Controls:** Frequency of sacrifice bunt attempts in appropriate situations (runner on, <2 outs)

**CRITICAL WARNING - Computer Manager Behavior:**
> *"Computer Manager has 1960s/70s mentality—likes to bunt early even at Setting 5. Setting 5 does NOT mean 'only bunt late in close games'."*  
> — Community consensus (documented in Strategy Guide)

**Official Behavior (Setting 3):**
- Computer Manager bunts with best bunters in appropriate situations
- Considers inning, score, base runners, batter quality vs. pitcher
- Realistic variation: might bunt 2/3 of time, swing away 1/3

**Community-Proven Anti-Bunting Strategy:**

**RECOMMENDED APPROACH (Barry Gillis / Community):**
1. **Set Team "Sacrifice Bunting" to 6 (Never)** — prevents early-game bunting with good hitters
2. **Set pitchers/weak hitters individually to 5** — allows occasional sacrifice situations
3. **Result:** Prevents quality hitters from bunting in early innings while allowing strategic bunts from appropriate players

**Why This Matters:**
- Computer Manager will bunt with .300 hitters in 3rd inning with runner on 1st, 0 outs
- This wastes outs and opportunities with quality bats
- Setting 5 is NOT conservative enough due to Computer Manager's old-school programming

**Recommended Settings:**

| Team Type | Setting | Reasoning |
|-----------|---------|-----------|
| **ALL teams** | **6 (Never)** | Prevent early-game bunting mistakes |
| Then override: Pitchers | 5 (individual) | Allow rare pitcher sacrifices |
| Then override: Weak hitters | 5 (individual) | Allow situational bunts |

**Evidence:**
- Community testing shows Setting 5 still allows too many early bunts
- Setting 6 (Never) + individual overrides = optimal control
- Power/run production more valuable than sacrifice outs

---

### 3. SQUEEZE BUNTING (Scale: 1-5)

**What It Controls:** Frequency of squeeze bunt attempts (runner on 3rd, <2 outs)

**Official Behavior (Setting 3):**
- Extremely situational: runner on 3rd, good bunter, <2 outs
- Computer Manager considers bunt rating, pitcher quality, game situation
- Rare tactic even at aggressive settings

**Community Research:**
- **Situation Requirements:** Runner on 3rd + good bunter + appropriate score
- **Success Factors:** Ex/Vg bunt rating + contact skills + low K rate
- **Risk Assessment:** Failed squeeze = strikeout or double play opportunity

**Recommended Settings:**

| Team Type | Setting | Reasoning |
|-----------|---------|-----------|
| **Most teams** | **4-5** | Rare tactic, high risk |
| High-contact, low-power | 2-3 | Only if roster supports strategy |
| Power-oriented | 5 | Waste at-bat for power hitters |
| Speed/small-ball focused | 3 | Allow in optimal situations |

**When It Makes Sense:**
- Runner on 3rd with <2 outs, tied/close game, weak hitter at plate
- Ex bunt rating + low strikeout rate
- Pitcher with poor control (high walk rate)

**Key Insight:** Even at Setting 1, squeeze is situational. Setting 4-5 makes it extremely rare without prohibiting entirely.

---

### 4. BASE RUNNING (Scale: 1-5)

**What It Controls:** Aggressiveness taking extra bases, stealing frequency, risk tolerance on close plays

**Official Behavior (Setting 3):**
- Computer Manager uses player speed ratings (Ex/Vg/Av/Fr/Pr)
- Considers game situation, score, inning
- Combines with individual "Stealing" and "Baserunning" settings

**Community Research - Steve Mutzu 70% Threshold:**
> *"Goal is to steal the base every attempt, not accumulate volume. 70% success rate = break-even. Below this hurts offense more than helps."*

**Recommended Settings:**

| Team Type | Setting | Reasoning |
|-----------|---------|-----------|
| **Fast team** (avg Vg speed) | **2** | Slightly aggressive, exploit advantage |
| **Slow team** (avg Fr speed) | **4** | Conservative, avoid outs |
| **Average speed** | **3** | Neutral, default MLB patterns |
| **Elite speed** (5+ Ex runners) | **2-3** | Aggressive but not reckless |

**Cascading Effects:**
- Combines with individual player "Stealing" setting (1-5, 6=Never)
- Affects first-to-third, scoring from second, tag-ups
- Player speed rating (Ex/Vg/Av/Fr/Pr) affects success probability

**Strategic Considerations:**
- SBs more important at bottom of order than top (Barry Gillis)
- 70% success rate = break-even threshold
- Quality over quantity: 10/10 steals > 50/70 steals

**Key Insight:** This setting affects ALL baserunning decisions (not just steals). Fast teams should be 2, slow teams 4.

---

### 5. HIT-AND-RUN (Scale: 1-5)

**What It Controls:** Frequency of hit-and-run attempts (runner in motion, batter must swing)

**Official Behavior (Setting 3):**
- Runner on base (typically 1st), batter must protect runner
- Computer Manager considers contact ability, strikeout rate, game situation
- Forces batter to swing regardless of pitch quality

**Community Expert Analysis:**

**Tyler Ensor:** *"Never use it"* — doesn't like how sim employs small ball

**DvdAvins:** Default team setting 5 for average teams, 4 for weak offense + adjustments for KF, GDPF, ISO

**rcrny:** Default "Never" with selective exceptions based on K rate and offensive context

**When Hit-and-Run Works:**
- **Era Considerations:** Golden Age (fewer Ks, more balls in play) = lower CS risk, higher GIDP prevention
- **Player Profile:** Low K rate, contact hitters, slow runners prone to GIDP
- **Classic Candidates:** Ken Oberkfell, Glenn Hubbard types — no power contact hitters

**Recommended Settings:**

| Team Type | Setting | Reasoning |
|-----------|---------|-----------|
| **High strikeout team** | **4-5** | Risky with many Ks, wastes at-bats |
| **Contact-oriented team** | **3** | More effective with low K rates |
| **Power team** | **5** | Wastes extra-base hit opportunities |
| **Most teams** | **3-4** | Neutral to conservative |

**Trade-offs and Risks:**
- **Power Loss:** Turns extra-base hits into singles
- **OBP Reduction:** Forces swings at pitches out of strike zone
- **Stolen Base Inefficiency:** Pitcher may be stolen-on anyway
- **Outfield Arm Override:** Runner may not advance on RF singles vs. strong arms

**GIDP Prevention Value (simdobbers2 example):**
- Team with 200/300 SB rate, only 80 GIDPs (40 below league average)
- Net gain: ~60 extra outs through GIDP reduction
- **Primary benefit** in most situations

**Key Insight:** Setting 3 results in "very few attempts." Even aggressive (1-2) produces limited usage. Default to 3-4 unless specific roster fit.

---

### 6. TAKING PITCHES (Scale: 1-5)

**What It Controls:** Patience at plate, working counts, drawing walks vs. aggressive hitting

**Official Behavior (Setting 3):**
- Computer Manager follows MLB average plate discipline patterns
- Considers batter walk/strikeout rates, pitcher control, game situation
- Affects pitch counts seen per plate appearance

**Community Research:**
- High "Taking Pitches" → more pitches per PA → increased pitcher fatigue
- Low "Taking Pitches" → aggressive swinging → fewer walks, more contact
- Interacts with individual player "Taking Pitches" override

**Recommended Settings:**

| Team Type | Setting | Reasoning |
|-----------|---------|-----------|
| **High-walk team** | **2** | Patient approach, exploit OBP advantage |
| **Low-walk team** | **4** | Swing early, put ball in play |
| **Most teams** | **3** | Neutral, default MLB patterns |
| **Power team vs. weak pitching** | **4** | Attack early, don't let pitchers settle |

**Strategic Applications:**
- **Pitcher Fatigue Strategy:** Setting 2 increases opponent pitch counts
- **Contact Strategy:** Setting 4 gets ball in play faster
- **Player-Specific:** Override individually for high-walk or free-swinger types

**Cascading Effects:**
- Influences pitcher fatigue accumulation (more pitches = faster fatigue)
- Affects at-bat quality (working counts vs. aggressive hacking)
- Individual player override available for specific hitters

**Key Insight:** Most teams should stay at 3. Adjust to 2 if high-OBP roster, 4 if contact-heavy roster.

---

### 7. USING CLOSER (Scale: 1-5) **[MOST CRITICAL SETTING]**

**What It Controls:** How readily closer is brought into games (8th inning vs. 9th only, tie games, lead size)

**Official Values:**
- **1** = Aggressive (closer in 8th inning, tie games, 2-run leads)
- **3** = MLB typical (9th inning, traditional save situations)
- **5** = Conservative (only traditional saves: 9th inning, ≤3 run lead)

**CRITICAL INTERACTION WITH PLAYER INSTRUCTIONS:**

**Official DMB FAQ Warning:**
> *"If you set team 'Using Closer' to 1, individual 'Pull for Closer' for EACH pitcher (including closer) also changes to 1. If closer's Pull for Closer is 1, Computer Manager gives him quicker hook."*

**THE SOLUTION (Official Guidance):**
1. Set Team "Using Closer" to **1-2** (aggressive closer usage)
2. Set **Closer's** individual "Pull for Closer" to **4-5** (keep him in)
3. **Result:** Closer enters early AND stays in game

**Community Consensus:**
- **Most competitive teams:** Setting 1-2 (maximize elite closer usage)
- **Traditional approach:** Setting 4-5 (preserve closer for traditional saves)
- **Optimal strategy:** Setting 1 + individual closer "Pull for Closer" = 5

**Recommended Settings:**

| Team Type | Setting | Reasoning |
|-----------|---------|-----------|
| **Elite closer (Ex durability)** | **1** | Maximize high-leverage usage |
| **Quality closer** | **1-2** | Aggressive but measured |
| **Weak closer / committee** | **3-4** | Traditional usage pattern |
| **"Superman Reliever" strategy** | **1** | Use best reliever in most innings |

**Strategic Implications:**
- Setting 1: Closer may enter in 8th inning with tie game or 1-2 run lead
- Setting 5: Closer only for 9th+ inning, ≤3 run lead (traditional save)
- **Value Proposition:** Elite closers get 80 IP at Setting 5 vs. 100+ IP at Setting 1

**Key Insight:** This is the single most important tactical setting. Affects game outcomes more than any other. Always pair with correct individual "Pull for Closer" setting.

---

### 8. INTENTIONAL WALKS (Scale: 1-5)

**What It Controls:** Frequency of intentional walks in strategic situations (1st base open, dangerous hitter)

**Official Behavior (Setting 3):**
- Computer Manager considers game situation, hitter quality, next batter
- Typical situations: 1st base open, RISP, dangerous hitter up, weak hitter on deck
- Follows MLB historical patterns for IBB frequency

**Community Research:**

**Alan Schwarz Diamond Mind Study (2009):**
- Tested IBBs extensively in simulation
- **Finding:** "Bad strategy, costing five runs per season"
- **Note:** Aggregate finding; some situations make more sense than others

**Expert Consensus:**
- Most teams: Setting 4 (below average)
- Rarely optimal to intentionally walk (puts runner on base, wastes pitches)
- Better to challenge hitters in most situations

**Recommended Settings:**

| Team Type | Setting | Reasoning |
|-----------|---------|-----------|
| **Most teams** | **4** | Below average, avoid wasting runners |
| **Weak pitching staff** | **3** | Allow occasional strategic walks |
| **Strong pitching staff** | **5** | Challenge everyone, avoid free runners |
| **High-strikeout pitchers** | **4-5** | Trust stuff over strategy |

**When IBB Makes Sense:**
- 1st base open, RISP, elite hitter up, much weaker hitter on deck
- Late innings, close game, setting up double play
- Specific pitcher-hitter mismatch (platoon disadvantage)

**Cascading Effects:**
- Individual pitcher override available
- Interacts with "Pitch-Around" setting (related tactic)
- Affects pitch counts and fatigue

**Key Insight:** Default to 4. Intentional walks rarely provide strategic advantage. Better to pitch to hitters.

---

### 9. PITCH-AROUND (Scale: 1-5)

**What It Controls:** Nibbling vs. dangerous hitters, avoiding strikes, unintentional-intentional walks

**Official Behavior (Setting 3):**
- Computer Manager pitches carefully to dangerous hitters
- Not a full IBB, but avoiding strike zone
- Considers hitter quality, game situation, next batter

**Community Consensus:**
- Similar to IBB: Generally setting 4 (below average)
- Wastes pitches, risks walks, doesn't often prevent damage
- Better to challenge most hitters

**Recommended Settings:**

| Team Type | Setting | Reasoning |
|-----------|---------|-----------|
| **Most teams** | **4** | Below average, challenge hitters |
| **Control pitchers** | **3** | Can pitch around without walks |
| **High-walk pitchers** | **5** | Avoid compounding control issues |

**Strategic Considerations:**
- Affects pitch counts and fatigue (more pitches per batter)
- Individual pitcher override available
- Less dramatic than IBB but same principle

**Cascading Effects:**
- Increases pitch counts → faster fatigue
- May result in walks anyway (unintentional)
- Interacts with Taking Pitches (patient hitters benefit)

**Key Insight:** Setting 4 for most teams. Avoid pitching around unless specific matchup demands it.

---

### 10. PITCHOUTS (Scale: 1-5)

**What It Controls:** Frequency of pitchout attempts to prevent steals, disrupt timing

**Community Expert Consensus:**
> *"Set to 5 (conservative)—pitches too valuable to waste. Risk of errors on pickoff throws. Exception: VG/Ex hold rating + great fielding 1B."*  
> — Community consensus (documented in Strategy Guide)

**Official Behavior (Setting 3):**
- Computer Manager considers runner speed, pitcher hold rating, catcher CS%
- Attempts pitchouts when steal seems likely
- Success rate: ~50% on pitchouts (70% without pitchout)

**Community Research - Pickoff & Pitchout Analysis:**

**abywaters/HooverH Pickoff Mechanics:**
- Jump rating impact: Higher jump (VG/Ex) draws more pickoff attempts
  - Pr: ~6% pickoff attempt rate
  - Fr: ~12%
  - Av: ~18%
  - Vg: ~25%
  - Ex: ~27%
- **Problem:** When pitcher throws to first AND runner steals, pickoff chance increases significantly
- Successful pickoffs rare (~2% of attempts), mostly vs. Ex jump runners
- **Key Finding:** Pickoffs more problematic than catcher throw-outs for aggressive stealers

**Recommended Settings:**

| Team Type | Setting | Reasoning |
|-----------|---------|-----------|
| **ALL teams** | **5** | Pitches too valuable; accept steals |
| **Exception:** VG/Ex hold + great fielding 1B | 3-4 | Only if optimal pickoff conditions |
| **Strong catcher CS%** | 5 | Still avoid; rely on catcher instead |

**Why Avoid Pitchouts:**
- **Wastes pitches:** Valuable for fatigue management
- **Low success rate:** ~50% vs. 70% without pitchout (only 20% improvement)
- **Risk of errors:** Pickoff throws can go awry
- **Pushes pitcher closer to fatigue limits**

**Strategic Alternative:**
- Accept stolen bases rather than waste pitches
- Better to pitch to batter and get out
- Focus defensive energy on preventing hits, not steals

**Key Insight:** Setting 5 for virtually all teams. Pitches are too valuable. Better to accept stolen bases.

---

### 11. TEMPLATES (Load / Save / Delete)

**What It Does:** Save current Team Instructions configuration as reusable template

**Use Cases:**
- **Multiple Teams:** Apply same settings across teams in different leagues
- **Park-Specific:** Save "Coors Field" template vs. "Fenway Park" template
- **Strategy Variations:** "Aggressive" vs. "Conservative" vs. "Small Ball" templates
- **Quick Reset:** Return to known-good configuration after experimentation

**How to Use:**
1. Configure Team Instructions to desired settings
2. Click "Save Template"
3. Name template descriptively (e.g., "Power Team - Aggressive Closer")
4. Load template on other teams or after reset

**Template Library Recommendations:**
- **Default Conservative:** Bunting 6, Steals 3-4, Closer 1, Pitchouts 5
- **Power Team:** Bunting 6, Hit-Run 5, Closer 1, Taking Pitches 4
- **Speed Team:** Bunting 6, Steals 2, Base Running 2, Closer 1
- **Small Ball (rare):** Bunting 3, Squeeze 3, Hit-Run 3, Steals 2

**Key Insight:** Templates save time but always verify settings match current roster and park.

---

## RECOMMENDED BASELINE CONFIGURATION

### Universal Settings (All Teams)

| Setting | Value | Reasoning |
|---------|-------|-----------|
| **Sacrifice Bunting** | **6 (Never)** | Prevent early-game bunting mistakes |
| **Using Closer** | **1** | Maximize elite reliever usage |
| **Pitchouts** | **5** | Conserve pitches, accept steals |
| **Intentional Walks** | **4** | Below average, rarely optimal |
| **Pitch-Around** | **4** | Challenge hitters, avoid wasted pitches |

### Roster-Dependent Settings

| Setting | Fast Team | Slow Team | Power Team | Contact Team |
|---------|-----------|-----------|------------|--------------|
| **Bunting for Hit** | 3-4 | 4-5 | 5 | 3-4 |
| **Squeeze** | 3-4 | 5 | 5 | 3-4 |
| **Base Running** | 2 | 4 | 3 | 3 |
| **Hit-and-Run** | 3 | 4-5 | 5 | 3 |
| **Taking Pitches** | 2-3 | 3-4 | 4 | 3 |

---

## CRITICAL REMINDERS

### 1. Using Closer + Pull for Closer Interaction
**MUST configure both:**
- Team "Using Closer" = 1
- Closer's individual "Pull for Closer" = 4-5
- **Failure to do both = quick hook for closer**

### 2. Anti-Bunting Strategy
**MUST use setting 6 (Never):**
- Team "Sacrifice Bunting" = 6
- Then override pitchers/weak hitters individually to 5
- **Setting 5 is NOT conservative enough**

### 3. Individual Player Overrides
**These team settings can be overridden per player:**
- All 11 team settings have individual equivalents
- Individual settings: 1-5 or **6 = Never** (absolute prohibition)
- Always configure team baseline first, then individual overrides

---

## NEXT STEP: PLAYER INSTRUCTIONS

After configuring Team Instructions (the baseline), proceed to Player Instructions to:
- Override team settings for specific players
- Set "Pull for PH" and "Pull for Closer" settings
- Configure platoon triggers
- Establish individual tactical preferences

**See:** Player Instructions Advisor (next document)

---

## SOURCES

**Official:**
- Diamond Mind Baseball FAQ (imaginesports.com/bball/reference/faqs)
- DMB Rules & Instructions (imaginesports.com/bball/reference/rules)
- Tom Tippett (DMB Creator) - Bunt for Hit mechanics

**Community Research:**
- Barry Gillis - Ultimate Guide to Diamond Mind Online (2017)
- Steve Mutzu - Base stealing 70% threshold, closer strategy
- Tyler Ensor, DvdAvins, rcrny - Hit-and-run analysis
- IS_cwolfson - Bunting mechanics and success rates
- abywaters/HooverH - Pickoff/pitchout mechanics
- Alan Schwarz - Diamond Mind simulation research (2009)

**Documented In:**
- docs/rules/dmb_unified_rules.md
- Teams/The Perfecter Plan/TEAM_CONFIG_GUIDE.md
- Community strategy guide (included in provided documentation)

---

**Version:** 1.0  
**Last Updated:** January 27, 2026  
**Next:** Player Instructions Advisor