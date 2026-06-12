# Player Instructions Advisor
## The Perfecter Plan - Complete Player-by-Player Recommendations

**Team:** The Perfecter Plan  
**League:** Alexis Rivero League (No DH)  
**Home Park:** Royals Stadium (1975-1979)

---

## OVERVIEW

Based on your roster data, here are the **specific player instruction overrides** you need to configure. Most players will stay at "(Team Default)" - only those with specific characteristics need overrides.

**Summary:**
- **19 position players** analyzed
- **9 pitchers** analyzed
- **47 total override recommendations** across 18 settings
- **Critical overrides:** 12 (must configure)
- **Recommended overrides:** 35 (should configure)

---

## HITTERS - PLAYER INSTRUCTIONS

### CRITICAL OVERRIDES (Must Configure)

#### 1. ALL PITCHERS - Sacrifice Bunting = 5 ⚠️
**Players:** Johnson, Gura, Haren, Minor, Moskau (if activated)

**Current Team Setting:** Will be 6 (Never) after you change it  
**Required Override:** All pitchers = **5**

**Reasoning:** No DH league means pitchers bat. Team setting 6 prevents ALL bunting. Must override pitchers individually to allow situational sacrifices.

---

#### 2. CLOSERS - Pull for Closer = 5 ⚠️

**Felipe Vazquez (L):**
- Current: Unknown
- **Required: 5 (Least frequent removal)**
- **Reasoning:** Team "Using Closer" = 2 brings him in aggressively. Setting 5 keeps him in game once entered.

**Trevor Rosenthal (R):**
- Current: Unknown
- **Required: 5 (Least frequent removal)**
- **Reasoning:** Same as Vazquez. Both closers should stay in game.

---

#### 3. PLATOON PLAYERS - Pull for PH Settings ⚠️

**Strong Platoon Splits (>100 OPS points):**

**Joe Panik (L, 2B):** BatPlat +1, OPS vs L .631 / vs R .640 (WEAK SPLIT)
- Pull for PH vs LHP: **(Team Default)** - split too small
- Pull for PH vs RHP: **(Team Default)**
- Pull for PH in Platoon: **(Team Default)**
- **Note:** Despite BatPlat +1, actual split is only 9 points. NOT a platoon candidate.

**Jeff Baker (R, 2B):** BatPlat -3, OPS vs L .730 / vs R .628 (**102 point split**)
- Pull for PH vs LHP: **5** (keep in vs LHP - strong side)
- Pull for PH vs RHP: **1** (pull vs RHP - weak side)
- Pull for PH in Platoon: **1** (trigger swap)
- **Reasoning:** 102-point platoon split. Designated platoon partner for Panik.

**Yangervis Solarte (S, 3B):** BatPlat +1, OPS vs L .554 / vs R .727 (**173 point split!**)
- Pull for PH vs LHP: **1** (pull vs LHP - much weaker)
- Pull for PH vs RHP: **5** (keep in vs RHP - strong side)
- Pull for PH in Platoon: **1** (trigger swap)
- **Reasoning:** HUGE split for switch hitter. Treat as RHB for platoon purposes.

**Marcus Thames (R, LF):** BatPlat -1, OPS vs L .737 / vs R .700 (37 point split)
- Pull for PH vs LHP: **5** (keep in vs LHP - strong side)
- Pull for PH vs RHP: **(Team Default)** - split modest
- Pull for PH in Platoon: **(Team Default)**
- **Reasoning:** Modest split, but platoon partner for Hockett.

**Oris Hockett (L, LF):** BatPlat +1, OPS vs L .736 / vs R .690 (46 point split - WRONG DIRECTION!)
- Pull for PH vs LHP: **(Team Default)**
- Pull for PH vs RHP: **(Team Default)**  
- Pull for PH in Platoon: **(Team Default)**
- **Reasoning:** REVERSE split (hits LHP better despite being LHB). Not a platoon candidate.

