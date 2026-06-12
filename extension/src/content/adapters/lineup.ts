/**
 * @file Edit Lineup page adapter — ImagineSports `/manage/edit_lineup`.
 *
 * **Purpose:** Scrapes batting order from `ul#phoneticlong > li` (not table rows),
 * roster pool eligibility, and batter rating tables into a structured `PageContext`.
 *
 * **Message flow:** Consumed by `registry.extractPageContext` → content script →
 * background; not called directly by messaging layer.
 *
 * **Dependencies:** `adapters/types.js` (`PageAdapter`), `shared/types.js` (`PageContext`,
 * `PageSlot`).
 *
 * @see docs/CODEBASE_MAP.md
 */
import type { PageAdapter } from "./types.js";
import type { PageContext, PageSlot } from "../../shared/types.js";

// --- Constants & patterns ---

const POS_RE = /^(DH|C|1B|2B|3B|SS|LF|CF|RF)$/i;

// --- Pitcher side & label parsing ---

/**
 * Infers opposing pitcher handedness from the lineup name dropdown value.
 *
 * @param lineupName - e.g. `"Primary vs. LHP"` or `"Primary vs RHP"`.
 * @returns `"lhp"` when name indicates vs-lefties, otherwise `"rhp"`.
 */
function detectPitcherSide(lineupName: string | undefined): string {
  const n = (lineupName || "").toLowerCase();
  if (n.includes("vs lhp") || n.includes("v. lhp") || n.includes("vs. lhp")) return "lhp";
  return "rhp";
}

/**
 * Strips ImagineSports option suffixes from a player label.
 *
 * @param text - Raw option text, e.g. `"Cobb, Ty ✻ - (CF, RF)"`.
 * @returns Clean `"Last, First"` name without eligibility or injury markers.
 */
function cleanPlayerLabel(text: string): string {
  return text
    .replace(/\s*-\s*\([^)]+\)\s*$/, "")
    .replace(/\s*[*#✻]\s*$/u, "")
    .replace(/^<INJ>\s*/i, "")
    .replace(/\s+INJ.*$/i, "")
    .trim();
}

/**
 * Parses a roster `<option>` into name and position eligibility list.
 *
 * @param text - Full option label from a player select.
 * @returns Parsed name and positions, or `null` for placeholders/invalid entries.
 */
function parseRosterOption(text: string): { name: string; positions: string[] } | null {
  const raw = (text || "").trim();
  const elig = raw.match(/\(([^)]+)\)\s*$/);
  const positions = elig
    ? elig[1]
        .split(",")
        .map((p) => p.trim())
        .filter(Boolean)
    : [];
  const name = cleanPlayerLabel(raw.replace(/\s*-\s*\([^)]+\)\s*$/, ""));
  if (name.length > 3 && !name.toLowerCase().includes("select") && name.includes(",")) {
    return { name, positions };
  }
  return null;
}

// --- DOM helpers ---

/**
 * Returns the selected option from an IS player dropdown, preferring explicit `[selected]`.
 *
 * @param select - Player `<select>` element.
 * @returns Selected option with a non-empty value, or `null`.
 */
function selectedOption(select: HTMLSelectElement): HTMLOptionElement | null {
  const marked = select.querySelector("option[selected], option[SELECTED]");
  if (marked instanceof HTMLOptionElement && marked.value) return marked;
  const sel = select.selectedOptions[0];
  if (sel?.value) return sel;
  return null;
}

/**
 * Locates the batting-order list root (`ul#phoneticlong` or fallbacks).
 *
 * @param document - Edit Lineup page document.
 * @returns List element containing batting-order `<li>` rows, or `null`.
 */
function lineupListRoot(document: Document): HTMLUListElement | null {
  return (
    document.querySelector<HTMLUListElement>("ul#phoneticlong") ??
    document.querySelector<HTMLUListElement>("form[name='lineup_form'] ul.boxy") ??
    document.querySelector<HTMLUListElement>("ul.boxy")
  );
}

