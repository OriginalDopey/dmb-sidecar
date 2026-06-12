import type { PageAdapter } from "./types.js";
import type { PageContext, PageSlot } from "../../shared/types.js";

/**
 * Edit Lineup page — selectors from write_ops_spike.md + typical IS manage forms.
 * TODO (Dave): verify selectors in DevTools; update BUILD_JOURNAL T11.
 */
export const lineupAdapter: PageAdapter = {
  pageType: "lineup",

  matches(url: URL): boolean {
    return url.pathname.includes("/manage/edit_lineup");
  },

  extract(document: Document, url: URL): PageContext {
    const params = url.searchParams;
    const curTeam = params.get("curTeam") ?? undefined;

    // Lineup name dropdown — often first select in form or named lineup_id
    let lineupName: string | undefined;
    const lineupSelect =
      document.querySelector<HTMLSelectElement>('select[name*="lineup" i]') ??
      document.querySelector<HTMLSelectElement>("form select");
    if (lineupSelect?.selectedOptions[0]) {
      lineupName = lineupSelect.selectedOptions[0].text.trim();
    }

    const slots: PageSlot[] = [];

    // IS uses numbered batting slots — try table rows or ordered selects
    const rows = document.querySelectorAll("table tr, .lineup-row, form tr");
    rows.forEach((row, idx) => {
      const selects = row.querySelectorAll("select");
      if (selects.length === 0) return;

      const playerSelect = selects[0] as HTMLSelectElement;
      const posSelect = selects.length > 1 ? (selects[1] as HTMLSelectElement) : null;
      const playerOpt = playerSelect.selectedOptions[0];
      if (!playerOpt || !playerOpt.text.trim()) return;

      const name = playerOpt.text.trim();
      if (name.toLowerCase().includes("select")) return;

      slots.push({
        order: slots.length + 1,
        playerName: name,
        position: posSelect?.selectedOptions[0]?.text.trim(),
      });
    });

    // Fallback: all player selects in form order
    if (slots.length === 0) {
      document.querySelectorAll<HTMLSelectElement>("form select").forEach((sel, i) => {
        const opt = sel.selectedOptions[0];
        const text = opt?.text?.trim() ?? "";
        if (!text || text.length < 2 || text.toLowerCase().includes("select")) return;
        // Skip lineup header select if it's the only short one
        if (i === 0 && text.includes("vs.")) {
          lineupName = lineupName ?? text;
          return;
        }
        slots.push({ order: slots.length + 1, playerName: text });
      });
    }

    return {
      pageType: "lineup",
      url: url.href,
      lineupName,
      curTeam,
      slots,
      extra: {
        title: document.title,
        slotCount: String(slots.length),
      },
    };
  },
};
