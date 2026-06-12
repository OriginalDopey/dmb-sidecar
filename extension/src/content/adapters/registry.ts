/**
 * @file Page adapter registry — URL-to-scraper dispatch for ImagineSports screens.
 *
 * **Purpose:** Selects the first matching `PageAdapter` for the current URL and
 * delegates DOM extraction. Returns a minimal unknown context when no adapter matches.
 *
 * **Message flow:** Called by `content.ts` on publish and `GET_PAGE_CONTEXT`; output
 * flows as `PageContext` through `PAGE_CONTEXT` / `CONTEXT_UPDATE` messages.
 *
 * **Dependencies:** `adapters/lineup.js`, `adapters/roster.js`, `adapters/types.js`,
 * `shared/types.js`.
 */
import type { PageAdapter } from "./types.js";
import { lineupAdapter } from "./lineup.js";
import { rosterAdapter } from "./roster.js";

// --- Registry ---

/** Ordered adapter list; first `matches()` win is used. */
const adapters: PageAdapter[] = [lineupAdapter, rosterAdapter];

/**
 * Finds the adapter responsible for a given URL.
 *
 * @param url - Parsed page location.
 * @returns Matching adapter or `null` when the page is unsupported.
 */
export function resolveAdapter(url: URL): PageAdapter | null {
  return adapters.find((a) => a.matches(url)) ?? null;
}

/**
 * Extracts structured page context for any ImagineSports URL.
 *
 * @param document - Live DOM in the content script world.
 * @param href - Full page URL string (parsed internally).
 * @returns `PageContext` from the matched adapter, or `{ pageType: "unknown", ... }`.
 */
export function extractPageContext(document: Document, href: string) {
  const url = new URL(href);
  const adapter = resolveAdapter(url);
  if (!adapter) {
    return {
      pageType: "unknown",
      url: href,
      extra: { title: document.title },
    };
  }
  return adapter.extract(document, url);
}