/**
 * Reads the active lineup name from hidden input or adjacent form fields.
 *
 * @param document - Edit Lineup page document.
 * @returns Lineup name string when found.
 */
function extractLineupName(document: Document): string | undefined {
  const named = document.querySelector<HTMLInputElement>("input[name='lineup_name']");
  if (named?.value?.trim()) return named.value.trim();

  for (const el of document.querySelectorAll("td, th, label, b, strong, span")) {
    if (!/lineup\s*name/i.test(el.textContent ?? "")) continue;
    const row = el.closest("tr");
    const input =
      row?.querySelector<HTMLInputElement>("input[type='text'], input:not([type='hidden'])") ??
      el.parentElement?.querySelector<HTMLInputElement>("input");
    if (input?.value?.trim()) return input.value.trim();
  }
  return undefined;
}

/**
 * Extracts defensive position abbreviation from a batting-order list item.
 *
 * @param li - `<li>` row in the phonetic batting list.
 * @returns Uppercase position code when matched against `POS_RE`.
 */
function positionFromLi(li: Element): string | undefined {
  const bold = li.querySelector("b");
  const t = bold?.textContent?.trim() ?? "";
  if (POS_RE.test(t)) return t.toUpperCase();
  for (const cell of li.querySelectorAll("td")) {
    const cellText = cell.textContent?.trim() ?? "";
    if (POS_RE.test(cellText)) return cellText.toUpperCase();
  }
  return undefined;
}

// --- Slot extraction ---

/**
 * Scrapes up to nine batting-order slots from the phonetic list markup.
 *
 * @param document - Edit Lineup page document.
 * @returns Ordered `PageSlot` array; empty when list root is missing (see legacy).
 */
function extractBattingSlots(document: Document): PageSlot[] {
  const root = lineupListRoot(document);
  if (!root) return extractBattingSlotsLegacy(document);

  const slots: PageSlot[] = [];
  root.querySelectorAll(":scope > li").forEach((li, index) => {
    const select = li.querySelector<HTMLSelectElement>("select[name^='playerID']");
    if (!select) return;

    const opt = selectedOption(select);
    if (!opt) return;

    const parsed = parseRosterOption(opt.textContent?.trim() ?? "");
    const name = parsed?.name ?? cleanPlayerLabel(opt.textContent?.trim() ?? "");
    if (!name || name.toLowerCase().includes("select") || name.startsWith("---")) return;

    slots.push({
      order: index + 1,
      playerName: name,
      position: positionFromLi(li),
    });
  });

  return slots.slice(0, 9);
}

/**
 * Fallback slot scraper for older table-row lineup markup.
 *
 * @param document - Edit Lineup page document.
 * @returns Up to nine deduplicated slots sorted by batting order.
 */
function extractBattingSlotsLegacy(document: Document): PageSlot[] {
  const slots: PageSlot[] = [];
  document.querySelectorAll("tr").forEach((row) => {
    const cells = row.querySelectorAll("td");
    if (cells.length < 2) return;
    const orderNum = parseInt(cells[0]?.textContent?.trim() ?? "", 10);
    if (!Number.isFinite(orderNum) || orderNum < 1 || orderNum > 9) return;
    const playerSelect = row.querySelector<HTMLSelectElement>("select");
    if (!playerSelect) return;
    const opt = selectedOption(playerSelect);
    if (!opt) return;
    const parsed = parseRosterOption(opt.textContent?.trim() ?? "");
    const name = parsed?.name ?? cleanPlayerLabel(opt.textContent?.trim() ?? "");
    if (!name) return;
    slots.push({ order: orderNum, playerName: name, position: positionFromLi(row) });
  });
  const byOrder = new Map(slots.map((s) => [s.order, s]));
  return [...byOrder.values()].sort((a, b) => a.order - b.order).slice(0, 9);
}

