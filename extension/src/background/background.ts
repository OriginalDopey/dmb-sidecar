import type { AdviseRequest, LineupAnalyzeResponse, PageContext, SidecarMessage } from "../shared/types.js";
import { loadSettings } from "../shared/config.js";

let latestContext: PageContext | null = null;
let latestTabId: number | null = null;

const IS_HOST_RE = /imaginesports\.com/i;

chrome.sidePanel
  .setPanelBehavior({ openPanelOnActionClick: true })
  .catch(console.error);

chrome.runtime.onMessage.addListener((message: SidecarMessage, sender, sendResponse) => {
  if (message.type === "PAGE_CONTEXT" || message.type === "CONTEXT_UPDATE") {
    latestContext = message.context;
    if (sender.tab?.id) latestTabId = sender.tab.id;
    chrome.runtime.sendMessage({ type: "CONTEXT_UPDATE", context: message.context }).catch(() => {});
    return false;
  }

  if (message.type === "REFRESH_CONTEXT") {
    refreshContextFromActiveTab()
      .then((context) => sendResponse({ type: "CONTEXT_UPDATE", context }))
      .catch((err) => sendResponse({ type: "ADVISE_ERROR", error: String(err) }));
    return true;
  }

  if (message.type === "ADVISE") {
    handleAdvise(message.question, sender.tab?.id)
      .then((response) => sendResponse(response))
      .catch((err) => sendResponse({ type: "ADVISE_ERROR", error: String(err) }));
    return true;
  }

  if (message.type === "LINEUP_ANALYZE") {
    handleLineupAnalyze()
      .then((response) => sendResponse(response))
      .catch((err) => sendResponse({ type: "ADVISE_ERROR", error: String(err) }));
    return true;
  }

  if (message.type === "LINEUP_EXPLAIN") {
    handleLineupExplain(message.question, message.lineup)
      .then((response) => sendResponse(response))
      .catch((err) => sendResponse({ type: "ADVISE_ERROR", error: String(err) }));
    return true;
  }

  return false;
});

async function getActiveTab(): Promise<chrome.tabs.Tab> {
  const [tab] = await chrome.tabs.query({ active: true, currentWindow: true });
  if (!tab?.id) throw new Error("No active tab — focus an ImagineSports window first.");
  return tab;
}

function isImagineSportsTab(tab: chrome.tabs.Tab): boolean {
  return Boolean(tab.url && IS_HOST_RE.test(tab.url) && tab.url.includes("/bball/"));
}

async function readContextFromTab(tabId: number): Promise<PageContext> {
  return new Promise((resolve, reject) => {
    chrome.tabs.sendMessage(tabId, { type: "GET_PAGE_CONTEXT" } as SidecarMessage, (ctx) => {
      if (chrome.runtime.lastError) {
        reject(new Error(chrome.runtime.lastError.message));
        return;
      }
      resolve(ctx as PageContext);
    });
  });
}

async function ensureContentScript(tabId: number): Promise<void> {
  try {
    await readContextFromTab(tabId);
  } catch {
    await chrome.scripting.executeScript({
      target: { tabId },
      files: ["dist/content.js"],
    });
  }
}

async function refreshContextFromActiveTab(): Promise<PageContext> {
  const tab = await getActiveTab();
  if (!isImagineSportsTab(tab)) {
    throw new Error(
      "Active tab is not ImagineSports. Open Edit Lineup or Roster, then reopen the side panel."
    );
  }

  await ensureContentScript(tab.id!);
  const context = await readContextFromTab(tab.id!);
  latestContext = context;
  latestTabId = tab.id!;
  chrome.runtime.sendMessage({ type: "CONTEXT_UPDATE", context }).catch(() => {});
  return context;
}

async function getContextForAdvise(tabIdFromSender?: number): Promise<PageContext> {
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

async function handleAdvise(question: string, tabId?: number): Promise<SidecarMessage> {
  const settings = await loadSettings();
  const context = await getContextForAdvise(tabId);

  if (
    context.pageType === "lineup" ||
    context.url.includes("/manage/edit_lineup")
  ) {
    return handleLineupExplain(question, undefined);
  }

  const body: AdviseRequest = { question, context };

  const res = await fetch(`${settings.apiUrl.replace(/\/$/, "")}/advise`, {
    method: "POST",
    headers: {
      "Content-Type": "application/json",
      "X-Api-Key": settings.apiKey,
    },
    body: JSON.stringify(body),
  });

  if (!res.ok) {
    const text = await res.text();
    throw new Error(`API ${res.status}: ${text}`);
  }

  const response = await res.json();
  return { type: "ADVISE_RESULT", response };
}

async function handleLineupAnalyze(_tabId?: number): Promise<SidecarMessage> {
  const settings = await loadSettings();
  const context = await refreshContextFromActiveTab();

  const res = await fetch(`${settings.apiUrl.replace(/\/$/, "")}/lineup/analyze`, {
    method: "POST",
    headers: {
      "Content-Type": "application/json",
      "X-Api-Key": settings.apiKey,
    },
    body: JSON.stringify(context),
  });

  if (!res.ok) {
    const text = await res.text();
    throw new Error(`API ${res.status}: ${text}`);
  }

  const response = await res.json();
  return { type: "LINEUP_RESULT", response, context };
}

async function handleLineupExplain(
  question: string,
  lineup?: LineupAnalyzeResponse
): Promise<SidecarMessage> {
  const settings = await loadSettings();
  const context = await refreshContextFromActiveTab();

  const res = await fetch(`${settings.apiUrl.replace(/\/$/, "")}/lineup/explain`, {
    method: "POST",
    headers: {
      "Content-Type": "application/json",
      "X-Api-Key": settings.apiKey,
    },
    body: JSON.stringify({ question, context, lineup: lineup ?? null }),
  });

  if (!res.ok) {
    const text = await res.text();
    throw new Error(`API ${res.status}: ${text}`);
  }

  const response = await res.json();
  return { type: "ADVISE_RESULT", response };
}
