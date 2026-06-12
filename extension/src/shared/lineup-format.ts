/** Baseball rate formatting for Lineup Lab grid (no leading zero on rates). */

export function fmtAvg(n: number | undefined, decimals: number): string {
  if (n == null || Number.isNaN(n) || n <= 0) return "—";
  const s = n.toFixed(decimals);
  return s.startsWith("0.") ? s.slice(1) : s;
}

export function fmtRc600(n: number | undefined): string {
  if (n == null || Number.isNaN(n) || n <= 0) return "—";
  return n.toFixed(1);
}

export function fmtDef(n: number | undefined): string {
  if (n == null) return "—";
  const v = Math.round(n * 10) / 10;
  if (Math.abs(v) < 0.05) return "0";
  return v > 0 ? `+${v.toFixed(1)}` : v.toFixed(1);
}
