# DMB Sidecar — Chrome Extension

## Build

```bash
npm install --cache /tmp/npm-cache-dmb-sidecar   # if ~/.npm permission errors
npm run build
```

Output: `dist/background.js`, `dist/content.js`, `dist/sidepanel.js`

## Load

Chrome → `chrome://extensions` → Developer mode → **Load unpacked** → this `extension/` folder.

See [../docs/manual-steps/CHROME_LOAD_UNPACKED.md](../docs/manual-steps/CHROME_LOAD_UNPACKED.md).

## Architecture

- `src/content/adapters/` — PageAdapter per IS screen
- `src/background/background.ts` — API client + message broker
- `src/sidepanel/` — UI
