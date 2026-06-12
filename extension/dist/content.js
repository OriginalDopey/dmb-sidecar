// src/content/adapters/lineup.ts
var lineupAdapter = {
  pageType: "lineup",
  matches(url) {
    return url.pathname.includes("/manage/edit_lineup");
  },
  extract(document2, url) {
    const params = url.searchParams;
    const curTeam = params.get("curTeam") ?? void 0;
    let lineupName;
    const lineupSelect = document2.querySelector('select[name*="lineup" i]') ?? document2.querySelector("form select");
    if (lineupSelect?.selectedOptions[0]) {
      lineupName = lineupSelect.selectedOptions[0].text.trim();
    }
    const slots = [];
    const rows = document2.querySelectorAll("table tr, .lineup-row, form tr");
    rows.forEach((row, idx) => {
      const selects = row.querySelectorAll("select");
      if (selects.length === 0) return;
      const playerSelect = selects[0];
      const posSelect = selects.length > 1 ? selects[1] : null;
      const playerOpt = playerSelect.selectedOptions[0];
      if (!playerOpt || !playerOpt.text.trim()) return;
      const name = playerOpt.text.trim();
      if (name.toLowerCase().includes("select")) return;
      slots.push({
        order: slots.length + 1,
        playerName: name,
        position: posSelect?.selectedOptions[0]?.text.trim()
      });
    });
    if (slots.length === 0) {
      document2.querySelectorAll("form select").forEach((sel, i) => {
        const opt = sel.selectedOptions[0];
        const text = opt?.text?.trim() ?? "";
        if (!text || text.length < 2 || text.toLowerCase().includes("select")) return;
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
        title: document2.title,
        slotCount: String(slots.length)
      }
    };
  }
};

// src/content/adapters/roster.ts
var rosterAdapter = {
  pageType: "roster",
  matches(url) {
    return url.pathname.includes("/team/roster");
  },
  extract(document2, url) {
    const curTeam = url.searchParams.get("curTeam") ?? void 0;
    const slots = [];
    document2.querySelectorAll("table tr").forEach((row) => {
      const cells = row.querySelectorAll("td");
      if (cells.length < 2) return;
      const nameCell = cells[0]?.textContent?.trim() ?? "";
      if (!nameCell || nameCell === "Player" || nameCell === "Name") return;
      const pos = cells[1]?.textContent?.trim();
      const salary = cells.length > 3 ? cells[3]?.textContent?.trim() : void 0;
      slots.push({
        order: slots.length + 1,
        playerName: nameCell,
        position: pos,
        bats: salary
      });
    });
    return {
      pageType: "roster",
      url: url.href,
      curTeam,
      slots,
      extra: {
        title: document2.title,
        playerCount: String(slots.length)
      }
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