/**
 * Collects unique roster names and position eligibility from all lineup selects.
 *
 * @param document - Edit Lineup page document.
 * @returns Deduped player names and per-name position arrays for the optimizer pool.
 */
function extractRosterPool(document: Document): { names: string[]; eligibility: Record<string, string[]> } {
  const names = new Set<string>();
  const eligibility: Record<string, string[]> = {};
  const scopes = [
    ...document.querySelectorAll<HTMLSelectElement>("form[name='lineup_form'] select[name^='playerID']"),
    ...document.querySelectorAll<HTMLSelectElement>("ul#phoneticlong select"),
  ];

  const seen = new Set<HTMLSelectElement>();
  for (const sel of scopes) {
    if (seen.has(sel)) continue;
    seen.add(sel);
    sel.querySelectorAll("option").forEach((opt) => {
      if (!opt.value) return;
      const parsed = parseRosterOption(opt.textContent?.trim() ?? "");
      if (!parsed) return;
      names.add(parsed.name);
      if (parsed.positions.length) eligibility[parsed.name] = parsed.positions;
    });
  }

  return { names: [...names], eligibility };
}

/**
 * Scrapes batter rating snippets from `table.stat_table` blocks on the lineup page.
 *
 * @param document - Edit Lineup page document.
 * @returns Map of player name → OBP/RC display string.
 */
function extractBatterRatings(document: Document): Record<string, string> {
  const ratings: Record<string, string> = {};
  document.querySelectorAll("table.stat_table").forEach((table) => {
    let headers: string[] = [];
    table.querySelectorAll("tr").forEach((tr) => {
      const cells = [...tr.querySelectorAll("td, th")].map((c) => c.textContent?.trim() ?? "");
      if (!cells.length) return;
      const lower = cells.map((c) => c.toLowerCase());
      if (lower.includes("player") || lower.includes("obp")) {
        headers = lower;
        return;
      }
      if (!headers.length) return;
      const pi = headers.indexOf("player");
      const oi = headers.findIndex((h) => h === "obp" || h === "rc");
      if (pi < 0 || pi >= cells.length) return;
      const player = cleanPlayerLabel(cells[pi]);
      if (!player || player.length < 3) return;
      ratings[player] = oi >= 0 ? cells[oi] : cells.slice(1, 4).join("/");
    });
  });
  return ratings;
}

// --- Adapter export ---

export const lineupAdapter: PageAdapter = {
  pageType: "lineup",

  /**
   * @param url - Current page URL.
   * @returns True when pathname includes `/manage/edit_lineup`.
   */
  matches(url: URL): boolean {
    return url.pathname.includes("/manage/edit_lineup");
  },

  /**
   * Builds full `PageContext` for the Edit Lineup screen.
   *
   * @param document - Live DOM.
   * @param url - Current location (for `curTeam`, `lineupID` query params).
   * @returns Structured context with slots, pool JSON, and pitcher side in `extra`.
   */
  extract(document: Document, url: URL): PageContext {
    const params = url.searchParams;
    const curTeam = params.get("curTeam") ?? undefined;
    const lineupId = params.get("lineupID") ?? undefined;

    const lineupName = extractLineupName(document);
    const slots = extractBattingSlots(document);
    const { names: rosterPool, eligibility } = extractRosterPool(document);
    const batterRatings = extractBatterRatings(document);
    const pitcherSide = detectPitcherSide(lineupName);

    return {
      pageType: "lineup",
      url: url.href,
      lineupName,
      curTeam,
      slots,
      extra: {
        title: document.title,
        lineupId: lineupId ?? "",
        pitcherSide,
        rosterCount: String(rosterPool.length),
        rosterPool: JSON.stringify(rosterPool),
        positionEligibility: JSON.stringify(eligibility),
        batterRatings: JSON.stringify(batterRatings),
        slotCount: String(slots.length),
      },
    };
  },
};
