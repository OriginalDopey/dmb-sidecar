// src/shared/config.ts
var DEFAULT_API_URL = "http://127.0.0.1:5280";
var DEFAULT_API_KEY = "dev-key-change-me";
async function loadSettings() {
  const stored = await chrome.storage.sync.get(["apiUrl", "apiKey"]);
  return {
    apiUrl: stored.apiUrl || DEFAULT_API_URL,
    apiKey: stored.apiKey || DEFAULT_API_KEY
  };
}

// src/background/background.ts
var latestContext = null;
var latestTabId = null;
var IS_HOST_RE = /imaginesports\.com/i;
chrome.sidePanel.setPanelBehavior({ openPanelOnActionClick: true }).catch(console.error);
chrome.runtime.onMessage.addListener((message, sender, sendResponse) => {
  if (message.type === "PAGE_CONTEXT" || message.type === "CONTEXT_UPDATE") {
    latestContext = message.context;
    if (sender.tab?.id) latestTabId = sender.tab.id;
    chrome.runtime.sendMessage({ type: "CONTEXT_UPDATE", context: message.context }).catch(() => {
    });
    return false;
  }
  if (message.type === "REFRESH_CONTEXT") {
    refreshContextFromActiveTab().then((context) => sendResponse({ type: "CONTEXT_UPDATE", context })).catch((err) => sendResponse({ type: "ADVISE_ERROR", error: String(err) }));
    return true;
  }
  if (message.type === "ADVISE") {
    handleAdvise(message.question, sender.tab?.id).then((response) => sendResponse(response)).catch((err) => sendResponse({ type: "ADVISE_ERROR", error: String(err) }));
    return true;
  }
  if (message.type === "LINEUP_ANALYZE") {
    handleLineupAnalyze().then((response) => sendResponse(response)).catch((err) => sendResponse({ type: "ADVISE_ERROR", error: String(err) }));
    return true;
  }
  if (message.type === "LINEUP_EXPLAIN") {
    handleLineupExplain(message.question, message.lineup).then((response) => sendResponse(response)).catch((err) => sendResponse({ type: "ADVISE_ERROR", error: String(err) }));
    return true;
  }
  return false;
});
async function getActiveTab() {
  const [tab] = await chrome.tabs.query({ active: true, currentWindow: true });
  if (!tab?.id) throw new Error("No active tab \u2014 focus an ImagineSports window first.");
  return tab;
}
function isImagineSportsTab(tab) {
  return Boolean(tab.url && IS_HOST_RE.test(tab.url) && tab.url.includes("/bball/"));
}
async function readContextFromTab(tabId) {
  return new Promise((resolve, reject) => {
    chrome.tabs.sendMessage(tabId, { type: "GET_PAGE_CONTEXT" }, (ctx) => {
      if (chrome.runtime.lastError) {
        reject(new Error(chrome.runtime.lastError.message));
        return;
      }
      resolve(ctx);
    });
  });
}
async function ensureContentScript(tabId) {
  try {
    await readContextFromTab(tabId);
  } catch {
    await chrome.scripting.executeScript({
      target: { tabId },
      files: ["dist/content.js"]
    });
  }
}
async function refreshContextFromActiveTab() {
  const tab = await getActiveTab();
  if (!isImagineSportsTab(tab)) {
    throw new Error(
      "Active tab is not ImagineSports. Open Edit Lineup or Roster, then reopen the side panel."
    );
  }
  await ensureContentScript(tab.id);
  const context = await readContextFromTab(tab.id);
  latestContext = context;
  latestTabId = tab.id;
  chrome.runtime.sendMessage({ type: "CONTEXT_UPDATE", context }).catch(() => {
  });
  return context;
}
async function getContextForAdvise(tabIdFromSender) {
  if (tabIdFromSender && latestContext && latestTabId === tabIdFromSender) {
    return latestContext;
  }
  try {
    return await refreshContextFromActiveTab();
  } catch (err) {
    if (latestContext) return latestContext;
    throw err;
  }
}
async function handleAdvise(question, tabId) {
  const settings = await loadSettings();
  const context = await getContextForAdvise(tabId);
  if (context.pageType === "lineup" || context.url.includes("/manage/edit_lineup")) {
    return handleLineupExplain(question, void 0);
  }
  const body = { question, context };
  const res = await fetch(`${settings.apiUrl.replace(/\/$/, "")}/advise`, {
    method: "POST",
    headers: {
      "Content-Type": "application/json",
      "X-Api-Key": settings.apiKey
    },
    body: JSON.stringify(body)
  });
  if (!res.ok) {
    const text = await res.text();
    throw new Error(`API ${res.status}: ${text}`);
  }
  const response = await res.json();
  return { type: "ADVISE_RESULT", response };
}
async function handleLineupAnalyze(_tabId) {
  const settings = await loadSettings();
  const context = await refreshContextFromActiveTab();
  const res = await fetch(`${settings.apiUrl.replace(/\/$/, "")}/lineup/analyze`, {
    method: "POST",
    headers: {
      "Content-Type": "application/json",
      "X-Api-Key": settings.apiKey
    },
    body: JSON.stringify(context)
  });
  if (!res.ok) {
    const text = await res.text();
    throw new Error(`API ${res.status}: ${text}`);
  }
  const response = await res.json();
  return { type: "LINEUP_RESULT", response, context };
}
async function handleLineupExplain(question, lineup) {
  const settings = await loadSettings();
  const context = await refreshContextFromActiveTab();
  const res = await fetch(`${settings.apiUrl.replace(/\/$/, "")}/lineup/explain`, {
    method: "POST",
    headers: {
      "Content-Type": "application/json",
      "X-Api-Key": settings.apiKey
    },
    body: JSON.stringify({ question, context, lineup: lineup ?? null })
  });
  if (!res.ok) {
    const text = await res.text();
    throw new Error(`API ${res.status}: ${text}`);
  }
  const response = await res.json();
  return { type: "ADVISE_RESULT", response };
}
//# sourceMappingURL=background.js.map
