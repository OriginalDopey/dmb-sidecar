# DMB Sidecar — User Guide

**Who this is for:** Classic Standard Diamond Mind Baseball owners using ImagineSports in Chrome.

**What it does:** A Chrome side panel that reads the IS page you already have open and answers front-office questions — lineup optimization, roster balance, and (when configured) Foundry-grounded strategy advice.

> Not affiliated with ImagineSports.

---

## Install (one time)

1. Run the backend: `./scripts/start-dev.sh` (API `:5280`, MCP bridge `:8765`).
2. Build the extension: `cd extension && npm install && npm run build`.
3. Chrome → `chrome://extensions` → **Developer mode** → **Load unpacked** → select the `extension/` folder.
4. Pin the extension; click its icon on any ImagineSports tab to open the **side panel**.

See [manual-steps/CHROME_LOAD_UNPACKED.md](manual-steps/CHROME_LOAD_UNPACKED.md) for screenshots.

---

## What you see on each screen

| ImagineSports page | Side panel mode | Primary actions |
|--------------------|-----------------|-----------------|
| **Edit Lineup** (`/manage/edit_lineup`) | **Lineup Lab** | **Optimize this lineup** · Ask / Explain |
| Roster, Bank, other manage screens | **Ask** | Type a question · **Explain this screen** |
| Any other IS page | **Ask** | General questions (rules + page context) |

The panel always shows which page was detected (lineup name, vs LHP/RHP, or roster sections).

---

## Lineup Lab (Edit Lineup)

### Optimize

1. Open **Edit Lineup** (e.g. Primary vs. LHP) with all 9 slots filled.
2. Open the side panel → **Optimize this lineup**.
3. Compare **Your lineup** vs **Recommended** in the spreadsheet grid:
   - **OBP / OPS / RC/600** — batting value for the pitcher side shown
   - **Def** — defensive runs saved (green positive, red negative)
   - **Fld** — range/error at position
   - Green cells on the recommended side = material gain in that slot

The recommendation uses the same logic as DiamondMind `generate_team_config` (RC+def positioning, implementation-plan batting order).

### Explain a recommendation

Use the text box or **prompt chips**:

| Chip | Question type | What you get |
|------|---------------|--------------|
| Main diff | Overall comparison | Top swaps and Δ RC+def |
| DH | Why not star at DH? | Defense-recovery narrative |
| Bat #4 | Batting order slot | OBP vs cleanup framing |
| At SS | Position assignment | Your slot vs model at that position |
| Knight > Mack | Head-to-head at position | Def/RC comparison |

Status line shows the routed handler, e.g. `Lineup explain · DhAssignment · 12ms`.

**Tips**

- Mention a player by **last name** (`Cobb`, `Mackanin`).
- Include a **position** (`SS`, `DH`) for assignment questions.
- Use `over` / `instead of` for two-player comparisons.

---

## General Ask (roster & other screens)

1. Navigate to the screen (e.g. **My Roster**).
2. Type a question or click **Explain this screen**.
3. Answers may cite:
   - **Foundry** — IQ-grounded agent (when Azure is configured)
   - **local-iq** — offline search over `iq-sources/` markdown
   - **mcp** — cached league snapshot from your dmb-mcp-server DB

If Foundry is unavailable, the panel shows a warning and falls back to offline handlers.

---

## Settings

Extension **Options** (right-click extension icon → Options):

| Setting | Default | Purpose |
|---------|---------|---------|
| API URL | `http://127.0.0.1:5280` | Local dev API |
| API Key | `dev-key-change-me` | Must match `appsettings.json` → `Security:ApiKey` |

---

## Troubleshooting

| Symptom | Fix |
|---------|-----|
| Panel says "Could not read page" | Refresh the IS tab; ensure you're on imaginesports.com |
| Optimize returns error | Run `./scripts/start-dev.sh`; check `data/player-pool/` CSVs exist |
| Explain is generic | Name a player + position; use prompt chips |
| 401 Unauthorized | Align API key in extension Options and API config |
| Foundry warning | Expected offline — DH/position explain still works locally |

---

## Privacy & scope

- Reads **only** the DOM of the active ImagineSports tab (lineup slots, roster table).
- Sends that JSON + your question to **your local API** (default localhost).
- Does **not** modify IS pages or submit forms — read-only overlay.
