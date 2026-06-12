/**
 * @file Service worker — message hub between content script, side panel, and sidecar API.
 *
 * **Purpose:** Caches the latest `PageContext`, proxies advise/lineup requests to the
 * local sidecar server, and ensures the content script is injected when needed.
 *
 * **Message flow:**
 * - From content: `PAGE_CONTEXT` → rebroadcast as `CONTEXT_UPDATE` to side panel
 * - From side panel: `REFRESH_CONTEXT`, `ADVISE`, `LINEUP_ANALYZE`, `LINEUP_EXPLAIN`
 * - To content: `GET_PAGE_CONTEXT` (on-demand scrape)
 * - To API: `POST /advise`, `POST /lineup/analyze`, `POST /lineup/explain`
 *
 * **Dependencies:** `shared/types.js`, `shared/config.js`, Chrome extension APIs
 * (`runtime`, `tabs`, `scripting`, `sidePanel`, `storage`).
 */
import type { AdviseRequest, LineupAnalyzeResponse, PageContext, SidecarMessage } from "../shared/types.js";
import { loadSettings } from "../shared/config.js";

// --- Cached state ---

/** Most recently received page context from any ImagineSports tab. */
let latestContext: PageContext | null = null;

/** Tab ID that produced `latestContext`; used to avoid stale cross-tab reads. */
let latestTabId: number | null = null;

/** Host pattern for ImagineSports DMB pages. */
const IS_HOST_RE = /imaginesports\.com/i;

// --- Extension setup ---

chrome.sidePanel
  .setPanelBehavior({ openPanelOnActionClick: true })
  .catch(console.error);

// --- Message routing ---

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

// --- Tab & content-script helpers ---

/**
 * Returns the active tab in the current browser window.
 *
 * @returns Active tab with a valid `id`.
 * @throws When no focused tab exists (user must focus an ImagineSports window).
 */
async function getActiveTab(): Promise<chrome.tabs.Tab> {
  const [tab] = await chrome.tabs.query({ active: true, currentWindow: true });
  if (!tab?.id) throw new Error("No active tab — focus an ImagineSports window first.");
  return tab;
}

/**
 * Checks whether a tab URL is an ImagineSports baseball management page.
 *
 * @param tab - Chrome tab to inspect.
 * @returns True when host matches and path includes `/bball/`.
 */
function isImagineSportsTab(tab: chrome.tabs.Tab): boolean {
  return Boolean(tab.url && IS_HOST_RE.test(tab.url) && tab.url.includes("/bball/"));
}

/**
 * Requests a fresh `PageContext` from the content script via `GET_PAGE_CONTEXT`.
 *
 * @param tabId - Target tab hosting the IS page.
 * @returns Parsed page context from the content adapter.
 * @throws When the content script is unreachable or returns an error.
 */
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

/**
 * Ensures the bundled content script is loaded in the target tab.
 *
 * Probes with `readContextFromTab`; on failure, injects `dist/content.js` once.
 *
 * @param tabId - Tab to inject into when content script is missing.
 */
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

/**
 * Reads context from the active tab, updates cache, and broadcasts to the side panel.
 *
 * @returns Fresh `PageContext` from the active ImagineSports tab.
 * @throws When active tab is not ImagineSports or scrape fails.
 */
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

/**
 * Resolves context for advise calls, preferring cache when sender tab matches.
 *
 * Falls back to `refreshContextFromActiveTab`, then stale `latestContext` on error.
 *
 * @param tabIdFromSender - Optional tab ID from the message sender.
 * @returns Page context for API request body.
 */
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

// --- API handlers ---

/**
 * Handles generic screen Q&A via `POST /advise`.
 *
 * Lineup pages are redirected to `handleLineupExplain` without hitting `/advise`.
 *
 * @param question - User question from side panel.
 * @param tabId - Sender tab ID for context cache lookup.
 * @returns `ADVISE_RESULT` or lineup explain result.
 */
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

/**
 * Runs lineup optimization via `POST /lineup/analyze` with fresh page context.
 *
 * @param _tabId - Reserved; context always refreshed from active tab.
 * @returns `LINEUP_RESULT` including context snapshot used for the request.
 */
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

/**
 * Explains lineup recommendations via `POST /lineup/explain`.
 *
 * @param question - Natural-language explain prompt.
 * @param lineup - Optional cached analyze response; server may re-analyze if omitted.
 * @returns `ADVISE_RESULT` with routed `questionKind` when applicable.
 */
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
