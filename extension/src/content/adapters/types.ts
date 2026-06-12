/**
 * @file Page adapter contract — shared interface for DOM scrapers.
 *
 * **Purpose:** Defines the shape each screen-specific adapter must implement so
 * `registry.ts` can dispatch uniformly without knowing markup details.
 *
 * **Message flow:** Adapters produce `PageContext`; consumed upstream by content
 * script messaging, never referenced in extension message types directly.
 *
 * **Dependencies:** `shared/types.js` (`PageContext`).
 */
import type { PageContext } from "../../shared/types.js";

/**
 * Contract for a single ImagineSports screen scraper.
 *
 * Implementations are registered in `registry.ts` and selected by `matches()`.
 */
export interface PageAdapter {
  /** Stable page type key (e.g. `"lineup"`, `"roster"`). */
  readonly pageType: string;

  /**
   * @param url - Parsed current page URL.
   * @returns Whether this adapter should handle the page.
   */
  matches(url: URL): boolean;

  /**
   * @param document - Live DOM in the content script isolated world.
   * @param url - Parsed current page URL (query params, pathname).
   * @returns Structured context for API and side panel consumption.
   */
  extract(document: Document, url: URL): PageContext;
}