**Jimmy Sebring (L, RF):** BatPlat +1, OPS vs L .658 / vs R .675 (17 point split)
- Pull for PH vs LHP: **(Team Default)** - split too small
- Pull for PH vs RHP: **(Team Default)**
- Pull for PH in Platoon: **(Team Default)**

**Jason Lane (R, RF):** BatPlat 0, OPS vs L .653 / vs R .677 (24 point split)
- Pull for PH vs LHP: **(Team Default)** - split too small
- Pull for PH vs RHP: **(Team Default)**
- Pull for PH in Platoon: **(Team Default)**

**Allen Craig (R, 1B):** BatPlat -1, OPS vs L .786 / vs R .681 (**105 point split**)
- Pull for PH vs LHP: **5** (keep in vs LHP - strong side)
- Pull for PH vs RHP: **1** (pull vs RHP - weak side)
- Pull for PH in Platoon: **1** (trigger swap)
- **Reasoning:** 105-point split. Platoon candidate with Smoak.

**Justin Smoak (S, 1B):** BatPlat +1, OPS vs L .649 / vs R .711 (62 point split)
- Pull for PH vs LHP: **1** (pull vs LHP - weaker side)
- Pull for PH vs RHP: **5** (keep in vs RHP - strong side)
- Pull for PH in Platoon: **1** (trigger swap)
- **Reasoning:** 62-point split. Platoon partner for Craig.

---

#### 4. REGULAR STARTERS - Pull for PH = 6 (Never) or 5

**Austin Jackson (CF):** Regular starter, .664 OPS vs R
- Pull for PH vs LHP: **6 (Never)** - only CF
- Pull for PH vs RHP: **6 (Never)** - only CF
- **Reasoning:** Only true everyday CF. Must stay in game.

**Jordy Mercer (SS):** Regular starter, .590 OPS vs R (weak!)
- Pull for PH vs LHP: **5** (keep in for defense despite weak bat)
- Pull for PH vs RHP: **6 (Never)** - only SS, very weak vs RHP
- **Reasoning:** Defensive specialist SS. Weak hitter but no replacement.

**Don Slaught (C):** Regular catcher
- Pull for PH vs LHP: **4** (your current setting - good)
- Pull for PH vs RHP: **4** (your current setting - good)
- **Reasoning:** Need him in game, but Vaughn available as backup C.

---

### RECOMMENDED OVERRIDES (By Setting)

#### BUNTING FOR HIT

**Players with Ex/Vg Bunt Rating:**
- **NONE** on your roster have Ex bunt for hit
- **Joe Panik (Vg Sac, Fr Hit):** (Team Default) - only Vg at sacrifice, not hit

**Players with Pr Bunt Rating - Set to 5-6:**
- Craig, Allen (Pr/Pr): **6 (Never)**
- Thames, Marcus (Pr/Pr): **6 (Never)**
- Doyle, Jack (Fr/Pr): **5**
- Kotchman, Casey (Fr/Pr): **5**
- Schierholtz, Nate (Fr/Pr): **5**
- Sebring, Jimmy (Fr/Pr): **5**
- Panik, Joe (Vg/Fr): **(Team Default)** - good at sac, not hit
- Baker, Jeff (Pr/Fr): **5**
- Smoak, Justin (Pr/Pr): **6 (Never)**
- Lane, Jason (Av/Pr): **(Team Default)**
- Clark, Jerald (Fr/Pr): **5**
- Vaughn, Farmer (Fr/Pr): **5**
- Berra, Dale (Av/Pr): **(Team Default)**
- Duffee, Charlie (Av/Pr): **(Team Default)**
- Hockett, Oris (Fr/Pr): **5**

**Reasoning:** Pr bunt rating = wasted outs. Prevent attempts.

---

#### SACRIFICE BUNTING

**Position Players:** All stay at (Team Default) = 6 after team change

**Pitchers (CRITICAL):**
- Johnson, Randy: **5**
- Gura, Larry: **5**
- Haren, Dan: **5**
- Minor, Mike: **5**
- Moskau, Paul: **5** (if activated)
- Vazquez, Felipe: **(Team Default)** - reliever, won't bat much
- Rosenthal, Trevor: **(Team Default)** - reliever
- Laxton, Bill: **(Team Default)** - reliever
- Lasher, Fred: **(Team Default)** - reliever

