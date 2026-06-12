import { extractPageContext } from "./adapters/registry.js";
import type { SidecarMessage } from "../shared/types.js";

function publishContext(): void {
  const context = extractPageContext(document, window.location.href);
  const msg: SidecarMessage = { type: "PAGE_CONTEXT", context };
  chrome.runtime.sendMessage(msg).catch(() => {
    // side panel / service worker may not be ready
  });
}

chrome.runtime.onMessage.addListener((message: SidecarMessage, _sender, sendResponse) => {
  if (message.type === "GET_PAGE_CONTEXT") {
    sendResponse(extractPageContext(document, window.location.href));
    return true;
  }
  return false;
});

// Initial + SPA-ish navigation
publishContext();
let lastUrl = location.href;
new MutationObserver(() => {
  if (location.href !== lastUrl) {
    lastUrl = location.href;
    publishContext();
  }
}).observe(document.body, { childList: true, subtree: true });
