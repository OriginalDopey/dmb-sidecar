/**
 * @file Content script — DOM bridge on ImagineSports pages.
 *
 * **Purpose:** Scrapes page context via registered adapters and publishes updates
 * when the user navigates. Responds to on-demand context requests from the
 * service worker.
 *
 * **Message flow:**
 * - Outbound: `PAGE_CONTEXT` (on load and URL change) → background
 * - Inbound: `GET_PAGE_CONTEXT` → synchronous `sendResponse` with `PageContext`
 *
 * **Dependencies:** `adapters/registry.js` (`extractPageContext`), `shared/types.js`.
 */
import { extractPageContext } from "./adapters/registry.js";
import type { SidecarMessage } from "../shared/types.js";

// --- Context publishing ---

/**
 * Extracts current page context and sends `PAGE_CONTEXT` to the service worker.
 *
 * Failures are swallowed when the side panel or worker is not yet listening.
 */
function publishContext(): void {
  const context = extractPageContext(document, window.location.href);
  const msg: SidecarMessage = { type: "PAGE_CONTEXT", context };
  chrome.runtime.sendMessage(msg).catch(() => {
    // side panel / service worker may not be ready
  });
}

// --- Messaging ---

chrome.runtime.onMessage.addListener((message: SidecarMessage, _sender, sendResponse) => {
  if (message.type === "GET_PAGE_CONTEXT") {
    sendResponse(extractPageContext(document, window.location.href));
    return true;
  }
  return false;
});

// --- Navigation detection ---

publishContext();
let lastUrl = location.href;
new MutationObserver(() => {
  if (location.href !== lastUrl) {
    lastUrl = location.href;
    publishContext();
  }
}).observe(document.body, { childList: true, subtree: true });
