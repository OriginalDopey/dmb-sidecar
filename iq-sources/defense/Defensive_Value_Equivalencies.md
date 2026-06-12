# Defensive Value Equivalencies - Range vs. Error Translation

## Key Formulas (Community Research)

### Range Grade Value:
- **Each range grade = 7-10 runs per season**
- Moving up one grade (Fr→Av→Vg→Ex) saves ~10 hits per season
- Moving down one grade loses ~10 hits per season

### Error Rating Value:
- **Each 50 error rating points = ~10 runs per season** (varies by position)
- Lower error rating = fewer errors = fewer runs allowed
- Error cost varies by position (0.5-1.13 runs per error)

### Combined Formula:
**Defensive Value = (Range Grade Value) + (Error Value)**

---

## Defensive Value Translation System

### Base Equivalencies (Approximate):

**Range Grade Progression:**
- Ex = +20-30 runs vs. Average (Av)
- Vg = +10-15 runs vs. Average (Av)
- Av = 0 runs (baseline)
- Fr = -10-15 runs vs. Average (Av)
- Pr = -20-30 runs vs. Average (Av)

**Error Rating Impact (per 50 points):**
- 0-50 ERR: +10 runs (fewer errors = saves runs)
- 51-100 ERR: 0 runs (baseline ~100 ERR)
- 101-150 ERR: -10 runs (more errors = costs runs)
- 151+ ERR: -15-20 runs (many more errors)

**Note:** Error impact varies by position (CF errors cost more than 2B errors)

---

## Salmon Example: Fr/0 Equivalent to What?

**Salmon: Fr/0 ERR at RF**

### Calculation:

**Range Impact:**
- Fr = -10 to -15 runs vs. Average
- Let's use -12 runs as middle estimate

**Error Impact:**
- 0 ERR = Makes 0% of average errors
- Average errors ~100 ERR
- Difference: 100 points below average
- Value: ~+20 runs (at RF, errors cost ~0.63 runs each)
- But wait, 0 ERR means virtually NO errors
- This is exceptional - could be worth +15-20 runs

**Total Defensive Value:**
- Range: -12 runs (Fr penalty)
- Errors: +15-20 runs (0 ERR bonus)
- **Net: +3 to +8 runs** defensive value

**Equivalent Range/Error Combinations:**

Salmon (Fr/0) ≈ **Av/65-75** or **Vg/90-100**

**Translation Examples:**
- Fr/0 ≈ Av/70 (Average range + good error rating)
- Fr/0 ≈ Vg/95 (Very Good range + average errors)
- Fr/0 is roughly **equivalent to average defender overall**

---

## Defensive Value Comparison Matrix

### Equivalent Defensive Value Combinations:

**At SS (0.62 runs per error):**

| Rating | Defensive Value | Equivalent To |
|--------|----------------|---------------|
| Ex/50 | +25 runs | Vg/25, Av/0 |
| Ex/100 | +10 runs | Vg/65, Av/50 |
| Vg/50 | +15 runs | Ex/100, Av/0 |
| Av/50 | +5 runs | Ex/150, Vg/100 |
| Fr/0 | +8 runs | Av/60, Vg/95 |
| Fr/100 | -7 runs | Av/150, Pr/50 |

**At CF (1.13 runs per error - errors cost more!):**

| Rating | Defensive Value | Equivalent To |
|--------|----------------|---------------|
| Ex/50 | +30 runs | Vg/25, Av/0 |
| Ex/100 | +15 runs | Vg/65, Av/50 |
| Vg/33 | +20 runs | Ex/65, Av/25 |
| Av/100 | 0 runs | Baseline |
| Fr/112 | -18 runs | Av/140, Pr/80 |

**At 2B (0.50 runs per error - errors cost less):**

| Rating | Defensive Value | Equivalent To |
|--------|----------------|---------------|
| Ex/50 | +22 runs | Vg/30, Av/0 |
| Ex/81 | +10 runs | Vg/70, Av/50 |
| Vg/70 | +12 runs | Ex/90, Av/30 |
| Av/100 | 0 runs | Baseline |
| Fr/132 | -12 runs | Av/150, Pr/80 |

---

## Practical Translation Examples

### Example 1: Salmon Fr/0 at RF

