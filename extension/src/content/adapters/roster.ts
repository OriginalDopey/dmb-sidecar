import type { PageAdapter } from "./types.js";
import type { PageContext, PageSlot } from "../../shared/types.js";

/**
 * Team roster page — extracts visible roster table rows.
 */
export const rosterAdapter: PageAdapter = {
  pageType: "roster",

  matches(url: URL): boolean {
    return url.pathname.includes("/team/roster");
  },

  extract(document: Document, url: URL): PageContext {
    const curTeam = url.searchParams.get("curTeam") ?? undefined;
    const slots: PageSlot[] = [];

    document.querySelectorAll("table tr").forEach((row) => {
      const cells = row.querySelectorAll("td");
      if (cells.length < 2) return;
      const nameCell = cells[0]?.textContent?.trim() ?? "";
      if (!nameCell || nameCell === "Player" || nameCell === "Name") return;
      const pos = cells[1]?.textContent?.trim();
      const salary = cells.length > 3 ? cells[3]?.textContent?.trim() : undefined;
      slots.push({
        order: slots.length + 1,
        playerName: nameCell,
        position: pos,
        bats: salary,
      });
    });

    return {
      pageType: "roster",
      url: url.href,
      curTeam,
      slots,
      extra: {
        title: document.title,
        playerCount: String(slots.length),
      },
    };
  },
};
