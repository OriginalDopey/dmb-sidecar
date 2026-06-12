const DEFAULT_API_URL = "http://127.0.0.1:5280";
const DEFAULT_API_KEY = "dev-key-change-me";

document.addEventListener("DOMContentLoaded", async () => {
  const stored = await chrome.storage.sync.get(["apiUrl", "apiKey"]);
  document.getElementById("apiUrl").value = stored.apiUrl || DEFAULT_API_URL;
  document.getElementById("apiKey").value = stored.apiKey || DEFAULT_API_KEY;
});

document.getElementById("save").addEventListener("click", async () => {
  const apiUrl = document.getElementById("apiUrl").value.trim() || DEFAULT_API_URL;
  const apiKey = document.getElementById("apiKey").value.trim() || DEFAULT_API_KEY;
  await chrome.storage.sync.set({ apiUrl, apiKey });
  document.getElementById("saved").textContent = "Saved.";
});
