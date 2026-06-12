import type { AdviseRequest, PageContext, SidecarMessage } from "../shared/types.js";
import { loadSettings } from "../shared/config.js";

let latestContext: PageContext | null = null;

chrome.sidePanel
  .setPanelBehavior({ openPanelOnActionClick: true })
  .catch(console.error);

chrome.runtime.onMessage.addListener((message: SidecarMessage, sender, sendResponse) => {
  if (message.type === "PAGE_CONTEXT" || message.type === "CONTEXT_UPDATE") {
    latestContext = message.context;
    chrome.runtime.sendMessage({ type: "CONTEXT_UPDATE", context: message.context }).catch(() => {});
    return false;
  }

  if (message.type === "ADVISE") {
    handleAdvise(message.question, sender.tab?.id)
      .then((response) => sendResponse(response))
      .catch((err) => sendResponse({ type: "ADVISE_ERROR", error: String(err) }));
    return true;
  }

  return false;
});

async function getContextFromTab(tabId?: number): Promise<PageContext> {
  if (latestContext) return latestContext;
  if (!tabId) throw new Error("No active tab");
  return new Promise((resolve, reject) => {
    chrome.tabs.sendMessage(tabId, { type: "GET_PAGE_CONTEXT" } as SidecarMessage, (ctx) => {
      if (chrome.runtime.lastError) reject(new Error(chrome.runtime.lastError.message));
      else resolve(ctx as PageContext);
    });
  });
}

async function handleAdvise(question: string, tabId?: number): Promise<SidecarMessage> {
  const settings = await loadSettings();
  const context = await getContextFromTab(tabId);
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
