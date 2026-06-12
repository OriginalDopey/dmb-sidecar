# DMB Sidecar — Chrome Extension

Chrome MV3 side panel for ImagineSports. See [docs/USER_GUIDE.md](../docs/USER_GUIDE.md) for end-user instructions.

## Build & test

```bash
npm install --cache /tmp/npm-cache-dmb-sidecar   # if ~/.npm permission errors
npm run build
npm test              # vitest + coverage (lineup-format.ts ≥90%)
```

Output: `dist/background.js`, `dist/content.js`, `dist/sidepanel.js`

## Load in Chrome

`chrome://extensions` → Developer mode → **Load unpacked** → this `extension/` folder.

[../docs/manual-steps/CHROME_LOAD_UNPACKED.md](../docs/manual-steps/CHROME_LOAD_UNPACKED.md)

## Module map

| Path | Role |
|------|------|
| `src/content/adapters/lineup.ts` | Edit Lineup DOM → slots |
| `src/content/adapters/roster.ts` | Roster page → batters/pitchers/IR |
| `src/background/background.ts` | API client (`/advise`, `/lineup/*`) |
| `src/sidepanel/sidepanel.ts` | Lineup Lab UI, prompt chips, explain |
| `src/shared/lineup-format.ts` | Grid number formatting (tested) |

Full map: [docs/CODEBASE_MAP.md](../docs/CODEBASE_MAP.md)
