import type { PageAdapter } from "./types.js";
import type { PageContext, PageSlot } from "../../shared/types.js";

const SECTIONS = ["batter", "pitcher", "ir"] as const;

function parseStatTables(document: Document): PageSlot[] {
  const players: PageSlot[] = [];
  const tables = document.querySelectorAll("table.stat_table");

  tables.forEach((table, tableIdx) => {
    const section = SECTIONS[tableIdx] ?? "unknown";
    let colMap: string[] | null = null;

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
        position: posIdx >= 0 ? cells[posIdx] : undefined,
        playerName,
        salary: salaryIdx >= 0 ? cells[salaryIdx] : undefined,
        section,
      });
    });
  });

  return players;
}

function scrapeFinanceExtra(document: Document): Record<string, string> {
  const text = document.body?.innerText ?? "";
  const extra: Record<string, string> = { title: document.title };

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

/**
 * Team roster page — IS uses three table.stat_table blocks (hitters, pitchers, IR).
 */
export const rosterAdapter: PageAdapter = {
  pageType: "roster",

  matches(url: URL): boolean {
    return url.pathname.includes("/team/roster");
  },

  extract(document: Document, url: URL): PageContext {
    const curTeam = url.searchParams.get("curTeam") ?? undefined;
    const slots = parseStatTables(document);
    const extra = scrapeFinanceExtra(document);
    extra.playerCount = String(slots.length);

    return {
      pageType: "roster",
      url: url.href,
      curTeam,
      slots,
      extra,
    };
  },
};
