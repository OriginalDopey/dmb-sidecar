export const DEFAULT_API_URL = "http://127.0.0.1:5280";
export const DEFAULT_API_KEY = "dev-key-change-me";

export interface SidecarSettings {
  apiUrl: string;
  apiKey: string;
}

export async function loadSettings(): Promise<SidecarSettings> {
  const stored = await chrome.storage.sync.get(["apiUrl", "apiKey"]);
  return {
    apiUrl: (stored.apiUrl as string) || DEFAULT_API_URL,
    apiKey: (stored.apiKey as string) || DEFAULT_API_KEY,
  };
}
