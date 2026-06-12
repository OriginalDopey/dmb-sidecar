# Diamond Mind Baseball: Unified Game Rules

> **Version:** 2025.2  
> **Last Updated:** December 30, 2024  
> **Source:** Official DMB documentation from imaginesports.com reference system  
> **Note:** This file contains the standard DMB rules. League-specific modifications should be documented in separate `league_rules.md` files.

---

## Table of Contents

1. [Game Basics](#game-basics)
2. [League Types & Setup](#league-types--setup)
3. [Player Pool & Ratings](#player-pool--ratings)
4. [Roster Management](#roster-management)
5. [Drafting System](#drafting-system)
6. [Team Management](#team-management)
7. [Lineup & Position Rules](#lineup--position-rules)
8. [Pitching & Bullpen Management](#pitching--bullpen-management)
9. [Tactical Settings](#tactical-settings)
10. [Transactions & Trading](#transactions--trading)
11. [Season Structure & Scheduling](#season-structure--scheduling)
12. [Playoffs & Postseason](#playoffs--postseason)
13. [Fatigue & Injury System](#fatigue--injury-system)
14. [Ballparks & Eras](#ballparks--eras)
15. [Financial System](#financial-system)
16. [Computer Manager](#computer-manager)
17. [Frequently Asked Questions](#frequently-asked-questions)

---

## Game Basics

### Core Philosophy
Diamond Mind Baseball simulates what might happen if players from throughout baseball history were pitted against each other in all sorts of different circumstances. The focus is on competitive team building and strategic management rather than replicating historical statistics.

**Design Philosophy (Tom Tippett Interview):**

**Statistical Realism Over Visual Realism:**
- DMB prioritizes **statistical accuracy and tactical realism** over graphics, animation, and sound
- Focus is on making the game **feel and work realistically** through accurate statistics and risk-reward profiles
- Textual, radio-style play-by-play commentary allows players to create mental images more realistic than early graphics/animation could provide
- Emphasis on strategy and tactical decision-making matches real-life manager experiences

**Risk-Reward Profiles:**
- Tactical decisions offer same risk-reward profiles that real-life managers face
- Count-aware decision making (pitch-by-pitch model enables realistic bunt/steal timing)
- Defensive managers can actively defend against tactics (pitchouts, pickoffs) rather than being passive observers
- Outcomes reflect realistic consequences (failing bunt/steal attempt puts batter behind in count)

**Data-Driven Approach:**
- Player ratings based on objective data from Project Scoresheet/Retrosheet
- Weather modeling uses comprehensive historical data plus external weather stations
- Statistical accuracy achieved through extensive data compilation and analysis
- Focus on what actually happened in baseball, not what looks good on screen

**Strategic Depth:**
- Game designed for avid baseball fans who value strategy over casual gaming
- Pitch-by-pitch simulation enables realistic tactical decision-making
- Manager has same strategic options as real-life managers
- Emphasis on building fundamentally sound teams that win through strategy

**Why This Matters:**
This design philosophy means DMB is optimized for players who want to experience the strategic and statistical aspects of baseball management, not just watch animated games. Every design decision prioritizes realism and strategic depth over visual appeal.

### Basic Requirements
- **Roster Size:** 25 active players + 3 Inactive Reserve (IR)
- **Minimum Pitchers:** 8 total, with at least 4 designated as Starting Pitchers (SP)
- **Minimum Catchers:** 1 required (Computer Manager will not replace if no backup available)
- **Salary Cap:** Varies by league type (see League Types section)
- **Season Length:** 162 games over 9 weeks (3 games per day for standard pace)

### Game Fundamentals
- **Play Schedule:** Games simulated daily at specific times (typically midnight, 9am, 3pm PT)
- **Series Format:** 18 series of 9 games each (standard leagues)
- **Team Structure:** 12 teams divided into 3 divisions
- **Home/Away Split:** Perfectly balanced 81 home/81 away games

### Simulation Engine: Pitch-by-Pitch Model

**Source:** Tom Tippett interview - Developer insights on simulation mechanics

**Core Innovation:**
Diamond Mind Baseball simulates **every pitch** in both pitch-by-pitch and batter-by-batter modes. This fundamental design decision enables realistic count-based tactical decisions that match real-life baseball strategy.

**Two Modes, Same Engine:**

**Pitch-by-Pitch Mode:**
- Reports result of every pitch
- Manager can change tactics between pitches
- Full visibility into count progression
- More control but takes longer to play

**Batter-by-Batter Mode:**
- Simulates pitches silently until plate appearance resolves
- Reports final outcome of plate appearance
- Still uses full pitch-by-pitch simulation internally
- Faster gameplay while maintaining realism

**Tactical Implications:**

**Count-Aware Decision Making:**
- Outcome probabilities differ based on count (2-0 count different from 0-2 count)
- Real-life strategy involves decisions heavily influenced by ball-strike count
- If batter gets into good hitter's count, bunting or stealing may waste that advantage
- Opposing managers try to guess tactics from pitch to pitch

**Bunt/Steal Timing:**
- In pitch-by-pitch mode, offensive manager has freedom to choose when to use tactics
- Defensive manager can defend with pitchouts and pickoff throws
- When batter takes strike on steal play or fails to get bunt down, falls behind in count
- Falling behind in count gives pitcher advantage (realistic consequence)
- In batter-by-batter only games, defensive manager is passive observer on these plays

**Strategic Advantages:**
- Tactical decisions are count-aware and situation-specific
- Risk-reward profiles match real-life manager decisions
- Full pitch simulation ensures realistic outcome distributions
- Manager has same strategic options as real-life managers

**Why This Matters:**
The pitch-by-pitch model is what enables DMB to offer realistic tactical decision-making. Without it, managers would be making decisions without full context of the count and game situation, reducing strategic depth and realism.

---

## League Types & Setup

### Classic Standard Leagues
- **Player Pool:** 5,000+ career-rated players from all of baseball history
- **Salary Cap:** $100 million starting budget
- **Weekly Income:** Regular payments throughout season
- **Pace:** 3 games per day (standard)
- **Draft:** Automated snake draft, 28 rounds
- **DH Option:** Available with or without designated hitter
- **Formation:** Constantly forming leagues open to all players

### Single Season (SSG) Leagues  
- **Player Pool:** Seasonal ratings for every MLB player (1920-present)
- **Salary Cap:** $120 million starting budget
- **Pace:** 9 games per day (accelerated)
- **Draft:** Automated snake draft
- **DH Rule:** DH only format

### Custom Leagues
- **Salary Cap:** $50M to unlimited (commissioner configurable)
- **Structure:** Flexible team count and divisions  
- **Rules:** Fully customizable by commissioner
- **Player Pool:** Can be limited (Random, By Seasons, etc.)
- **Pace:** Standard (3/day), Accelerated (9/day), or Turbo (18/day)
- **Formation:** Created by individual team owners

### Ladder Leagues
Special competitive format with promotion/relegation system:

#### Standard Association
- **Format:** Standard league rules with DH alternating by season
- **Structure:** 8-division ladder (Major League to Independent League)
- **Player Pool:** Full Classic pool
- **Draft:** Automated snake draft

#### Random 600 Association  
- **Format:** Limited player pool competition
- **Player Pool:** Random 600 players (minimum 40 catchers)
- **Era:** Moneyball Era (2005-2012) 
- **Draft:** Manual draft, 2-hour time limits
- **Special Rules:** Pool compliance, promotion/relegation
- **Details:** See `league_rules_random600.md` for complete specifications

---

## Player Pool & Ratings

### Classic Player Pool
The Classic player pool includes over 5,000 career-rated players:
- **MLB Players:** Complete major league history
- **Negro League Stars:** Over 100 legendary players  
- **NPB Players:** Japanese professional baseball stars
- **Inclusion Criteria:** Generally based on career plate appearances/innings pitched
- **Annual Updates:** New players added yearly

**Player Pool Selection Process (Official DMB):**
- Initial pool creation: Determined total number of players desired
- Position quotas: Divided players by position requirements
- Selection method: Generally chose players at each position with most career plate appearances or innings pitched to fill quotas
- New player additions: Based on salaries of most similar players in pool at time of addition

**Active Players:**
- **Not available in Classic mode:** Game projects performance based on entire careers; active players' careers are still ongoing
- **Available in SSG mode:** Single Season Game version includes active players with seasonal ratings
- **Historical note:** Experimented with adding "peaked" active players but no longer do so

### Player Rating Philosophy
Diamond Mind Baseball projects what players might accomplish if they played against each other across different eras and circumstances, rather than trying to replicate exact historical statistics.

#### Career vs Peak Performance
- **Under 6,000 PA (hitters) / 500 points (pitchers):** Full career performance
- **Over those thresholds:** Best consecutive seasons totaling ~6,000 PA or 500 points
- **Points System:** Pitchers earn 2 points per start, 1 point per relief appearance

#### Normalization Process
Player statistics are adjusted for fair cross-era comparison using sophisticated statistical analysis:

**Era Adjustment Methodology:**
- **Historical Baselines:** 1893 (60'6" pitching), 1920 (live ball), 1969 (mound lowered), 1990s (offensive explosion)
- **Index Calculation:** Player performance ÷ league average × 100 (where 100 = average)
- **Contextual Factors:** Ballpark effects, competition level, rule changes
- **Sample Size Requirements:** Minimum thresholds for statistical reliability

**Core Normalized Statistics:**
- **Batting:** BA+, OBP+, SLG+, OPS+, RC+, RC27+, HR+, BB+, K+, SB+
- **Pitching:** ERA+, ERC+, H+, BB+, K+, HR+, WHIP+, Component statistics
- **Fielding:** Range Factor+, Fielding Average+, Position-adjusted defensive metrics

**Advanced Statistical Framework:**
- **Runs Created (RC):** Multiple era-specific formulas (24 variations)
- **Component ERA (ERC):** Estimates expected ERA from component skills
- **Factor Statistics:** BBF (walk frequency), KF (strikeout frequency), HRF (power frequency)
- **Sabermetric Integration:** ISO (Isolated Power), SEC (Secondary Average), advanced metrics

**Quality Assurance:**
- **Cross-Reference Validation:** Multiple data sources for verification
- **Era-Specific Adjustments:** Context-sensitive normalization
- **Ongoing Refinement:** Continuous improvement of adjustment methodologies

#### Ratings Categories
**Offensive Ratings:**
- **Primary Metrics:** RC27+ (Runs Created per 27 outs), OPS+ (On-base + Slugging)
- **Component Analysis:** BA+, OBP+, SLG+ for detailed skill assessment
- **Power Evaluation:** ISO+ (Isolated Power), HR+ (Home Run frequency)
- **Plate Discipline:** BB+ (Walk rate), K+ (Strikeout rate)
- **Speed/Baserunning:** SB+ (Stolen base frequency), success rates
- **Advanced Metrics:** Factor statistics (BBF, KF, HRF) for skill vs. luck assessment

**Pitching Ratings:**
- **Primary Metrics:** ERA+ (Earned Run Average), ERC+ (Component ERA)
- **Skill Components:** K+ (Strikeout rate), BB+ (Walk rate), HR+ (Home run prevention)
- **Advanced Analysis:** H+, R+, OAVG+, OSLG+, OOBP+ (opposition statistics)
- **Durability Assessment:** Innings capacity, fatigue resistance
- **Situational Performance:** Clutch ratings, platoon effectiveness

**Defensive Ratings:** 
- **Range:** Ex/Vg/Av/Fr/Pr (Excellent to Poor)
- **Error Rate:** Percentage of league-average errors for position/era
- **Throwing Ability:** Arm strength ratings
- **Position Eligibility:** Based on career playing time

**Pitching Ratings:**
- **Endurance:** Pitch count limits before fatigue
- **Effectiveness:** Normalized performance vs. league
- **Role Designation:** SP (100+ career starts) or RP

**Starting Pitcher Designation Requirements (Official DMB):**
- **General Rule:** Pitcher must have started at least 100 games during career to be designated as starter (SP)
- **For Pitchers with 500+ Points:** Must have started at least 100 games during their best consecutive sequence of seasons comprising approximately 500 points
- **Points System:** 2 points per start, 1 point per relief appearance
- **Rationale:** Ensures only pitchers with significant starting experience receive SP designation

---

## Roster Management

### Active Roster (25 Players)
- **Participation:** All players eligible for games
- **Salary Impact:** Full salaries count against cap
- **Flexibility:** Can adjust composition throughout season
- **Requirements:** Must maintain minimum pitching staff

### Inactive Reserve (3 Players)
- **Purpose:** Injured players, strategic reserves, salary management
- **Game Eligibility:** Cannot participate in games
- **Injury Protection:** Players on IR cannot be injured
- **Fatigue Benefits:** Guaranteed rest for pitchers and catchers
- **Salary:** Full salaries still count against cap

### Position Requirements & Recommendations
**Mandatory:**
- 8 minimum pitchers (4 must be SP-designated)
- 1 catcher minimum

**Recommended Structure:**
- 17-18 position players, 10-11 pitchers
- 2-3 catchers (fatigue management)
- Utility players for positional flexibility
- Mix of left/right-handed batters
- Platoon advantage considerations

**Computer Manager Protection:**
- Will not pinch-hit for only available catcher
- Will not pinch-hit if no replacement available at position (before 9th inning)
- Respects minimum rest requirements for pitchers

---

## Drafting System

### Draft Structure
**Format:**
- Snake-style draft (1-12, 12-1, 1-12, etc.)
- 28 total rounds (25 active + 3 IR positions)
- Draft order randomly assigned

**Draft Types:**
- **Automated:** Complete roster submitted, draft runs instantly
- **Manual:** Pick-by-pick selection (Custom Leagues only)
- **Time Limits:** 1-24 hours or unlimited (manual drafts)

### Automated Draft Strategy
**Primary Picks:**
- List players in preferred selection order
- System drafts highest available player on your list
- If unavailable, moves pick to bottom and continues

**Alternate Picks:**
- Optional backup for each primary pick
- Must have lower salary than primary
- Provides insurance against popular players being taken
- **Official Guidance:** If two players at position are more or less equivalent and you definitely want one or the other, set one as alternate to the other. If you would rather improve chances of getting next player in draft list rather than alternate to player above him, don't list alternate.

**Draft Completion:**
- Skipped picks moved to end of list
- System assigns "most similar" player if needed
- **Similar = same position, handedness, highest available salary** (official definition)

**Draft Order Strategy:**
- **Official Guidance:** List players who are most difficult to replace highest in draft list
- Generally would be highest-salaried "star" players
- If lower-priced player is particularly outstanding value, might list him high as well
- At any given time, certain players are perceived as particularly good values and are very popular, hence generally can only be secured with high draft choice

### Manual Draft Features
**Advanced Options:**
- Real-time pick selection
- Draft matrix showing team compositions
- Pre-pick capability with "stops"
- Time limit enforcement
- Auto-assignment for missed picks (minimum salary player)

**Strategic Considerations:**
- Monitor other teams' positional needs
- Adjust strategy based on available players
- Balance between stars and depth
- Consider salary cap implications early

### Manual Draft Best Practices (Community Tips)
**Essential Setup:**
- **Stops on Early Picks:** Put stops on your first 3 picks initially (remove them once draft starts)
- **Update Button Required:** After reordering picks, must click "Update Pick Order" button - changes won't save otherwise
- **Alternate Preparation:** Have alternates for every player on your draft form (any pick can be taken before your turn)

**Draft Mechanics:**
- **Clock:** 3-hour time limit per pick (if preloaded pick available, auto-drafts immediately)
- **Stops Configuration:** Two types of stops:
  - Global setting: What happens when your pick is taken (move to next vs. pause for adjustment)
  - Individual stops: Force draft to pause on specific picks even if available
- **Auto-Draft Prevention:** Never let draft stop on you and then be absent (don't waste others' time)

**Draft Process:**
- Draft order set first, then draft typically pauses so everyone can review and adjust
- Can toggle between "Roster" page (same as autodraft prep) and "Manual Draft HQ" page (live draft control center)
- After draft, roster management works exactly like autodraft leagues (can drop/release normally)

**Manual Draft Official Details (DMB FAQ):**

**Pre-Picks and Stops:**
- In manual draft, you can list one or more "pre-picks" in advance of your turn to pick
- You can set "stop" to any player
- If you've set stop to player who is taken by another team before your turn to pick, instead of your next listed "pre-pick" player being drafted, draft will be halted so you can decide what to do next during time available to you to make your pick

**Time Limits:**
- Custom League creator must indicate whether there is time limit (anything from 1-24 hours or unlimited)
- Can specify whether during certain hours (typically overnight) time limit will be suspended
- If you fail to make pick within draft's time limit, you will be given minimum salary player at random and draft will move to next pick

**Draft Strategy in Manual Drafts:**
- Same considerations apply as automated draft (list difficult-to-replace players highest)
- **Additional advantage:** Opportunity to adjust roster strategy as draft progresses
- **Draft Matrix:** By watching Draft Matrix, can see what positions other teams have and have not filled at any given point
- **Strategic adjustment:** If you have not yet drafted catcher but most other teams have, it may be safe to wait longer to pick catcher than you otherwise might have, and turn attention to filling other positions sooner

### Post-Draft Period
**Hold Period:** 24-hour waiting period after draft completion
- No roster changes permitted
- Allows review of completed rosters
- Fair opportunity for all teams to plan

**Preseason Roster Changes:**
- Unlimited moves allowed after Hold Period
- 100% salary refund for released players (Standard Leagues)
- Custom Leagues may have different refund percentages
- Season begins the Monday following draft completion

**Official Clarification (DMB FAQ):**
- Once draft has been completed and league has been set up, there is 24-hour Hold Period during which no roster changes can be made
- At end of Hold Period, you may make changes to roster (subject to any league rules that may have been specially agreed restricting preseason moves)
- In Standard Leagues, before Opening Day you can make unlimited moves and be credited with 100% of released player's salary to use in signing replacement
- In Custom Leagues, this percentage can be set lower, although default setting is still 100%

---

## Team Management

### Required Setup Tasks
All team configuration must be completed before Opening Day:

**Lineup Configuration:**
- Set default lineups vs RHP and vs LHP
- Configure batting orders (1-9)
- Assign defensive positions
- Designate bench roles and substitutes

**Bullpen Assignments:**
- 5-man starting rotation
- Closer designation
- Setup men (left/right)
- Long relievers and spot starters
- Emergency roles

**Tactical Instructions:**
- Team-wide strategic tendencies (1-5 scale)
- Individual player instructions
- Situational preferences
- Computer Manager guidance

### Season Structure & Scheduling

#### Game Schedule
**Standard Leagues (3 games/day):**
- 162 games over 9 weeks
- 18 series of 9 games each
- Games at: Midnight, 9am, 3pm Pacific Time
- No games on Sundays

**Accelerated Leagues (9 games/day):**
- 162 games over 3 weeks
- Same series structure, faster pace
- Three game blocks per day

**Series Format:**
- All series are 9 games
- Home/away splits alternate
- Perfect 81-81 home/road balance

#### Weekly Structure
- **Week Duration:** 7 calendar days
- **Income Payment:** After last game of each week
- **Statistics:** Updated continuously
- **Standings:** Real-time throughout season

#### Postseason Timing
- **Division Championships:** Decided by regular season record
- **Wild Card:** Best non-division winner
- **Playoff Format:** Best-of-seven series
- **Home Field:** Based on regular season record
- **Schedule:** 2-3-2 format (Games 1-2 home to better record)

---

## Lineup & Position Rules

### Defensive Position Assignment
**Eligibility Requirements:**
- Players can only be assigned to positions where they have ratings
- Ratings shown as: Ex/Vg/Av/Fr/Pr + Error percentage
- Better ratings generally improve defensive performance

**Positional Flexibility:**
- Utility players can play multiple positions
- Quality varies based on player's experience at each position
- Consider defensive spectrum for emergency moves:
  SS → 2B → 3B → LF/RF → 1B → DH

### Defensive Value: Error Cost by Position
**Run Cost per Error (Community Research - HooverH/DvdAvins):**
Based on Flood Study analysis and database research, each error costs different amounts by position:

| Position | Runs/Error | Notes |
|----------|------------|-------|
| C | 0.79 | Errors often at plate (allow runs) or pick-offs (advance runners to 3rd) |
| 1B | 0.79 | Similar to catcher - errors in critical situations |
| 2B | 0.50 | Lower cost, but errors can prevent double plays |
| SS | 0.62 | Balance between middle infield and critical situations |
| 3B | 0.54 | Similar to 2B, often less critical than middle infield |
| LF | 0.88 | Outfield errors often allow extra bases or runs to score |
| CF | 1.13 | Highest cost - errors often result in extra bases or runs |
| RF | 0.63 | Variable - some calculations suggest higher (1.0+) |

#### Catcher Range Value: Exception to General Rule

**Source:** Community research (HooverH, chesswiz, knip, TylerEnsor, dandevine, baudib) - Forum discussion on catcher range value

**Key Finding: Catcher Range is Nearly Undetectable**

**Empirical Evidence:**

**HooverH's Testing (Home Game - 10 seasons, 9-team league, 160 games each):**
- Tested catcher range from Pr to Ex, holding all else constant
- **Results:** Random variation with no discernible trend
  - Ex: 1288 runs
  - Vg: 1294 runs
  - Av: 1281 runs
  - Fr: 1295 runs
  - Pr: 1288 runs
- **Conclusion:** Any catcher defense effect must be much less than variation in data (~10 runs per season)
- **Note:** Ex and Pr giving same result indicates effect is negligible compared to random variation

**HooverH's Daily Plays Analysis:**
- Analyzed catcher assists (GO to catcher) plus catcher outs (Pop-Outs) vs. Range
- **Finding:** Distribution of catcher Total Chances vs. Range looks **essentially identical** to total population
- No significant (>1%) increase in catcher Total Chances with Range
- **Calculation:** Catcher has ~100 Assists + Pop-Outs per season
- If range effect is ~1% per band, that's only **1 extra out per season per band**
- **Conclusion:** Effect is negligible

**knip's A/9 Analysis (Assists per 9 innings, accounting for CS):**
- Measured A/9 across catcher range ratings (Pr=1, Fr=2, Av=3, Vg=4, Ex=5)
- Filtered to catchers with ≥25,000 fielding innings to reduce noise
- **Finding:** ~0.05 difference in A/9 between Pr and Ex
- Each band separated by ~0.0125 A/9
- **Translation:** 1 additional Assist each 80 games per band
- **Conclusion:** "Virtually meaningless"

**chesswiz's Salary/Playoff Analysis:**
- Paired catchers similar in all respects except range (C Throw, Run, Injury, PB, error rating, BatPlat)
- Each pair had C Range difference of at least two levels
- **Salary Finding:** "Better" catchers cost ~$1/2 mil more than counterparts
- Indicates owners value a band of C Range at about **$1/4 mil**
- **Playoff Finding:** "Better" catchers made playoffs 23% of time vs. "worse" ones at 43%
- **Conclusion:** Even modest owner valuation ($1/4 mil per band) appears to be more than range is actually worth

**Community Consensus:**

**Relative Importance at Catcher:**
- **Errors:** More important than range (0.79 runs/error)
- **Throwing Arm:** More important than range (affects caught stealing, prevents advances)
- **Passed Balls:** More important than range (affects wild pitches, runner advancement)
- **Range:** Nearly undetectable effect (~1 out per season per band)

**Why Catcher Range Differs from Other Positions:**
- **TylerEnsor:** "Catcher is of course the exception, because its effect is nearly undetectable"
- **dandevine:** Questions why range wouldn't be worth same at all positions, but evidence shows it's not
- **chesswiz:** "In this sim it appears errors, throwing arm, and passed balls are each more important than range"

**Practical Valuation:**
- **baudib and knip:** Both rate catcher range at **2 runs per band** (though knip notes he can't remember how he came to that)
- **Reality:** Empirical evidence suggests even 2 runs per band may be overstating value
- **Market Evidence:** Owners paying ~$1/4 mil per band, but playoff success suggests this is overvaluation

**Strategic Implications:**
- **Priority Order for Catcher Defense:**
  1. **Error Rating:** Most important (0.79 runs/error, ~14 errors per 162 games in Standard Era)
  2. **Throwing Arm:** Critical for caught stealing and preventing advances
  3. **Passed Balls:** Important for preventing wild pitches and runner advancement
  4. **Range:** Nearly negligible (~1 out per season per band)
- **Draft Strategy:** Don't pay premium for catcher range - focus on error rating, throwing arm, and passed balls
- **Value Assessment:** If paying $1/4 mil per range band, you're likely overpaying based on playoff success data

**Key Insights:**
- **Outfield Errors More Costly:** CF errors cost nearly 2× more than middle infield errors
- **Position Context Matters:** Cost varies based on typical error situations
- **Range vs. Error Trade-off:** At 1B, Av/138 (better range) saves ~10 hits vs. Fr/30 (better errors) saving ~10 runs - generally close, with Av/138 slightly preferred (~2 runs better)
- **Practical Application:** Error rates over 110-115 at key positions (especially OF, middle IF) require serious advantages elsewhere to justify

#### Defensive Ratings: Range vs Error Trade-Off Analysis

**Source:** Community research (willibphx methodology) - See `docs/rules/defensive_ratings_range_error_tradeoff.md` for complete details

**Core Methodology (Standard Era):**
- **Each error = 0.6 runs** (Standard Era baseline)
- Error rating = percentage of league-average errors for position
- Lower error rating = fewer errors = more runs saved

**Standard Era Error Averages (per 162 games):**
- **SS: 29 errors** (league average)
- **2B: 22 errors**
- **3B: 24 errors**
- **1B: 14 errors**
- **OF: 8 errors each** (LF, CF, RF)

**Era-Specific Error Averages (per 162 games) - Official DMB Reference:**

| Era of Play | P | C | 1B | 2B | 3B | SS | LF | CF | RF |
|-------------|---|---|----|----|----|----|----|----|----|
| Standard (NL 1920-92) | 16 | 14 | 14 | 22 | 24 | 29 | 8 | 8 | 8 |
| Dead Ball (1903-1919) | 25 | 27 | 24 | 40 | 35 | 64 | 14 | 14 | 14 |
| Golden Age (1920-1941) | 17 | 17 | 27 | 30 | 27 | 48 | 11 | 11 | 11 |
| Baby Boomers (1946-1960) | 16 | 14 | 14 | 22 | 24 | 30 | 8 | 8 | 8 |
| Pitcher Era (1963-1968) | 16 | 14 | 14 | 21 | 24 | 30 | 8 | 8 | 8 |
| Turf Time (NL 1969-1992) | 14 | 14 | 12 | 17 | 24 | 25 | 8 | 8 | 8 |
| Home Run Derby (NL 1993-2004) | 12 | 11 | 11 | 12 | 21 | 21 | 6 | 6 | 6 |
| Moneyball (NL 2005-2012) | 17 | 14 | 8 | 11 | 17 | 16 | 4 | 4 | 4 |
| Statcast (NL 2013-2022) | 19 | 18 | 8 | 11 | 16 | 16 | 5 | 5 | 5 |
| 2024 AL Era | - | - | - | - | - | - | - | - | - |
| 2024 NL Era | - | - | - | - | - | - | - | - | - |

**Note:** Error rating represents percentage of league-average errors for position/era. Example: SS rated Vg/80 commits 80% of league-average errors for shortstops in that era.

**Note:** Error rating represents percentage of league-average errors for position/era. Example: SS rated Vg/80 commits 80% of league-average errors for shortstops in that era.

**Calculation Formula:**
```
Expected Errors = (Error Rating / 100) × League Average Errors
Runs Cost = Expected Errors × 0.6 runs
Runs Saved = (Errors Avoided) × 0.6 runs
```

**Critical Rule of Thumb: Range vs Error Trade-Off**

**For Infielders (2B, 3B, SS):**
- **~50 error points ≈ 1 range grade in value**
- Example: Vg/50 ≈ Ex/100 for shortstops
- Each 10-point error improvement ≈ 0.6 runs saved per season
- Range matters significantly (catches more balls)
- Error rating matters significantly (0.6 runs per error)
- **For same range: Lower error rating is always better**

**For Outfielders (LF, CF, RF):**
- **Range outweighs error rating**
- Errors less common (8 per 162 games)
- Range value more significant than error reduction
- Prioritize range upgrades over error rating improvements

**Application Guidelines:**

**When Evaluating Defensive Upgrades:**

1. **Same Range, Different Error:** Lower error rating is always better
   - Calculate: (Error difference / 100) × League Avg Errors × 0.6
   - Example: Ex/84 vs Ex/64 = 20 points = 5.8 errors = 3.5 runs saved

2. **Different Range, Different Error:** Use 50-point rule of thumb
   - 50 error points ≈ 1 range grade
   - Calculate error benefit first, then compare to range cost
   - Example: Vg/50 vs Ex/100 ≈ equal value

3. **Outfielders:** Prioritize range over error rating
   - Range more important due to lower error frequency
   - Error rating secondary consideration

**Position-Specific Considerations:**
- **SS, 2B, 3B:** Error rating very important (high error frequency: 22-29 errors/season)
- **1B:** Moderate error importance (14 errors/season)
- **OF:** Range more important than error (low error frequency: 8 errors/season)
- **C:** Error rating important (14 errors/season), range less variable

**Example Application:**
- **Question:** "Is Belanger (Ex/64) better than Smith (Ex/84) defensively?"
- **Analysis:** Both have Excellent range (same). Belanger has 64 error rating vs Smith's 84 (20 points better).
- **Calculation:** 20/100 × 29 × 0.6 = 3.5 runs saved per season
- **Conclusion:** Belanger is clearly better defensively (same range, better error rating)

**Note:** This methodology uses 0.6 runs/error as Standard Era baseline. Position-specific error costs (shown in table above) may vary slightly, but 0.6 runs/error provides reliable baseline for infielders in Standard Era calculations.

#### Defensive Diminishing Returns: "Too Much of a Good Thing?"

**Source:** Community research (IS_cwolfson, abywaters, DvdAvins, bokoskid, willibphx) - Message board discussion on stacking multiple Excellent defenders

**The Question:**
Does stacking multiple Excellent defenders cause diminishing returns? In other words, does each additional Ex fielder provide less value than the previous one because "there are only so many outs to go around"?

**The Answer:**
Yes, but the effect is **marginal and practically irrelevant** for most roster construction decisions.

**Mathematical Analysis (abywaters):**
- Each Ex fielder makes approximately 25-30 more plays per season than an Av fielder
- With ~5000 balls in play per season and ~6 K/9, each additional Ex fielder reduces opportunities for other fielders
- **Magnitude:** Each additional Ex fielder saves ~23.8 hits instead of 25 hits (0.2 plays per season reduction)
- **Total Effect:** All-Ex defense vs. individual Ex defenders = ~8.4 hits per season difference
- **Win Impact:** ~0.00391 wins per season (would take ~313 seasons to see 1 win difference)

**Empirical Testing Results:**

**bokoskid's Testing (10 seasons per scenario):**
- Added one Ex fielder at a time (C, then 1B, then 2B, etc.)
- Compared to individual position testing (only one position Ex, others Av)
- **Result:** Runs saved from adding all positions sequentially was very close to sum of individual position savings
- **Conclusion:** Diminishing returns exist but are minimal

**willibphx's Testing (200 seasons per scenario):**
- More comprehensive testing with larger sample size
- **Result:** After adding all 7 positions, runs saved was 177.5 vs. 192.6 (sum of individual values)
- **Difference:** ~8% loss in value or 15 runs per season
- **Conclusion:** Small but measurable diminishing returns

**HooverH's Testing:**
- Found diminishing returns more pronounced in lower offensive environments
- In high offense environment: ~90 runs less than sum of individual savings (~0.1 wins per team per season)
- In lower offense environment: Effect was larger both proportionally and absolutely

**Key Insight: Runs vs. Wins (DvdAvins Analysis):**
- **Runs Saved:** Diminishing returns exist (each Ex fielder saves slightly fewer runs)
- **Wins:** Diminishing returns are **microscopic** (percentage-based, not absolute)
- **Mechanism:** Each Ex fielder saves same percentage of remaining runs, so win impact remains approximately constant
- **Example:** Reducing runs from 750 to 730 (20 runs) = 2.19 wins. Reducing from 730 to 710.5 (19.5 runs) = 2.19 wins (same win impact despite fewer runs saved)

**Practical Implications:**

**For Most Leagues:**
- Diminishing returns are **not worth worrying about**
- Focus on **value per dollar**, not avoiding diminishing returns
- All-Ex defense is still better than mixed defense (just slightly less than linear improvement)

**For Elite Competition (GOT Finals, Ladder ML, Masters Tournament):**
- Effect is real but still marginal (~0.5 wins per season for all-Ex vs. individual Ex)
- May matter in extremely tight races, but rarely decisive
- Still better to have all Ex defenders than not, if cost is equal

**Strategic Considerations:**
- **Value per dollar matters more** than avoiding diminishing returns
- If defense is overpriced, don't stack Ex defenders regardless of diminishing returns
- If defense is underpriced, stack Ex defenders even with diminishing returns
- **Position matters:** SS, 2B, CF get more chances than corner OF, so diminishing returns less relevant for high-opportunity positions

**Mechanism Explanation:**
- Each defensive out ends a sequence of events
- When Ex fielder turns hit into out, there is one fewer subsequent batter
- Fewer batters = fewer balls in play = fewer opportunities for other fielders
- This is **structural**, not a flaw in the simulation
- Same phenomenon occurs in real baseball (not unique to DMB)

**Important Note:**
- This is about **range ratings specifically**, not fielding in general
- Error ratings operate independently
- Other fielding characteristics (arm strength, passed balls) not included in this analysis

#### Outfield Positioning: Corner-to-Corner Moves

**Source:** Community discussion (rcrny, IS_cwolfson, tonzmaniac, DvdAvins) - Moving players between LF and RF

**Moving Corner Outfielders (LF ↔ RF):**

**Range Rating:**
- **No loss of range** when moving corner outfielder to opposite corner
- LF-rated player in RF: Range stays same
- RF-rated player in LF: Range stays same

**Error Rating:**
- **Errors increase** when playing out-of-position at opposite corner
- All increased errors are **on throws** (not fielding errors)
- Example: LF-rated player with 0 errors in LF may make ~4 errors per 162 games in RF
- Example: Barry Bonds makes ~4 more errors per 162 games in RF than in LF

**Arm Strength:**
- **Arm strength value differs** between LF and RF
- **OF Throw worth ~2/3 as much in LF as in RF** (DvdAvins analysis)
- Playing Vg/Ex arm in LF wastes about 1/3 of arm value
- **Strategic Implication:** Vg/Ex throwers more valuable in RF than LF

**Strategic Considerations:**
- Moving LF-rated player to RF: Range stays same, errors increase on throws, arm value increases
- Moving RF-rated player to LF: Range stays same, errors increase on throws, arm value decreases
- **Net Effect:** Generally not worth moving solely for arm value unless significant difference
- **Exception:** If player has Ex arm and is rated in LF, may be worth moving to RF if RF-rated alternative is worse

**Center Field to Corner:**
- Centerfielder rated Ex with poor arm may be better value in LF than CF
- Range more important than arm in LF (arm less valuable there)
- Few such players exist (only 5 SSG seasons found, 3 by Devon White)

**Error Rating of 0:**
- Players with 0 error rating can still make errors (rare but possible)
- Example: 01 Luis Gonzalez (Vg/0 in LF) made 0 errors in 90,000 LF innings but 7 errors in 1,225 RF innings
- Example: 02 Honus Wagner (Vg/0 at 2B) made 0 errors in 57,000+ 2B innings
- **Strategic Note:** 0 error rating players are extremely rare and valuable for error prevention

**Positional Difficulty:**
- Reference section indicates RF is more difficult than LF
- Moving RF-rated player to LF: May have no penalty (easier position)
- Moving LF-rated player to RF: Errors increase (more difficult position)

### Lineup Construction Strategy
**Batting Order Principles:**
1. **Leadoff:** High OBP, speed, table-setter
2. **#2 Hitter:** Good contact, hit-and-run, move runners
3. **#3-4-5:** Power hitters, RBI production, heart of order
4. **#6-7:** Secondary power, setup for top of order
5. **#8-9:** Dependent on DH rules and roster construction

**Advanced Lineup Framework (Tyler Ensor Method):**
More detailed approach for optimal batting order construction:

1. **Top 3 Hitters:** Place your best 3 hitters in slots 1, 2, and 4
   - **Slot 1:** Prioritize OBP (table-setter, most plate appearances)
   - **Slot 2:** Balance of OBP and contact (move runners, set up heart)
   - **Slot 4:** Prioritize SLG (power, RBI production)

2. **#3 Slot:** Use your 4th or 5th best hitter who hits into fewer DPs
   - If both 4th/5th best hit into many DPs, look elsewhere for #3 hitter
   - Consider using one of your top 3 if they have low DP rate

3. **#5 Slot:** Place the other of your 4th/5th best hitters

4. **Remaining Slots (6-9):** Order from best to worst, with one exception:
   - If your worst hitter is considerably worse than your 2nd-worst (e.g., pitcher in NL), place worst hitter 8th instead of 9th

**Power Hitters at Leadoff (Special Consideration):**
- Power hitters with high OBP (Bobby Bonds, Rickey Henderson, Jimmy Wynn types) can efficiently clean up "table scraps" from bottom of order
- They provide dual value: setting up heart of lineup AND driving in bottom-half hitters
- Can be strategic advantage if roster construction supports it

**Additional Factors:**
- Speed (stealing ability, extra bases)
- Left/right-handedness (platoon optimization)
- GIDP rates (minimize double plays)
- Context-dependent adjustments

**Platoon Considerations:**
- Configure separate lineups vs LHP and vs RHP
- Maximize favorable matchups
- Consider bench depth for substitutions

---

## Pitching & Bullpen Management

### Starting Rotation Structure
**Requirements:**
- 5-man rotation standard (can designate 6th for flexibility)
- 4-day rest minimum between starts (3 days possible with endurance costs)
- Starting pitcher designation required (100+ career starts)

**Rotation Options:**
- **Ace Option:** #1 starter can pitch on 3 days rest, pushing others back
- **Spot Starter:** 5th starter only used with 4+ days rest
- **Emergency Starters:** Relievers can start but perform poorly
- **Rotation Skips:** Can skip 5th starter in favorable situations

**Ace Option Details (Official DMB FAQ):**
- **Who can handle Ace Option:** Any starting pitcher can pitch with four days rest without any loss of endurance
- **Three Days Rest:** With three days rest, number of pitches starter can throw before tiring will be affected by number of pitches he threw in last start
- **Endurance Requirements:** Only starters with greatest endurance can handle Ace Option without their ability to pitch deep into games being affected
- **Fatigue Impact:** Even greatest endurance pitchers will become fatigued earlier in game if they threw high number of pitches in previous start
- **Mechanism:** Because pitcher endurance is measured over moving five-day window, three-day rest creates overlap with previous start's pitch count

### Bullpen Role Assignments
Designate up to 3 pitchers per role (Computer Manager uses most appropriate):

**Low Leverage:**
- **Mop-Up:** Blowout situations, garbage time
- **Long Relief:** Multiple innings when starter removed early

**High Leverage:**  
- **Setup Men:** 7th-8th innings, critical situations
- **Left/Right Specialist:** Platoon matchup relief
- **Closer:** Save situations (3-run lead or less, 9th inning or later)

**Role Flexibility:**
- Computer Manager considers game situation over rigid roles
- Multiple closer designations create confusion - avoid
- Setup men used earlier than closers in leverage situations

### Fatigue & Endurance System

#### Pitcher Endurance Ratings
Endurance determines pitch count limits before effectiveness deteriorates:
- **Excellent (Ex):** Highest stamina for role
- **Very Good (Vg):** Above average endurance  
- **Average (Av):** Typical workload capacity
- **Fair (Fr):** Below average stamina
- **Poor (Pr):** Limited endurance

#### Standard Era Pitch Limits (Ideal Weather)

**Starting Pitchers:**
| Rating | One Game | Five-Day Window |
|--------|----------|-----------------|
| Ex     | 135-145  | 230-250        |
| Vg     | 125-135  | 210-230        |
| Av     | 120-130  | 200-220        |
| Fr     | 115-125  | 195-215        |
| Pr     | 105-115  | 180-200        |

**Relief Pitchers:**
| Rating | One Game | Five-Day Window |
|--------|----------|-----------------|
| Ex     | 65-75    | 90-105         |
| Vg     | 50-60    | 70-85          |
| Av     | 40-50    | 55-70          |
| Fr     | 35-45    | 50-65          |
| Pr     | 30-40    | 40-55          |

#### Era-Specific Adjustments
Pitch limits vary by era of play to reflect historical usage patterns:

**Dead Ball Era (1903-1919):** Higher limits, complete games common
**Golden Age (1920-1941):** High starter limits, developing relief roles  
**Modern Eras (1990s+):** Lower limits, specialized relief usage
**Current (2020s):** Lowest limits, maximum specialization

#### Weather Effects
- **Hot Weather:** Reduced endurance, earlier fatigue
- **Cold Weather:** Reduced endurance, grip issues  
- **Rain:** Shortened endurance, difficult conditions
- **Ideal Conditions:** Standard pitch count limits apply

**Weather Data Sources (Tom Tippett Interview):**
- **Primary Source:** Project Scoresheet/Retrosheet play-by-play files contain weather information for each game
- **Data Elements:** Temperature, wind speed and direction, precipitation, cloud cover, field condition
- **Historical Coverage:** Complete weather data from play-by-play records dating back to Project Scoresheet origins (1984)

**Retractable Roof Handling (Critical Detail):**
- **Problem:** When roof is closed, game account shows "no wind, 70 degrees inside" but doesn't reflect actual outdoor conditions
- **Solution:** System generates typical weather conditions for location and time of year FIRST, then decides if roof should be closed
- **Data Source:** External weather station data located near parks with retractable roofs
- **Database:** Custom database of weather data for roof-closed games merged with play-by-play file data

**Why This Matters:**
Without proper retractable roof handling, weather data would be seriously biased:
- **Toronto/Milwaukee:** Would only have data for nicest summer days, missing cold April nights
- **Arizona/Houston:** Would only have cooler spring/fall days, ignoring summer heat
- **Result:** Inaccurate weather modeling that doesn't reflect true seasonal conditions

**Impact on Simulation:**
- Weather affects pitcher fatigue (hot/cold/rain reduce endurance)
- Accurate weather data ensures realistic fatigue patterns throughout season
- Proper retractable roof handling maintains realistic weather distribution across all games

#### Moving Window System
Endurance tracked over rolling 5-day period with separate limits:
- Today only
- Today + yesterday  
- Today + last 2 days
- Today + last 3 days
- Today + last 4 days (full 5-day window)

Recent usage has greater impact than older work within the window.

### Catcher Fatigue
**Fatigue Accumulation:**
- Based on total batters faced over moving 10-day window
- Extra-inning games and high-offense games increase fatigue faster
- All aspects of performance deteriorate when fatigued

**Computer Manager Protection:**
- Will not start fatigued catcher if backup available
- Projects potential fatigue before upcoming game
- Prevents pushing catcher beyond safe threshold

**Rest Benefits:**
- Playing other positions (1B, DH) counts as rest
---

## Tactical Settings

### Team Instructions Philosophy
All tactics use a 1-5 scale where:
- **1 = Most Aggressive:** Maximum frequency/risk-taking
- **3 = Neutral:** Historical MLB average behavior  
- **5 = Most Conservative:** Minimum frequency/risk-averse

**Default Behavior (Official DMB FAQ):**
- **What happens if I leave my team and player instructions unchanged?**
- DMB has studied play-by-play data carefully to ascertain frequency with which different tactics have been used in major leagues in different game situations
- If you leave team and player tendency settings unchanged, decisions of Computer Manager will follow patterns found in that data
- **Realistic variation:** Managers will not necessarily do same thing every time. In given situation, for example, they might have particular player sacrifice bunt two-thirds of time and swing away one-third of time
- **Adjusting settings:** If you adjust bunt tendencies for your team or that player, it will alter this ratio accordingly

### Team-Wide Offensive Settings

**Bunting (1-5):**
- Controls sacrifice frequency in appropriate situations
- Affects all players unless individually overridden
- Setting 1: Bunt at every reasonable opportunity
- Setting 5: Rarely sacrifice, swing away mentality

**Important Bunting Warning (Community Insight):**
The Computer Manager has a "1960s/70s manager mentality" regarding bunting - it likes to bunt early in games even at Setting 5. Setting 5 does NOT mean "only bunt late in close games" as many expect. 

**Recommended Approach:**
- Set Team Bunt setting to "Never" (6) to prevent early-game bunting with good hitters
- Then individually set pitchers and weak hitters to 5 if you want occasional sacrifice situations
- This prevents quality hitters from bunting in early innings while still allowing strategic bunts from appropriate players

**Base Running (1-5):**
- Aggressiveness taking extra bases
- Stealing frequency (combined with individual settings)
- Risk tolerance on close plays

**Hit-and-Run (1-5):**
- Frequency of hit-and-run attempts
- Situational usage with runners on base

**Taking Pitches (1-5):**
- Patience at the plate, working counts
- Drawing walks vs. aggressive hitting

### Team-Wide Pitching Settings

**Using Closer (1-5):**
- How readily closer is brought into games
- Setting 1: Use closer aggressively, earlier in games
- Setting 5: Save closer for traditional save situations only

**Intentional Walks (1-5):**
- Frequency of strategic free passes
- Willingness to pitch around dangerous hitters

**Pitch-Around (1-5):**
- Avoiding the strike zone with tough hitters
- Nibbling strategy vs. challenging hitters

**Pitchouts (1-5):**
- Frequency of stealing prevention attempts
- Balance between catching runners and getting behind in count

### Individual Player Overrides

**Available Settings:**
- Same 1-5 scale as team settings
- **6 = Never:** Absolute prohibition of tactic
- Overrides team setting for specific player

**Strategic Applications:**
- **Best Base Stealers:** May not need aggressive setting (already steal optimally)
- **Poor Bunters:** Set to 5 or 6 to prevent failed attempts
- **Clutch Situations:** Star players on conservative pinch-hitting settings
- **Pitching Specialists:** Closer settings to control usage patterns

**Pull for Pinch Hitter/Closer:**
- Controls Computer Manager's willingness to remove player
- Critical for protecting stars and managing pitcher usage
- Setting 6 prevents removal in most situations

---

## Transactions & Trading

### Trading System

**Trade Requirements:**
- Must exchange equal numbers of players
- **Trade Deficit Limit:** 10% maximum value difference (Standard Leagues)
- Cash can be included to balance deals
- Both teams must stay under salary cap

**Trade Deficit Calculation:**
```
Example: $1M player for $1.5M player fails (66.7% < 90% threshold)
Solution: $1M player + $400K cash = $1.4M (93.3% ≥ 90%)
```

**Trade Timing:**
- Available throughout season until deadline
- **Deadline:** Monday 3am PT before Week 7 starts
- No trades during playoffs

**Strategic Trading:**
- Split release penalties between teams
- Target players other teams want to release
- Cash considerations to improve value
- Salary cap management tool

**Official Trading Guidance (DMB FAQ):**

**Why Trade Instead of Free Agency?**
- If you release a player, you only get back percentage of salary league rules specify (75% in Standard Leagues)
- If you identify another team that might be interested in player you plan to release, you can offer trade that splits "haircut" on releasing player (25% in Standard Leagues) between two teams
- **Benefit to you:** Get more for player than if just released him
- **Benefit to trading partner:** Gets player for less than his full salary if signed as free agent
- **Alternative:** Propose to team owner who has expressed dissatisfaction with player who interests you that you will "take him off their hands" (for more than other owner would get if he released player, but less to you than his full salary)

**Best Way to Initiate Trades:**
1. **Most effective:** Find team in league that is good match for what you'd like to accomplish - player(s) and/or cash you're offering fills need they have, while they can spare player(s) and/or cash you want - and make them offer
2. **Next best:** "Shop" particular player by inviting offers for him by email or on league message board
3. **Least effective:** Simply declaring that you're open to all trade offers

**Key Principle:**
- Must look at trade from point of view of other team
- No matter how fair exchange might be in abstract, if what you propose doesn't at least appear like it might improve other team as well, other owner is unlikely to accept your offer

### Free Agency

**Player Availability:**
- Released players become free agents
- Available to all teams under salary cap
- **Signing Deadline:** Monday 3am PT before Week 9

**Salary Recovery:**
- **Standard Leagues:** 75% salary recovery on releases
- **Custom Leagues:** Commissioner sets percentage
- **Preseason:** 100% recovery before Opening Day

**Strategic Considerations:**
- Monitor other teams' potential releases
- Bookmark players of interest
- Email notifications for bookmarked player status
- Timing releases to maximize recovery value

### Loan System

**Loan Mechanics:**
- Cash advances against future income
- Interest calculated on outstanding balance  
- Cannot prepay loans to recover interest
- Loan amount + interest deducted from future payments

**Interest Calculation (Official DMB FAQ):**

**Standard Leagues (3 games per day):**
- Interest calculated daily on weekly rate (basis of seven day week)
- **Payment timing:** Day "ends" at time each sim run is scheduled (midnight, 9am, 3pm PT)
- **Timing optimization:** If you take out loan at 3:01 PM PT, you pay one less day's interest than if taken at 2:59 PM PT
- **Practical application:** Can take out loan to acquire player in interval between start of sim run and team's games being simmed, and not pay interest for that first day/series

**Accelerated Leagues (9 games per day):**
- **Preseason and Sundays:** Each "real" day counts as one "day" when computing interest over "virtual week"
- **Regular season:** Each "real" day counts as three "days" when computing interest over "virtual week"
- Each three-game series occurs on single "day" in this computation
- **Day ends:** At time each sim run is scheduled (midnight, 9am, 3pm PT)
- **Peculiarity:** In 3 games/day leagues, first day of every week is Sunday. In 9 games/day leagues, only "weeks" 4 and 7 begin with Sunday
- **Effect:** In accelerated leagues, "weekly" interest rate applicable to weeks 2, 3, 5 and 6 actually is just 6/7 of league's weekly rate

**Interest on Positive Balances:**
- **Payment time:** Calculated and paid daily at **noon PT** (12:00 PM)
- **Crediting order:** Interest credited in leagues in order of oldest to newest (may be brief interval after noon before interest credited)
- **Calculation method:** Interest on positive bank balances is calculated and paid daily on weekly interest rate (basis of seven day week)
- **Compounding:** Each successive day's interest payment during week will be slightly higher (since balance on which interest is calculated has increased by prior day's interest payment)
- **Effective rate:** League's interest rate payable on positive account balances is effective weekly rate, so interest paid on first day of week will be slightly less than 1/7th of weekly rate on opening balance, to allow for compounding
- **Formula (for mathematically minded):** Daily rate = $b × [1.r^(1/7)-1] = i, where b = account balance, r = league's interest rate, and i = daily interest payable

**Preseason Interest:**
- **Positive balances:** Do not begin to earn interest until season begins
- **Loans:** Do pay interest on loans taken out during preseason for each day prior to Opening Day
- **Strategy:** If taking out loan in preseason to sign player, pay less interest if wait until right before season starts than if take out loan right after Hold Period expires (though may risk someone else signing player you want)

**Maximum Loan Strategy:**
- Taking out loan before Opening Day to bolster team is not necessarily bad strategy
- Interest on loan partly offset by fact that (in most leagues) you do not lose part of salary of player dropped in preseason, as you would after season has begun
- **Warning:** Borrowing maximum amount possible may not be good idea
- **Reason:** Because of effect of compound interest, in Standard League maximum amount you can borrow in preseason is only about half total amount of scheduled weekly payments (not to mention amount of interest foregone had you allowed some of those payments to accumulate)
- **Long-term cost:** Long-term cost to value of roster probably would outweigh short-term advantage gained
- **Exception:** Could be Custom Leagues with unusual rules in which this might not be case
- **Reality:** Doesn't mean you can't win if borrow to max (many have), but you are taking a risk

---

## Financial System

### Starting Budget & Income

**Classic Standard Leagues:**
- $100 million salary cap
- Weekly income throughout season
- Income timing: After last game of each week

**Official Clarification (DMB FAQ):**
- **When is weekly income paid?** Income is added to team's bank account after the last game of the week has run

---

## Playoffs & Postseason

### Playoff Structure

**Qualification:**
- 3 Division winners (best record in each division)
- 1 Wild Card (best record among non-division winners)
- Tiebreaker: Head-to-head record, then run differential

**Series Format:**
- **League Championship Series (LCS):** Division winners vs. Wild Card
- **World Series:** LCS winners face off
- **All Series:** Best-of-seven format

**Home Field Advantage:**
- Awarded to team with better regular season record
- **Format:** 2-3-2 (Games 1-2 at home for better record)
- Games 3-4-5 at other team's park
- Games 6-7 back to better record's park if necessary

### Playoff Roster Rules

**Active Roster Changes:**
- **Between Regular Season & LCS:** Roster changes allowed
- **Between LCS & World Series:** Roster changes allowed  
- **During Series:** NO changes allowed once series begins
- **Injury Exception:** Can activate IR players between series only

**Strategic Considerations:**
- Plan roster for playoff matchups
- Consider starting rotation alignment
- Bullpen depth for short series
- Bench players for specific situations

---

## Ballparks & Eras

### Ballpark System

**Park Selection:**
- Choose home ballpark during team creation
- Affects all 81 home games throughout season
- Cannot be changed after selection

**Park Factors:**
Park factors show relative impact compared to league average (100 = neutral):
- **Above 100:** Favors that statistic (more likely)
- **Below 100:** Suppresses that statistic (less likely)
- **Separate factors:** Left-handed vs. right-handed batters

**Critical Understanding:**
- **No Absolute Factors:** Park factors are ALWAYS relative to their era and league context
- **Era Dependency:** A HR factor of 227 in Dead Ball Era ≠ 227 in Home Run Era
- **Dynamic Adjustment:** Game engine automatically adjusts factors when parks cross eras
- **Context Matters:** Same park can have vastly different impacts in different offensive environments

**Factor Categories:**
- **1B (Singles):** Base hit frequency
- **2B (Doubles):** Extra-base hit rate  
- **3B (Triples):** Three-base hits
- **HR (Home Runs):** Long ball frequency

**Physical Determinants:**
Park factors reflect multiple physical characteristics:
- **Field Dimensions:** Foul line distances, gaps, center field depth
- **Fence Configuration:** Heights and shapes throughout outfield
- **Playing Surface:** Grass vs. artificial turf (affects ball speed/bounces)
- **Environmental Factors:** Elevation, wind patterns, weather effects
- **Foul Territory:** Size affects batting averages (more territory = more foul outs)
- **Hitting Background:** Visual factors for batters seeing pitches

**Mathematical Reality:**
Park factor calculation example (2003 Fenway):
- Home games: 156 HRs in 5,010 ABs = .0311 HR/AB
- Away games: 172 HRs in 5,094 ABs = .0338 HR/AB  
- Park Factor: .0311 ÷ .0338 = 92 (8% harder to hit HRs at Fenway)

**Park Strategy:**
- **Hitter-Friendly Parks:** Higher offensive numbers, favor balanced offense
- **Pitcher Parks:** Lower offensive numbers, emphasize pitching/defense
- **Extreme Parks:** Build roster to maximize advantages (Fenway = RH power)
- **Factor-Based Building:** Performance impact based on factors, NOT raw dimensions

### Era of Play

**Era Selection:**
Determines baseline statistical environment for league:

**Historical Eras:**
- **Dead Ball (1903-1919):** Low offense, more errors, few strikeouts
- **Golden Age (1920-1941):** High batting averages, moderate offense
- **Baby Boomers (1946-1960):** More walks and home runs
- **Pitcher Era (1963-1968):** Extremely low offense, pitching dominant
- **Turf Time (1969-1992):** Artificial turf effects, moderate offense
- **Home Run Derby (1993-2004):** Offensive explosion, high power numbers
- **Moneyball (2005-2012):** More strikeouts, declining offense
- **Statcast (2013-2022):** Maximum strikeouts, launch angle revolution

**Modern Eras:**
- **AL 2022/NL 2022:** Most recent completed seasons
- **Standard Era (1920-1992):** Neutral baseline for Classic leagues

**Era Effects:**
- Determines league-wide offensive levels
- Affects pitcher endurance limits
- Influences tactical decision frequency
- Adjusts park factor impacts

**Dynamic Park Factor Adjustment:**
When parks are used in different eras than their rating period:
- **Automatic Scaling:** Game engine recognizes era mismatches and adjusts factors
- **Spread Adjustment:** Factor ranges compress/expand based on era offensive levels
- **Example Logic:** Baker Bowl (1915-19) HR factor of 227 would reduce to ~150 in modern era
- **Reverse Example:** Modern park with HR factor 130 might increase to 175 in Dead Ball Era
- **Rare Event Scaling:** When events are rare (HRs in Dead Ball, triples today), factors have wider spreads

---

## Computer Manager

### Decision-Making Philosophy
The Computer Manager plays percentages, not hunches:
- Ignores hot/cold streaks
- Focuses on matchup advantages
- Considers platoon splits
- Uses historical frequency data for tactical decisions

**Official Clarifications (DMB FAQ):**

**Hot/Cold Streaks:**
- Computer Manager does not look at whether a player has been "hot" or is having a good game or season
- Decisions based on who provides best match-up against opposing pitcher
- Particularly in last inning when team is behind, will "pull out all the stops" to score tying run
- May pinch-hit for middle-of-order hitter in extreme circumstances to gain platoon advantage

**Bunting Behavior:**
- Managers must keep defenses honest
- Unless batter's instruction set to "Never," there's always possibility (however remote) that Computer Manager will have him bunt
- Because we simulate so many games, unusual plays and tactics occur daily
- When team owners share experiences, it may create appearance these plays occur frequently, when relative to total games simulated, they actually are quite uncommon

**Best Base Stealer Settings:**
- On neutral setting, top base stealer will steal at virtually every opportunity that's appropriate
- Setting individual instruction to steal more frequently makes him even more aggressive, taking risks you may not want
- If you lower team stealing instruction, effect is to keep weaker runners (but not best base stealers) from going

### Substitution Logic

**Pinch Hitting:**
- Will use pinch hitters for platoon advantage
- Considers late-game leverage situations
- May pinch hit for star players in critical spots
- Respects individual "Pull for PH" settings
- **Protection:** Will not PH if no defensive replacement available

**Official Clarifications (DMB FAQ):**

**Pinch Hitting Protection:**
- Will not pinch hit for a player if there is no other player on active roster rated at his position **before the 9th inning**
- In 9th inning, may pinch hit even without replacement if team is behind
- **Catcher Exception:** Computer Manager will **never** pinch hit for catcher if there isn't another player rated to catch available to replace him

**Closer Settings:**
- **Pull for Closer applies to closer himself:** If you set team "Using Closer" to 1, individual "Pull for Closer" for each pitcher (including closer) also changes to 1
- **Effect:** If closer's Pull for Closer setting is 1, Computer Manager will give him quicker hook and look to another pitcher to step in to close out game
- **Strategy:** To go aggressively to closer and stick with him: Set team "Using Closer" to 1 or 2, but closer's "Pull for Closer" setting to 4 or 5

**Multiple Closers (vs L/R):**
- If you list closer vs L and closer vs R, Computer Manager may bring in other closer even against platoon advantage if first batter reaches base
- **Reason:** Computer Manager gives greater weight to fact it is closer situation than to platoon match-ups
- **Solution:** Increase "Pull for Closer" setting for each closer to 4 or 5, or use single pitcher specified to close

**Bullpen Role Priority:**
- When you list more than one set-up man or pinch hitter, Computer Manager doesn't just use them in order listed
- Assumes you want player listed first used in most important situations
- Will use second listed set-up man earlier in game, saving first listed for later when game is on the line
- Computer Manager may also look to no. 2 set-up man when it needs Long Reliever (if none of listed Long Relievers are rested and available)
- **Warning:** Listing closer as no. 2 set-up man may cause Computer Manager to use closer in non-save situations, even in blow-outs

**Pitching Changes:**
- Follows bullpen role assignments
- Considers pitcher fatigue and effectiveness
- Prioritizes matchups in close games
- Uses "Using Closer" and "Pull for Closer" settings

**Defensive Substitutions:**
- Late-inning defensive upgrades
- Double-switch opportunities
- Injury replacements
- Emergency position coverage

### Strategic Behavior

**Tactical Execution:**
- Team settings determine base frequencies
- Individual overrides modify behavior
- Situational context influences decisions
- Maintains realistic MLB usage patterns

**Role Management:**
- Uses primary role assignments first
- Falls back to secondary roles when needed
- Considers pitcher rest and availability
- Balances workload across staff

**Emergency Situations:**
- Handles unexpected injuries
- Manages position player pitching if needed
- Protects essential players (catchers, etc.)
- Maintains legal lineups at all times

---

## Frequently Asked Questions

### Player Pool and Salaries

**Q: How did you decide which players to include in the Classic (career-rated) player pool?**
A: Initial pool creation determined total number of players desired, divided by position quotas, then generally chose players at each position with most career plate appearances or innings pitched to fill quotas.

**Q: Do you ever add new players?**
A: Yes, new players are added to the Classic player pool annually.

**Q: Why are there no active players available?**
A: Game projects performance based on players' entire careers. Active players' careers are still ongoing. Active players are available in Single Season Game (SSG) version. Experimented with adding "peaked" active players but no longer do so.

**Q: How are the salaries for players determined?**
A: Salaries initially were based on a formula that assigned a single number encompassing all aspects of player performance. Salaries for players added periodically are based on salaries at the time of players most similar to them in the player pool.

**Q: How often do salaries change?**
A: Salaries for Classic players are adjusted three times annually, based on how frequently players have been used at their present salaries compared to similar players. SSG player salaries are adjusted annually.

### Player Performance

**Q: Why aren't my players performing like their real-life stats?**
A: DMB simulates players in different contexts than their careers. Performance is normalized for era, park, and competition level. Your league's environment may differ significantly from their historical context. Additionally, players will play many times more seasons in DMB than their actual careers, so extreme seasons (both good and bad) are likely to occur.

**Q: Do I get a specific season from a player's career?**
A: No. Every team gets the same "player" - a normalized composite based on their career/peak performance. Individual seasons will vary due to baseball randomness.

**Q: Will a poorly performing player eventually improve?**
A: There's no "rubber band" effect. Good players generally produce expected results over time, but baseball includes natural variation and slumps. Over course of a season, things may "even out" for a player, but there is nothing in the game simulator that adjusts a player's chances of success based on his past performance during that season.

**Q: Could the reason that one of my players is doing poorly be that I got a bad season from his actual career?**
A: No. Every team that uses a particular player gets the "same" player.

**Q: What minimum number of at bats, innings pitched, or innings in the field should a player have before I can rely on their "sim stats"?**
A: There is no definite answer, but the number probably is much larger than you think. Different patterns of usage can distort statistical comparisons even over thousands of AB, IP or innings in the field. For example, some players have been used relatively much more than others in extreme parks like Baker Bowl and Coors Field. Pitchers have been seen to under or overperform their projected ERA over stretches longer than 10,000 IP, and fielders exceed their projected RF over stretches longer than 25,000 innings in the field, by 0.25 or more.

**Q: How can you tell who are the best "clutch" players?**
A: Some players appear to have been "clutch" performers for particular seasons, but baseball analysts have searched for evidence of "clutch" ability over players' careers without success. They have been unable to quantify it, or even prove that it exists. For this reason, **no players in Classic mode have been given special "clutch" ability**. All that can be said is that the better the player, the more likely it is that he will succeed in "clutch" situations. Some players are, however, rated to do better in clutch situations in the Single Season Game (SSG) version.

### Roster Management

**Q: How many pitchers should I carry?**
A: Minimum 8 required (4 SP), but contemporary management needs more. Consider 10-11 pitchers for proper bullpen depth and fatigue management.

**Q: Can I change my lineup after the draft?**
A: Yes, unlimited changes during the Hold Period and throughout the season. Only restriction is during active playoff series.

**Q: What's the best spending balance between hitting and pitching?**
A: No single formula works. Successful approaches range from balanced (70% hitting/30% pitching) to extreme specialization. Match your strategy to your park.

### Game Mechanics

**Q: Does fatigue affect non-pitcher performance?**
A: No evidence exists for general player fatigue effects. Only catchers and pitchers have fatigue systems that impact performance.

**Q: Do players have "clutch" ability?**
A: Classic leagues have no clutch ratings - better players simply perform better in all situations. Some SSG players have clutch ratings for specific seasons.

**Q: Can I expect my injury-prone player to miss the same games as real life?**
A: No. Injury ratings indicate relative risk, but DMB doesn't project specific injury totals. Variance is normal and expected.

### Strategy & Tactics

**Q: Should I set my best base stealer to steal more frequently?**
A: Usually no. Elite base stealers already attempt steals at optimal opportunities. Aggressive settings may force poor attempts.

**Q: Why did the Computer Manager bunt with my slugger?**
A: Computer Manager maintains realistic tactical variety. Even stars occasionally bunt to keep defenses honest, though it's rare.

**Q: How do I prevent the Computer Manager from pinch hitting my stars?**
A: Set individual "Pull for PH" setting to 5 or 6. Setting 6 prevents removal except in extreme circumstances.

### League Management

**Q: Can I change Custom League settings after creation?**
A: No. Settings are locked once created. Must cancel and recreate league with new settings if changes needed. If you cancel league, owners with teams in league will have team credit added back to their accounts when league is cancelled. When you cancel league, system gives you option to send message to league's team owners, so you can explain that you'll be recreating league with different settings.

**Q: Can I clear my team draft form without having to delete each player one by one?**
A: Yes, by clicking Clear Draft button at bottom of Draft Team page. So that you don't accidentally wipe out team's draft, you have to confirm second time that you want to clear draft form after you click Clear Draft button. You also have option of populating team's draft form with rosters of your past teams.

**Q: How many pitchers should I carry on my roster?**
A: We don't want to tell you how you must run your team, which is why we only require that you draft 8 pitchers, including 4 with starter designation. Throughout much of baseball history, teams carried fewer pitchers than they do today. However, that was before general adoption of 5-man starting rotations, dedicated closers, and specialist setup men. If you want Computer Manager to manage your pitching staff in contemporary fashion, you have to give it enough arms to do that, or you'll find it making decisions you may not like that are dictated by pitchers being fatigued.

**Q: What is the best proportion of spending between hitting and pitching?**
A: There isn't a single best approach. Balanced teams, hitting-oriented teams and pitching-oriented teams all can succeed, provided players chosen complement each other and your home park.

**Q: I've created a Custom League with a Random Manual Draft Player Pool. How can I check who is in the pool? Can I change the pool?**
A: When you create Custom League with Random Manual Draft Player Pool, you can check who is in pool by going to Draft Team page and searching hitters and pitchers. If you are unhappy with composition of pool (e.g., not enough catchers), if you click on Commissioner Options under League Name on My Teams page, you can click on Re-Shuffle the Pool to create new pool for league. You cannot add or remove specific players from random player pool. Also, you cannot re-shuffle pool once any other team has joined league.

**Q: In the manual draft for a random or by seasons limited player pool, minimum salary players were added to the player pool during the course of the draft. Why did this happen? How can owners who drafted these players be forced to drop them?**
A: During course of manual draft using limited player pool, to ensure drafts do not "crash" if owner has insufficient funds to complete their draft from players remaining in pool, minimum salary starting pitcher or position player will be added to player pool automatically each time last player of that type is drafted. These players will be labeled. After draft concludes, those players who have not been drafted or who are released will be "hidden" and unavailable to other teams to draft as free agents. There is no mechanism to force owners to drop players they have drafted. It may be advisable to specify this requirement as league rule when setting up limited player pool Custom League. One of challenges of limited player pool leagues is difficulty building competitive roster under salary cap due to shortage of lower-salaried players. Replacing players with eligible players after draft has concluded may require owner to replace player they drafted with less expensive player and/or to take out loan.

**Q: How does the random draft player pool work?**
A: System randomly selects subset of total player pool. Can re-shuffle before other teams join. May auto-add minimum salary players during draft to prevent crashes.

**Q: What happens if I miss my manual draft pick?**
A: You receive a random minimum salary player and draft continues. This applies only to leagues with time limits.

---

## Conclusion

Diamond Mind Baseball offers the deepest and most realistic baseball simulation available, combining historical accuracy with strategic depth. Success comes from understanding both baseball fundamentals and the specific mechanics of the DMB system.

Key principles for success:
- Build rosters that complement your ballpark
- Balance salary allocation across positions
- Understand Computer Manager behavior
- Use tactical settings strategically
- Manage fatigue and injuries proactively
- Think long-term about roster construction

The most important factor is to have fun while competing with fellow baseball enthusiasts from around the world.

---

*This document represents the unified rules for Diamond Mind Baseball based on official sources from Imagine Sports. For league-specific modifications, consult your league's supplementary rules documentation.*

### Releases
- **Pre-Season:** 100% salary refund
- **In-Season:** 75% salary refund
- **Strategic Use:** Clear roster space, manage salary

### Loans & Banking
- **Weekly Income:** 5% daily interest on positive balances
- **Loan Availability:** Based on future income potential
- **Interest Rate:** 15% per week on borrowed amounts
- **Strategic Timing:** Minimize interest costs

---

## Season Structure

### Regular Season
- **Length:** 162 games over 9 weeks
- **Schedule:** 3 games/day (M-Sa), none on Sunday
- **Game Times:** Approximately 3am, noon, 6pm ET
- **Weather Effects:** Enabled by default

### Key Dates
- **Draft:** Variable based on league creation
- **Opening Day:** Monday following draft completion
- **Trade Deadline:** Week 7, Monday 3am ET
- **Free Agency Deadline:** Week 9, Monday 3am ET
- **Regular Season End:** Week 9, Saturday

---

## Playoffs & Postseason

### Playoff Format
#### Standard Leagues (12 teams)
- **Division Winners:** 3 automatic qualifiers
- **Wild Card:** Best second-place team
- **Rounds:** 2 (LCS, World Series)
- **Series Length:** Best-of-seven
- **Home Field:** 2-3-2 format

#### Custom Leagues
- **Structure:** Varies by team count
- **2 Teams:** One-game playoff if tied
- **4-6 Teams:** Best-of-seven championship
- **8+ Teams:** Multiple rounds

### Playoff Rules
- **Roster Lock:** Set before Game 1, changes only between series
- **Fatigue Carryover:** Pitcher rest and injury status continues
- **Rest Requirements:** Pitchers need 3+ days between starts
- **Home Field Advantage:** Based on regular season record

### Tie-Breakers
1. Head-to-head record
2. Run differential (Wild Card) / Division record (Division)
3. Division record (Wild Card) / Run differential (Division)
4. Coin toss

---

## Fatigue & Injury System

### Pitcher Fatigue
- **Tracking Period:** 5-day rolling window
- **Endurance Thresholds:** Vary by rating and era
- **Weather Effects:** Temperature and humidity impact
- **Performance Decline:** Effectiveness drops past threshold

#### Endurance Ratings (Standard Era)
| Rating | SP One Game | SP Five Days | RP One Game | RP Five Days |
|--------|-------------|--------------|-------------|--------------|
| Ex     | 135-145     | 230-250      | 65-75       | 90-105       |
| Vg     | 125-135     | 210-230      | 50-60       | 70-85        |
| Av     | 120-130     | 200-220      | 40-50       | 55-70        |
| Fr     | 115-125     | 195-215      | 35-45       | 50-65        |
| Pr     | 105-115     | 180-200      | 30-40       | 40-55        |

### Catcher Fatigue
- **Tracking Period:** 10-day rolling window
- **Measurement:** Batters faced while catching
- **Performance Impact:** Effectiveness declines with fatigue
- **Rest Method:** Playing other positions provides partial rest

**Official Clarifications (DMB FAQ):**

**Fatigue Effects:**
- When catchers become fatigued, **all aspects of their performance begin to suffer**
- The more fatigued they become, the more their performance deteriorates
- Catcher fatigue is based on total batters faced during moving 10-day window
- Extra-inning games or slugfests (unusually high number of batters to plate) have greater impact on catcher

**Computer Manager Protection:**
- Will not start a catcher if there is healthy backup available and scheduled starter could be pushed beyond his fatigue threshold in upcoming game
- Projects potential fatigue before upcoming game to prevent pushing catcher beyond safe threshold

**Fatigue Susceptibility:**
- **No individual differences:** All catchers have same fatigue susceptibility
- Number of games catcher played from season to season during actual career is at least as likely indication of ability as durability
- No reason in most cases to think catcher would not have been capable of playing as much as next guy had he been given opportunity
- Fatigue threshold set so catchers should (injuries aside) be able to catch approximately **85% of team's games** during season

**Rest Methods:**
- **Yes, counts as rest:** Playing other positions (like 1B) or being DH counts as "rest" in determining fatigue
- Playing other positions provides partial rest from catching duties

### Injury System
- **Frequency:** Somewhat less than real-life rates
- **Risk Factors:** Based on historical injury patterns
- **Position Risk:** Catchers and certain situations higher risk
- **DH Benefit:** Reduced injury risk for designated hitters
- **Recovery:** Players must complete full injury duration

**Official Clarifications (DMB FAQ):**

**Injury Propensity:**
- Players may be more or less injury-prone, but **injuries are not a performance category as such**
- In real life, injuries occur relatively much less frequently and are much less predictable than things like homeruns and strikeouts
- **We do not try to project players' injury propensity with same precision that their performance stats are projected**, because that would be unrealistic
- **Cannot expect player to miss about number of games he missed on average per season due to injury during actual career**

**Pitcher Injuries:**
- **More pitching = more injury occasions:** The more pitchers pitch, the more occasions there are for them to be injured
- **No increased likelihood:** However, pitching more does not increase the likelihood that they will be injured when such occasions arise
- Pitching beyond endurance does not increase injury risk per occasion

**DH Benefit:**
- Being used as DH **reduces but does not eliminate** injury risk

**Postseason Injury Replacement:**
- **Cannot make changes during playoff series** (as was case in major leagues until recently)
- **Can make changes between:** End of regular season and LCS, and between LCS and World Series
- Can replace injured player with player from Inactive-Injury Reserve list between series

**Injury Rate Estimates (Community Research - DvdAvins/tonzmaniac):**
Based on missed Plate Appearances (includes partial games missed):

| Rating | Missed PA % | Notes |
|--------|-------------|-------|
| Iron | ~1% | Minimal missed time |
| Normal | ~4% | Standard injury risk |
| Prone | ~7% (DvdAvins) or ~10% (tonzmaniac) | Moderate increased risk |
| Fragile | ~11% (DvdAvins) or ~13-14% (tonzmaniac) | Significant injury risk |

**Important Notes:**
- These are estimates including partial games missed (not just full games)
- Actual rates may vary - DvdAvins and tonzmaniac estimates differ slightly
- Variance is normal and expected (DMB doesn't project specific injury totals)
- Use these as planning guidelines, not guarantees

---

## Ballparks & Eras

### Ballpark Selection
- **Timing:** Must choose before draft
- **Lock Status:** Cannot change after draft submission
- **Duplicates:** Allowed in standard leagues
- **Strategic Impact:** Build team around park characteristics

### Park Factors
- **Scale:** 100 = league average
- **Above 100:** More of that event (e.g., HRs)
- **Below 100:** Fewer of that event
- **Dynamic Adjustment:** Factors adjust for cross-era usage
- **Components:** Dimensions, surface, elevation, weather, foul territory

### Era Selection
Each era has distinct characteristics affecting play:

#### Dead Ball Era (1903-19)
- Low offense, few HRs, more errors, complete games common
- Pitcher-dominated, inside-the-park action

#### Golden Age (1920-41)
- End of dead ball, increased offense
- Highest batting averages in history

#### Baby Boomers (1946-60)
- More HRs and walks, improved fielding
- Station-to-station baseball

#### Pitcher Era (1963-68)
- Most pitcher-dominated since Dead Ball
- Low scores, fewer HRs, more complete games

#### Turf Time (1969-92)
- Artificial surfaces, symmetrical stadiums
- Balanced offense and pitching

#### Home Run Derby (1993-2004)
- Offensive explosion, higher ERAs
- Fewer complete games

---

## Frequently Asked Questions

### Player Pool & Salaries
**Q: How are player salaries determined?**
A: Based on comprehensive performance formulas; adjusted 3x yearly based on usage.

**Q: Why aren't active players in Classic mode?**
A: Classic uses full career data; active players only in SSG mode.

### Player Performance
**Q: How do cross-era matchups work?**
A: Advanced normalization adjusts for era, park, and competition level differences.

**Q: Are clutch ratings used?**
A: No; clutch ability is not quantified in Classic mode.

### Team Management
**Q: What if I don't set tactical instructions?**
A: Computer Manager uses MLB data-based tendencies and situational logic.

**Q: How many pitchers should I carry?**
A: Minimum 8 (4 SP), but modern tactics often require 10-12 pitchers.

### Drafting & Trading
**Q: Should I always list alternate picks?**
A: Only if you want one or the other; otherwise leave blank for better next picks.

**Q: What is trade deficit?**
A: Maximum 10% salary difference allowed between trade sides.

---

## Ballpark Reference Data

### Key Historical Parks

**Extreme Home Run Parks:**
- **Baker Bowl (1915-19):** HR factors 227/227 (LH/RH) - Most extreme HR park in database
- **Columbia Park (1903-07):** 162/111 HR factors, asymmetrical (RF: 280ft, LF: 340ft)
- **South End Grounds (1910-14):** 152/152 HR factors, extreme foul territory differences
- **Polo Grounds (1952-56):** 154/157 HR factors, famous for short foul lines (280ft/258ft)

**Pitcher-Friendly Parks:**
- **Astrodome (1977-81):** 59/46 HR factors, most HR-suppressing park
- **Braves Field (1915-19):** 42/42 HR factors, massive dimensions (440ft CF, 402ft LF)
- **Griffith Stadium (1921-25):** 30/32 HR factors, enormous CF (421ft)
- **Dodger Stadium (1962-66):** 57/59 HR factors, large dimensions throughout

**Triples Parks:**
- **Comerica Park (2018-22):** 176/165 triples factors, gap-heavy design
- **Forbes Field (1959-63):** 173/170 triples factors, deep gaps and corners
- **Minute Maid Park (2020-24):** 172/94 triples factors, extreme LH advantage
- **Oracle Park (2015-19):** 168/118 triples factors, deep foul territory

**Modern Extreme Parks:**
- **Coors Field (2020-24):** 107/120 HR factors, 158/140 triples factors (altitude effect)
- **Fenway Park (2020-24):** 92/101 HR factors, 139/133 triples factors (Green Monster effect)
- **Guaranteed Rate Field (2020-24):** 121/111 HR factors, 67/65 triples factors

### Era-Specific Considerations

**Dead Ball Era Characteristics:**
- HR factors often extreme due to rarity (few HRs hit league-wide)
- Triples more common, so factors cluster closer to 100
- Large ballparks with distant fences were norm

**Modern Era Characteristics:**
- HR factors compress toward 100 (HRs common league-wide)  
- Triples factors show extreme spreads (triples now rare)
- Smaller, more standardized ballpark dimensions

**Cross-Era Usage:**
- Historical extreme parks get factor compression in modern eras
- Modern parks get factor expansion in historical low-offense eras
- Game engine handles these adjustments automatically

---

## League-Specific Rule Modifications

> **Note:** This section references external league rule files that override or modify these standard rules.

### Implementation
League-specific rules should be documented in separate files:
- **File Format:** `league_rules_[LEAGUE_NAME].md`
- **Location:** Same directory as this file
- **Override Priority:** League rules supersede standard rules when conflicts exist

### Common Modifications
- Salary cap adjustments
- Draft format changes
- Roster size variations
- Tactical setting restrictions
- Playoff format modifications
- Trading deadline changes

### Reference Example
```markdown
<!-- In league_rules_600ladder.md -->
# 600 Ladder League Rules

## Salary Cap Override
- **Standard Rule:** $100M (Classic) / $130M (SSG)
- **League Rule:** $120M for all team types

## Draft Modifications
- **Standard Rule:** 28 rounds
- **League Rule:** 30 rounds (27 active + 3 IR)
```

---

## Rule Version History

### 2025.1 (October 3, 2025)
- Initial unified rules compilation
- Added league-specific override framework
- Consolidated FAQ section
- Updated era descriptions

---

**Reference:** Diamond Mind Baseball Online Official Rules, 2025  
**Maintained by:** League Analytics Team  
**Contact:** For rule clarifications or updates