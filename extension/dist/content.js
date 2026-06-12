// src/content/adapters/lineup.ts
var POS_RE = /^(DH|C|1B|2B|3B|SS|LF|CF|RF)$/i;
function detectPitcherSide(lineupName) {
  const n = (lineupName || "").toLowerCase();
  if (n.includes("vs lhp") || n.includes("v. lhp") || n.includes("vs. lhp")) return "lhp";
  return "rhp";
}
function cleanPlayerLabel(text) {
  return text.replace(/\s*-\s*\([^)]+\)\s*$/, "").replace(/\s*[*#✻]\s*$/u, "").replace(/^<INJ>\s*/i, "").replace(/\s+INJ.*$/i, "").trim();
}
function parseRosterOption(text) {
  const raw = (text || "").trim();
  const elig = raw.match(/\(([^)]+)\)\s*$/);
  const positions = elig ? elig[1].split(",").map((p) => p.trim()).filter(Boolean) : [];
  const name = cleanPlayerLabel(raw.replace(/\s*-\s*\([^)]+\)\s*$/, ""));
  if (name.length > 3 && !name.toLowerCase().includes("select") && name.includes(",")) {
    return { name, positions };
  }
  return null;
}
function selectedOption(select) {
  const marked = select.querySelector("option[selected], option[SELECTED]");
  if (marked instanceof HTMLOptionElement && marked.value) return marked;
  const sel = select.selectedOptions[0];
  if (sel?.value) return sel;
  return null;
}
function lineupListRoot(document2) {
  return document2.querySelector("ul#phoneticlong") ?? document2.querySelector("form[name='lineup_form'] ul.boxy") ?? document2.querySelector("ul.boxy");
}
function extractLineupName(document2) {
  const named = document2.querySelector("input[name='lineup_name']");
  if (named?.value?.trim()) return named.value.trim();
  for (const el of document2.querySelectorAll("td, th, label, b, strong, span")) {
    if (!/lineup\s*name/i.test(el.textContent ?? "")) continue;
    const row = el.closest("tr");
    const input = row?.querySelector("input[type='text'], input:not([type='hidden'])") ?? el.parentElement?.querySelector("input");
    if (input?.value?.trim()) return input.value.trim();
  }
  return void 0;
}
function positionFromLi(li) {
  const bold = li.querySelector("b");
  const t = bold?.textContent?.trim() ?? "";
  if (POS_RE.test(t)) return t.toUpperCase();
  for (const cell of li.querySelectorAll("td")) {
    const cellText = cell.textContent?.trim() ?? "";
    if (POS_RE.test(cellText)) return cellText.toUpperCase();
  }
  return void 0;
}
function extractBattingSlots(document2) {
  const root = lineupListRoot(document2);
  if (!root) return extractBattingSlotsLegacy(document2);
  const slots = [];
  root.querySelectorAll(":scope > li").forEach((li, index) => {
    const select = li.querySelector("select[name^='playerID']");
    if (!select) return;
    const opt = selectedOption(select);
    if (!opt) return;
    const parsed = parseRosterOption(opt.textContent?.trim() ?? "");
    const name = parsed?.name ?? cleanPlayerLabel(opt.textContent?.trim() ?? "");
    if (!name || name.toLowerCase().includes("select") || name.startsWith("---")) return;
    slots.push({
      order: index + 1,
      playerName: name,
      position: positionFromLi(li)
    });
  });
  return slots.slice(0, 9);
}
function extractBattingSlotsLegacy(document2) {
  const slots = [];
  document2.querySelectorAll("tr").forEach((row) => {
    const cells = row.querySelectorAll("td");
    if (cells.length < 2) return;
    const orderNum = parseInt(cells[0]?.textContent?.trim() ?? "", 10);
    if (!Number.isFinite(orderNum) || orderNum < 1 || orderNum > 9) return;
    const playerSelect = row.querySelector("select");
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
function extractRosterPool(document2) {
  const names = /* @__PURE__ */ new Set();
  const eligibility = {};
  const scopes = [
    ...document2.querySelectorAll("form[name='lineup_form'] select[name^='playerID']"),
    ...document2.querySelectorAll("ul#phoneticlong select")
  ];
  const seen = /* @__PURE__ */ new Set();
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
function extractBatterRatings(document2) {
  const ratings = {};
  document2.querySelectorAll("table.stat_table").forEach((table) => {
    let headers = [];
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
var lineupAdapter = {
  pageType: "lineup",
  /**
   * @param url - Current page URL.
   * @returns True when pathname includes `/manage/edit_lineup`.
   */
  matches(url) {
    return url.pathname.includes("/manage/edit_lineup");
  },
  /**
   * Builds full `PageContext` for the Edit Lineup screen.
   *
   * @param document - Live DOM.
   * @param url - Current location (for `curTeam`, `lineupID` query params).
   * @returns Structured context with slots, pool JSON, and pitcher side in `extra`.
   */
  extract(document2, url) {
    const params = url.searchParams;
    const curTeam = params.get("curTeam") ?? void 0;
    const lineupId = params.get("lineupID") ?? void 0;
    const lineupName = extractLineupName(document2);
    const slots = extractBattingSlots(document2);
    const { names: rosterPool, eligibility } = extractRosterPool(document2);
    const batterRatings = extractBatterRatings(document2);
    const pitcherSide = detectPitcherSide(lineupName);
    return {
      pageType: "lineup",
      url: url.href,
      lineupName,
      curTeam,
      slots,
      extra: {
        title: document2.title,
        lineupId: lineupId ?? "",
        pitcherSide,
        rosterCount: String(rosterPool.length),
        rosterPool: JSON.stringify(rosterPool),
        positionEligibility: JSON.stringify(eligibility),
        batterRatings: JSON.stringify(batterRatings),
        slotCount: String(slots.length)
      }
    };
  }
};

// src/content/adapters/roster.ts
var SECTIONS = ["batter", "pitcher", "ir"];
function parseStatTables(document2) {
  const players = [];
  const tables = document2.querySelectorAll("table.stat_table");
  tables.forEach((table, tableIdx) => {
    const section = SECTIONS[tableIdx] ?? "unknown";
    let colMap = null;
    table.querySelectorAll("tr").forEach((tr) => {
      const cells = Array.from(tr.querySelectorAll("td, th")).map(
        (c) => c.textContent?.replace(/\s+/g, " ").trim() ?? ""
      );
      if (cells.length < 3) return;
      const lower = cells.map((c) => c.toLowerCase());
      if (lower.includes("player") || lower.includes("pos")) {
        colMap = lower;
        return;
      }
      if (!colMap) return;
      const posIdx = colMap.indexOf("pos");
      const playerIdx = colMap.indexOf("player");
      const salaryIdx = colMap.indexOf("salary");
      if (playerIdx < 0) return;
      const rawName = cells[playerIdx] ?? "";
      if (!rawName || rawName.toLowerCase().startsWith("total")) return;
      const playerName = rawName.replace(/\s*[✻#]\s*$/u, "").trim();
      if (!playerName || playerName.length > 50) return;
      players.push({
        order: players.length + 1,
        position: posIdx >= 0 ? cells[posIdx] : void 0,
        playerName,
        salary: salaryIdx >= 0 ? cells[salaryIdx] : void 0,
        section
      });
    });
  });
  return players;
}
function scrapeFinanceExtra(document2) {
  const text = document2.body?.innerText ?? "";
  const extra = { title: document2.title };
  const viewing = text.match(/viewing\s+(.+?)\s+in\s+(.+?)\s+League/i);
  if (viewing) {
    extra.teamName = viewing[1].trim();
    extra.leagueName = viewing[2].trim();
  }
  const total = text.match(/Total Value:\s*(\$[\d,]+)/i);
  const cash = text.match(/Cash Balance:\s*(\$[\d,]+)/i);
  const loan = text.match(/Max Loan:\s*(\$[\d,]+)/i);
  const stadium = text.match(/Stadium:\s*([^\n]+)/i);
  if (total) extra.totalValue = total[1];
  if (cash) extra.cashBalance = cash[1];
  if (loan) extra.maxLoan = loan[1];
  if (stadium) extra.stadium = stadium[1].trim();
  return extra;
}
var rosterAdapter = {
  pageType: "roster",
  /**
   * @param url - Current page URL.
   * @returns True when pathname includes `/team/roster`.
   */
  matches(url) {
    return url.pathname.includes("/team/roster");
  },
  /**
   * Builds full `PageContext` for the team roster screen.
   *
   * @param document - Live DOM.
   * @param url - Current location (for `curTeam` query param).
   * @returns Roster slots plus finance metadata in `extra`.
   */
  extract(document2, url) {
    const curTeam = url.searchParams.get("curTeam") ?? void 0;
    const slots = parseStatTables(document2);
    const extra = scrapeFinanceExtra(document2);
    extra.playerCount = String(slots.length);
    return {
      pageType: "roster",
      url: url.href,
      curTeam,
      slots,
      extra
    };
  }
};

// src/content/adapters/registry.ts
var adapters = [lineupAdapter, rosterAdapter];
function resolveAdapter(url) {
  return adapters.find((a) => a.matches(url)) ?? null;
}
function extractPageContext(document2, href) {
  const url = new URL(href);
  const adapter = resolveAdapter(url);
  if (!adapter) {
    return {
      pageType: "unknown",
      url: href,
      extra: { title: document2.title }
    };
  }
  return adapter.extract(document2, url);
}

// src/content/content.ts
function publishContext() {
  const context = extractPageContext(document, window.location.href);
  const msg = { type: "PAGE_CONTEXT", context };
  chrome.runtime.sendMessage(msg).catch(() => {
  });
}
chrome.runtime.onMessage.addListener((message, _sender, sendResponse) => {
  if (message.type === "GET_PAGE_CONTEXT") {
    sendResponse(extractPageContext(document, window.location.href));
    return true;
  }
  return false;
});
publishContext();
var lastUrl = location.href;
new MutationObserver(() => {
  if (location.href !== lastUrl) {
    lastUrl = location.href;
    publishContext();
  }
}).observe(document.body, { childList: true, subtree: true });
//# sourceMappingURL=content.js.map