---

#### BASE STEALING

**Elite Stealers (Vg/Ex Steal + Speed):**

**Jimmy Sebring (Ex Run, Vg Steal, Fr Jump):**
- **Recommendation: 3** (neutral - already optimal)
- **Reasoning:** Ex baserunning + Vg stealing. Setting 3 = steals at every good opportunity. Don't go to 1-2 (too aggressive per official guidance).

**Austin Jackson (Vg Run, Vg Steal, Av Jump):**
- **Recommendation: 3** (neutral)
- **Reasoning:** Vg in both categories. Setting 3 optimal.

**Jack Doyle (Av Run, Vg Steal, Ex Jump):**
- **Recommendation: 4** (slightly conservative)
- **Reasoning:** Ex jump rating = 27% pickoff attempt rate. Setting 4 helps avoid pickoffs.

**Good Stealers (Av Steal):**
- Slaught, Don (Fr Run, Av Steal): **5** (slow runner negates steal ability)
- Panik, Joe (Av Run, Av Steal): **(Team Default)**
- Mercer, Jordy (Av Run, Av Steal): **(Team Default)**
- Baker, Jeff (Fr Run, Av Steal): **5** (slow runner)
- Lane, Jason (Fr Run, Av Steal): **5** (slow runner)
- Schierholtz, Nate (Vg Run, Av Steal): **4** (good runner, moderate stealer)
- Duffee, Charlie (Av Run, Av Steal): **(Team Default)**
- Craig, Allen (Fr Run, Av Steal): **5** (slow runner)

**Poor Stealers (Pr/Fr Steal):**
- Thames, Marcus (Av Run, Pr Steal): **6 (Never)**
- Smoak, Justin (Pr Run, Pr Steal): **6 (Never)**
- Kotchman, Casey (Pr Run, Fr Steal): **6 (Never)**
- Hockett, Oris (Av Run, Fr Steal): **5**
- Clark, Jerald (Fr Run, Fr Steal): **6 (Never)**
- Vaughn, Farmer (Pr Run, Fr Steal): **6 (Never)**
- Solarte, Yangervis (Av Run, Fr Steal): **5**

---

#### BASERUNNING

**Ex Runners:**
- **Sebring, Jimmy (Ex Run):** **2** (aggressive - exploit skill)

**Vg Runners:**
- **Jackson, Austin (Vg Run):** **2** (aggressive)
- **Schierholtz, Nate (Vg Run):** **2** (aggressive)

**Pr Runners:**
- **Smoak, Justin (Pr Run):** **5** (very conservative)
- **Vaughn, Farmer (Pr Run):** **5** (very conservative)

**Fr Runners:**
- **Baker, Jeff (Fr Run):** **4** (conservative)
- **Craig, Allen (Fr Run):** **4** (conservative)
- **Slaught, Don (Fr Run):** **4** (conservative)
- **Thames, Marcus (Av Run):** **(Team Default)**
- **Lane, Jason (Fr Run):** **4** (conservative)
- **Clark, Jerald (Fr Run):** **4** (conservative)

**Everyone else:** (Team Default) = 3

---

#### HIT-AND-RUN

**Power Hitters (waste XBH opportunity):**
- **Thames, Marcus (SLG .422):** **5** (below average)
- **Craig, Allen (SLG .397):** **5** (below average)

**High K-rate (need to check actual K%):** 
- If K% >20%: Set to **5-6**
- Most of your team appears contact-oriented, so **(Team Default)** for most

---

#### TAKING 3-0 PITCHES

**High Walk Rate (BBF < 12):**
- **Smoak, Justin (BBF 8.71):** **5** (green light - elite walk rate)
- **Thames, Marcus (BBF 11.9):** **(Team Default)**
- **Lane, Jason (BBF 11.2):** **(Team Default)**
- **Baker, Jeff (BBF 12.7):** **(Team Default)**
- **Jackson, Austin (BBF 12.8):** **(Team Default)**

