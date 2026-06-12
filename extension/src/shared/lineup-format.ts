/**
 * @file Lineup Lab stat formatters — display helpers for comparison grid cells.
 *
 * **Purpose:** Formats rate stats (OBP, OPS), RC/600, and defensive runs for the
 * side panel HTML table using baseball conventions (no leading zero on sub-1 rates).
 *
 * **Message flow:** Pure functions; imported by `sidepanel/sidepanel.ts` during
 * grid rendering only.
 *
 * **Dependencies:** None.
 */

// --- Rate formatting ---

/**
 * Formats a rate stat with optional leading-zero strip for values below 1.
 *
 * @param n - Numeric rate (e.g. OBP 0.405).
 * @param decimals - Fixed decimal places.
 * @returns Formatted string or em dash when missing, NaN, or non-positive.
 */
export function fmtAvg(n: number | undefined, decimals: number): string {
  if (n == null || Number.isNaN(n) || n <= 0) return "—";
  const s = n.toFixed(decimals);
  return s.startsWith("0.") ? s.slice(1) : s;
}

/**
 * Formats RC/600 to one decimal place.
 *
 * @param n - Runs created per 600 PA.
 * @returns Formatted string or em dash when missing, NaN, or non-positive.
 */
export function fmtRc600(n: number | undefined): string {
  if (n == null || Number.isNaN(n) || n <= 0) return "—";
  return n.toFixed(1);
}

/**
 * Formats defensive runs saved/lost with sign and one decimal; near-zero → `"0"`.
 *
 * @param n - Defensive value in runs.
 * @returns Signed decimal string, `"0"`, or em dash when undefined.
 */
export function fmtDef(n: number | undefined): string {
  if (n == null) return "—";
  const v = Math.round(n * 10) / 10;
  if (Math.abs(v) < 0.05) return "0";
  return v > 0 ? `+${v.toFixed(1)}` : v.toFixed(1);
}
