# Manual: Load Chrome extension (Mac)

## Build

```bash
cd extension
npm install
npm run build
```

## Load

1. Open Chrome → `chrome://extensions`
2. Enable **Developer mode** (top right)
3. **Load unpacked** → select the `extension/` folder in your clone (contains `manifest.json`)
4. Pin **DMB Sidecar** to toolbar

## Configure

1. Right-click extension icon → **Options** (or open `options.html`)
2. API URL: `http://127.0.0.1:5280`
3. API Key: `dev-key-change-me` (match `appsettings.json`)

## Test

1. Start backend: `./scripts/start-dev.sh`
2. Log into [ImagineSports](https://www.imaginesports.com/bball/)
3. Open **Edit Lineup** for your team
4. Click extension icon → side panel opens
5. Click **Explain this screen**

## Troubleshooting

| Symptom | Fix |
|---------|-----|
| Empty lineup slots | Update selectors in `extension/src/content/adapters/lineup.ts` — use DevTools |
| API 401 | Check API key in options matches `Security:ApiKey` |
| CORS / fetch failed | Ensure API running on 5280; check `host_permissions` in manifest |
| Foundry error in answer | Complete FOUNDRY_IQ_PORTAL.md; run `az login` |