**Salmon:** Fr/0
- Range: Fr = -12 runs (penalty)
- Errors: 0 ERR = +15 runs (excellent)
- **Total: +3 runs** defensive value

**Equivalent:** Av/65 or Vg/95
- **Conclusion:** Fr/0 is roughly average defensive value overall
- The 0 ERR compensates for Fr range

### Example 2: Germany Smith Ex/84 at SS

**Smith:** Ex/84
- Range: Ex = +20 runs (bonus)
- Errors: 84 ERR = +3 runs (slightly better than average)
- **Total: +23 runs** defensive value

**But:** 84 ERR is high for SS (should be 55-70)
- **More accurate:** Ex range but error rating drags down value
- **Equivalent to:** Vg/50 or Av/30

### Example 3: Belanger Ex/64 at SS

**Belanger:** Ex/64
- Range: Ex = +20 runs
- Errors: 64 ERR = +7 runs (good error rating)
- **Total: +27 runs** defensive value

**Equivalent to:** Vg/25 (much better than Smith)

---

## Defensive Value Translation Formula

### General Formula (Position-Specific):

**Defensive Runs = (Range Value) + (Error Value × Position Multiplier)**

Where:
- Range Value: Ex=+20, Vg=+10, Av=0, Fr=-10, Pr=-20 (approximate)
- Error Value: (100 - ERR) × (Error Cost per Point)
- Position Multiplier: CF=1.13, LF=0.88, C/1B=0.79, SS=0.62, 2B/3B=0.50

### Simplified Formula (Quick Estimate):

**Net Defensive Value ≈ (Range Grade Value) + ((100 - ERR) ÷ 5)**

**Examples:**
- Ex/64: +20 + ((100-64)÷5) = +20 + 7 = **+27 runs**
- Vg/33: +10 + ((100-33)÷5) = +10 + 13 = **+23 runs**
- Fr/0: -10 + ((100-0)÷5) = -10 + 20 = **+10 runs**
- Av/100: 0 + ((100-100)÷5) = 0 + 0 = **0 runs** (baseline)

---

## Your Roster Translation Analysis

### Current Players:

**Lewis (CF): Vg/33**
- Value: +10 (Vg) + 13 (33 ERR) = **+23 runs**
- **Excellent defensive value!**

**Salmon (RF): Fr/0**
- Value: -10 (Fr) + 20 (0 ERR) = **+10 runs**
- **Average defensive value overall**
- Equivalent to: Av/65 or Vg/95

**Smith (SS): Ex/84**
- Value: +20 (Ex) + 3 (84 ERR) = **+23 runs**
- But 84 ERR is poor for SS
- **Should be:** Ex/55 = +29 runs (optimal)
- **Current equivalent:** Vg/50

**Belanger (SS): Ex/64**
- Value: +20 (Ex) + 7 (64 ERR) = **+27 runs**
- **Much better than Smith**
- Equivalent to: Vg/25

---

## Key Insights

### Range vs. Error Trade-offs:

1. **Ex/100 ≈ Vg/65 ≈ Av/30** (similar defensive value)
2. **Fr/0 ≈ Av/65 ≈ Vg/95** (0 ERR compensates for Fr range)
3. **Vg/33 ≈ Ex/65** (excellent error rating = good range grade)

### Position Matters:

- **CF errors cost more** (1.13 runs) - prioritize low ERR
- **2B errors cost less** (0.50 runs) - range matters more
- **SS errors moderate** (0.62 runs) - balance both

### Your Salmon:

**Fr/0 is roughly equivalent to:**
- Av/65-70 (Average range + good errors)
- Vg/90-100 (Very Good range + average errors)

**Bottom line:** Salmon's 0 ERR makes him roughly average defensive value overall, despite Fr range.

---

## Application to Your Roster Decisions

**Salmon (Fr/0) vs. Better Range Options:**

Salmon at RF:
- Fr/0 = +10 runs defensive value
- Roughly equivalent to: Av/65 or Vg/95

**Question:** Is this better than alternatives?

**Answer:** For RF, 0 ERR is excellent. His Fr range hurts, but 0 ERR is rare and valuable. He's roughly average defensively overall - acceptable for RF.

**If you could get Vg/50 instead:**
- Vg/50 = +20 runs defensive value
- Much better than Fr/0

But finding 0 ERR players is rare - Salmon's error rating is exceptional even with Fr range.

