/**
 * @file Extension settings — sidecar API endpoint and authentication.
 *
 * **Purpose:** Loads user-configurable API URL and key from `chrome.storage.sync`,
 * falling back to local development defaults.
 *
 * **Message flow:** Read by `background.ts` before each HTTP call to the sidecar
 * server; not part of extension IPC.
 *
 * **Dependencies:** Chrome `storage.sync` API.
 */

// --- Defaults ---

/** Local sidecar server URL when nothing is stored. */
export const DEFAULT_API_URL = "http://127.0.0.1:5280";

/** Development API key placeholder; override via extension options storage. */
export const DEFAULT_API_KEY = "dev-key-change-me";

/** Resolved settings passed to fetch handlers. */
export interface SidecarSettings {
  apiUrl: string;
  apiKey: string;
}

// --- Storage ---

/**
 * Loads API connection settings from synced extension storage.
 *
 * @returns `apiUrl` and `apiKey`, using defaults when keys are unset.
 */
export async function loadSettings(): Promise<SidecarSettings> {
  const stored = await chrome.storage.sync.get(["apiUrl", "apiKey"]);
  return {
    apiUrl: (stored.apiUrl as string) || DEFAULT_API_URL,
    apiKey: (stored.apiKey as string) || DEFAULT_API_KEY,
  };
}
