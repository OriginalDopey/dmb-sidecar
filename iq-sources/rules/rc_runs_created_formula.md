# Runs Created (RC) Formula - Official Calculation

> **Source:** Imagine Sports "Explanation of Statistics" page (official formula)  
> **Important Note:** The formula in the Glossary differs from the actual formula used by the system  
> **Last Updated:** January 2025

---

## Critical Discrepancy: Glossary vs. Actual Formula

**Important Finding:** The Glossary section shows a different RC formula than what is actually used by Imagine Sports. The **actual formula** used by the system is found in the "Explanation of Statistics" section and **includes strikeouts** in the calculation.

**Why This Matters:**
- Players can have **negative RC** even without GDP or CS
- High strikeout rates can cause negative RC values
- The Glossary formula is incomplete/outdated

---

## Official RC Formula (From "Explanation of Statistics")

The formula used by Imagine Sports to calculate Runs Created:

### Step 1: Calculate A (On-Base Component)

```
A = H + BB + HBP – CS – GIDP
```

**Where:**
- **H** = Hits
- **BB** = Bases on Balls (Walks)
- **HBP** = Hit by Pitch
- **CS** = Caught Stealing
- **GIDP** = Grounded into Double Plays

### Step 2: Calculate B (Advancement Component)

```
B = .24 × (BB – IBB + HBP) + .62 × SB + .50 × (SH + SF) + TB - .03 × K
```

**Where:**
- **BB** = Bases on Balls (Walks)
- **IBB** = Intentional Walks
- **HBP** = Hit by Pitch
- **SB** = Stolen Bases
- **SH** = Sacrifice Hits (bunts)
- **SF** = Sacrifice Flies
- **TB** = Total Bases (H + 2B + 2×3B + 3×HR)
- **K** = Strikeouts ⚠️ **This is the key difference from Glossary**

**Critical Component:** The `-.03 × K` term means strikeouts **reduce** the B value, which can make B negative for high-strikeout players.

### Step 3: Calculate C (Opportunities Component)

```
C = AB + BB + HBP + SH + SF
```

**Where:**
- **AB** = At Bats
- **BB** = Bases on Balls
- **HBP** = Hit by Pitch
- **SH** = Sacrifice Hits
- **SF** = Sacrifice Flies

### Step 4: Calculate RC (Final Formula)

```
RC = (2.4 × C + A) × (3 × C + B) / (9 × C) – 0.9 × C
```

---

## Why Negative RC Can Occur

### Primary Causes

1. **High Strikeout Rate:** The `-.03 × K` term in the B component can make B negative
   - Example: Player with 5 strikeouts in 5 PA
   - B = .24(0) + .5(0) + 0 - .03(5) = -0.15
   - This negative B value can result in negative RC

2. **Ground Into Double Plays:** GIDP reduces the A component
   - A = H + BB + HBP – CS – GIDP
   - High GIDP rates reduce A, contributing to lower RC

3. **Caught Stealing:** CS reduces the A component
   - A = H + BB + HBP – CS – GIDP
   - High CS rates reduce A

### Example Calculation: Negative RC

**Player Stats:**
- 5 PA, 0 H, 0 BB, 0 HBP, 0 SH, 0 SF, 5 K, 0 GDP, 0 CS

**Step 1: Calculate A**
```
A = 0 + 0 + 0 - 0 - 0 = 0
```

**Step 2: Calculate B**
```
B = .24(0) + .5(0) + 0 - .03(5) = -0.15
```

**Step 3: Calculate C**
```
C = 5 + 0 + 0 + 0 + 0 = 5
```

**Step 4: Calculate RC**
```
RC = (2.4 × 5 + 0) × (3 × 5 + (-0.15)) / (9 × 5) – 0.9 × 5
RC = (12) × (14.85) / 45 – 4.5
RC = 178.2 / 45 – 4.5
RC = 3.96 – 4.5
RC = -0.54
```

**Result:** Negative RC of -0.54 runs created

### General Rule for Negative RC

According to community analysis (DvdAvins):
- If every PA is an ordinary out, RC/600 = **-60.00**
- That equals **-0.1 per PA**
- Strikeouts and GDP can make it worse

---

## RC/600 and RC/650 Normalization

### RC/600 (Per 600 At Bats)

```
RC/600 = (RC / PA) × 600
```

**Note:** This is calculated per 600 **at bats**, not plate appearances. However, the formula uses PA in the denominator.

### RC/650 (Per 650 Plate Appearances)

```
RC/650 = (RC / PA) × 650
```

**Where PA = AB + BB + HBP + SH + SF**

---

## RC27: Runs Created Per 27 Outs

```
RC27 = RC / OUT × 27
```

**Where OUT = (AB – H) + GIDP + CS + SH + SF**

**Key Insight:** RC27 accounts for how many outs a player makes. A player with higher OBP will have fewer outs, resulting in higher RC27 than RC+ (normalized RC per PA).

---

## Normalized RC (RC+)

RC+ is calculated season-by-season and then aggregated:

```
RC+ = (Player's RC / League Average RC in same PA) × 100
```

**Calculation Method:**
1. For each season, calculate what the league average hitter would have produced in the player's number of PA
2. Compare player's actual RC to league average RC
3. Sum across all seasons
4. Multiply by 100 (where 100 = league average)

**Key Point:** RC+ accounts for era differences, making players from different offensive eras comparable.

---

## Important Notes for Player Evaluation

### RC/600 Does NOT Include:
- **Run Rating:** Baserunning value must be added separately
- **Defensive Value:** Must be calculated separately
- **Park Effects:** RC is not park-adjusted (must apply park factors separately if desired)

### When Evaluating Players:
1. **Use RC/600 or RC/650** as primary offensive metric
2. **Add baserunning value** separately (run rating)
3. **Consider platoon splits** - calculate separately for LHP vs RHP
4. **Apply park adjustments** if evaluating extreme park factors (±3 RC for extreme parks)

### Negative RC Interpretation:
- Negative RC is **normal** for pitchers with limited ABs
- High strikeout hitters can have negative RC in small samples
- Don't panic about negative RC for part-time players
- Focus on RC/600 or RC/650 for meaningful evaluation

---

## Formula Comparison: Glossary vs. Actual

### Glossary Formula (Incomplete)
The Glossary shows a formula **without** the strikeout component in B:
```
B = .24(BB – IBB + HBP) + .62(SB) + .50(SH + SF) + TB
```
**Missing:** `-.03 × K` term

### Actual Formula (Used by System)
```
B = .24(BB – IBB + HBP) + .62(SB) + .50(SH + SF) + TB - .03(K)
```
**Includes:** Strikeout penalty term

**Recommendation:** Always use the "Explanation of Statistics" formula, not the Glossary version.

---

## References

- **Official Source:** Imagine Sports "Explanation of Statistics" page
- **Community Discussion:** Message board thread on negative RC (January 2025)
- **Key Contributors:** TylerEnsor, DvdAvins, HooverH, willibphx

---

*This document clarifies the actual RC formula used by Imagine Sports and explains why negative RC values can occur, particularly for high-strikeout players or pitchers with limited plate appearances.*