**Low Walk Rate (BBF > 18):**
- **Clark, Jerald (BBF 22.2):** **4** (swing early - doesn't walk much)
- **Vaughn, Farmer (BBF 27.3):** **4** (swing early)
- **Berra, Dale (BBF 18.2):** **(Team Default)**

**Everyone else:** (Team Default)

---

#### DOUBLE SWITCH

**Bench Players (willing to use in double switch):**
- **Doyle, Jack:** **2** (utility, can fill multiple positions)
- **Duffee, Charlie:** **2** (CF backup, defensive sub)
- **Schierholtz, Nate:** **2** (OF backup)
- **Clark, Jerald:** **2** (bench OF)
- **Vaughn, Farmer:** **2** (backup C)
- **Berra, Dale:** **2** (backup SS)

**Regular Starters:**
- **Jackson, Austin:** **5** (everyday CF, avoid removal)
- **Mercer, Jordy:** **5** (everyday SS, defensive specialist)
- **Slaught, Don:** **4** (regular C but Vaughn available)

**Platoon Players:** (Team Default) - depends on game situation

---

#### PH IN BLOWOUT

**High-Salary Stars (rest in blowouts):**
- **Johnson, Randy ($20.9M):** **5** (rest when possible)
- **Gura, Larry ($9.4M):** **4**
- **Haren, Dan ($9.1M):** **4**
- **Vazquez, Felipe ($7.9M):** **4**
- **Minor, Mike ($7.1M):** **4**
- **Craig, Allen ($5.0M):** **4**

**Fragile Injury Rating (rest more often):**
- **Slaught, Don (Fragile):** **1** (rest in blowouts)
- **Panik, Joe (Fragile):** **1** (rest often - already set!)
- **Craig, Allen (Fragile):** **4** (balance value vs injury risk)

**Bench Players (get playing time):**
- **Vaughn, Farmer:** **1** (bench C, get ABs in blowouts)
- **Berra, Dale:** **1** (bench IF)
- **Clark, Jerald:** **1** (bench OF)

**Everyone else:** (Team Default) = 3

---

## PITCHERS - PLAYER INSTRUCTIONS

### SACRIFICE BUNTING (CRITICAL)

**All Starting Pitchers:** **5**
- Johnson, Randy: **5**
- Gura, Larry: **5**
- Haren, Dan: **5**
- Minor, Mike: **5**
- Moskau, Paul: **5** (if activated from IR)

**Relievers:** (Team Default) = 6 (won't bat often in NL)

---

### PITCHING AROUND

**All Pitchers:** **4-5** (avoid wasting pitches, challenge hitters)

**High Walk Rate (BB/9 > 4.0):**
- **Laxton, Bill (BB/9 6.51):** **5** (avoid compounding control issues)
- **Lasher, Fred (BB/9 6.10):** **5**
- **Rosenthal, Trevor (BB/9 4.96):** **5**
- **Moskau, Paul (BB/9 4.61):** **5**

**Good Control (BB/9 < 3.0):**
- **Haren, Dan (BB/9 2.45):** **(Team Default)**
- **Gura, Larry (BB/9 2.94):** **(Team Default)**
- **Minor, Mike (BB/9 3.07):** **(Team Default)**

---

### INTENTIONAL WALKS

**Same as Pitching Around** - set high-walk pitchers to **5**, others (Team Default)

---

### PICKOFF THROWS

**ALL PITCHERS:** **5 (Least frequent)**

**Reasoning:** Community consensus - "Pitches too valuable to waste."

- Johnson, Randy: **5**
- Gura, Larry: **5**
- Haren, Dan: **5**
- Vazquez, Felipe: **5**
- Minor, Mike: **5**
- Rosenthal, Trevor: **5**
- Laxton, Bill: **5**
- Lasher, Fred: **5**
- Moskau, Paul: **5**

**Exception:** None of your pitchers have VG/Ex Hold + great 1B fielding combination

---

### PITCHOUTS

**ALL PITCHERS:** **5 (Least frequent)**

**Same reasoning as pickoffs** - conserve pitches, accept stolen bases.

---

### PULL FOR RELIEVER

**Ace Starters (ride them):**
- **Johnson, Randy (Ex SP Dur):** **4** (let him work deep)

**Quality Starters:**
- **Gura, Larry (Vg SP Dur):** **3** (neutral - good durability)
- **Minor, Mike (Vg SP Dur):** **3** (neutral)

**Average/Weak Starters:**
- **Haren, Dan (Av SP Dur):** **(Team Default)** = 2 (slightly quick hook)
- **Moskau, Paul (Fr SP Dur):** **2** (quick hook - low endurance)

**Relievers:**
- **All relievers:** (Team Default) = 2 (slightly quick hook is fine)

---

### PULL FOR CLOSER ⚠️ CRITICAL

**Closers (MUST CONFIGURE):**
- **Vazquez, Felipe:** **5 (Least frequent removal)**
- **Rosenthal, Trevor:** **5 (Least frequent removal)**

**Reasoning:** Team "Using Closer" = 2 brings them in aggressively (8th inning, tie games). Setting 5 keeps them in game once entered.

**Setup Men:**
- **Laxton, Bill:** **(Team Default)** = can be removed if needed
- **Lasher, Fred:** **(Team Default)** = can be removed if needed

**Starters:**
- **All starters:** (Team Default) = want them out for closer/setup in high leverage

---

## CONFIGURATION SUMMARY

### Priority 1: CRITICAL (Must Do)

1. **Team Sacrifice Bunting → 6 (Never)**
2. **All 4 SP Sacrifice Bunting → 5** (Johnson, Gura, Haren, Minor)
3. **Vazquez Pull for Closer → 5**
4. **Rosenthal Pull for Closer → 5**
5. **Platoon Pull for PH settings:**
   - Baker: vs RHP = 1, Platoon = 1
   - Solarte: vs LHP = 1, Platoon = 1
   - Craig: vs RHP = 1, Platoon = 1
   - Smoak: vs LHP = 1, Platoon = 1

### Priority 2: HIGH VALUE (Should Do)

6. **Regular starters Pull for PH → 6 (Never):**
   - Jackson (CF): both = 6
   - Mercer (SS): vs RHP = 6

7. **All pitchers Pickoff Throws → 5**
8. **All pitchers Pitchouts → 5**
9. **Poor bunters → 5-6 (Never)**
10. **Poor stealers → 5-6 (Never)**

### Priority 3: OPTIMIZATION (Nice to Have)

11. **Baserunning by speed** (Sebring/Jackson = 2, slow = 4-5)
12. **Double Switch** (bench = 2, starters = 5)
13. **PH in Blowout** (stars/fragile = 4-5, bench = 1)

---

## QUICK REFERENCE: TOP 10 PLAYERS TO CONFIGURE

| Player | Setting | Value | Reason |
|--------|---------|-------|--------|
| **Randy Johnson** | Sacrifice Bunting | 5 | SP in No DH league |
| **Felipe Vazquez** | Pull for Closer | 5 | Keep closer in game |
| **Trevor Rosenthal** | Pull for Closer | 5 | Keep closer in game |
| **Jeff Baker** | Pull for PH vs RHP | 1 | 102-pt platoon split |
| **Yangervis Solarte** | Pull for PH vs LHP | 1 | 173-pt platoon split! |
| **Allen Craig** | Pull for PH vs RHP | 1 | 105-pt platoon split |
| **Justin Smoak** | Pull for PH vs LHP | 1 | 62-pt platoon split |
| **Austin Jackson** | Pull for PH (both) | 6 | Only everyday CF |
| **Jimmy Sebring** | Baserunning | 2 | Ex run rating |
| **Justin Smoak** | Base Stealing | 6 | Pr steal, prevent attempts |

---

**Version:** 1.0 - Complete Roster Analysis  
**Data Source:** Actual team ratings from bball/team pages  
**Next Step:** Apply these recommendations in Player Instructions screen