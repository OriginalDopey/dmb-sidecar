# Codebase Map

Where logic lives and how data flows. For architecture diagrams see [ARCHITECTURE.md](../ARCHITECTURE.md).

---

## Repository layout

```
dmb-sidecar/
├── extension/          Chrome MV3 side panel + content scripts
├── src/DmbSidecar.Api/ ASP.NET Core 8 — auth, orchestration, explain routing
├── src/DmbSidecar.McpBridge/  FastAPI — lineup engine + MCP data HTTP
├── tests/              xUnit, pytest, vitest
├── data/player-pool/   Offline CSVs for Lineup Lab (IS SearchResults export)
├── iq-sources/         Markdown corpus (Foundry IQ + local fallback search)
├── scripts/            Dev stack, CI mirror, sync helpers
└── docs/               User guide, quality gates, ADRs
```

---

## Chrome extension (`extension/`)

| Path | Responsibility |
|------|----------------|
| `src/content/adapters/lineup.ts` | Scrapes Edit Lineup DOM (`#phoneticlong li`) → `PageContext.slots` |
| `src/content/adapters/roster.ts` | Scrapes roster sections (batters, pitchers, IR) |
| `src/content/adapters/registry.ts` | URL → adapter dispatch |
| `src/background/background.ts` | Message broker; `fetch` to API (`/advise`, `/lineup/analyze`, `/lineup/explain`) |
| `src/sidepanel/sidepanel.ts` | Lineup Lab grid, prompt chips, explain rendering |
| `src/shared/lineup-format.ts` | OBP/OPS/RC/600 display helpers (tested) |
| `src/shared/types.ts` | Shared TS contracts matching API models |

**Message flow:** Side panel → `chrome.runtime.sendMessage` → background → API → response → side panel.

---

## API (`src/DmbSidecar.Api/`)

### Entry & HTTP surface

| File | Role |
|------|------|
| `Program.cs` | DI wiring, CORS, minimal API endpoints |
| `Middleware/ApiKeyMiddleware.cs` | `X-Api-Key` on all routes except `/health`, `/swagger` |

### Endpoints

| Route | Service | Behavior |
|-------|---------|----------|
| `GET /health` | `McpBridgeClient`, `FoundryAgentService` | Liveness + dependency flags |
| `POST /advise` | `AdviseService` | General screen Q&A (Foundry → offline fallback) |
| `POST /lineup/analyze` | `LineupAnalyzeService` | Current vs recommended lineup + grid stats |
| `POST /lineup/explain` | `LineupExplainService` | Classify question → handler prose |
| `POST /foundry/smoke` | `FoundryAgentService` | One-shot Foundry connectivity test |

### Services

| File | Role |
|------|------|
| `LineupAnalyzeService.cs` | Calls MCP bridge `/lineup/analyze`; maps JSON to `LineupAnalyzeResponse` |
| `LineupExplainService.cs` | Classifies question; Foundry or `LineupExplainRouter` offline answer |
| `LineupExplain/LineupQuestionClassifier.cs` | Regex + entity extraction → `LineupQuestionKind` |
| `LineupExplain/LineupExplainRouter.cs` | Dispatches to typed handlers |
| `LineupExplain/Handlers/*.cs` | One handler per question family (DH, batting order, position, …) |
| `AdviseService.cs` | Builds Foundry prompt with MCP snapshot + page context |
| `OfflineAdviseHelper.cs` | Offline answer when Foundry fails |
| `OfflineRosterReview.cs` | Structured roster review for "explain this screen" on roster page |
| `LocalIqService.cs` | Keyword search over `iq-sources/**/*.md` |
| `McpDataFilter.cs` | Rejects stale MCP snapshots vs browser DOM |
| `FoundryAgentService.cs` | Azure AD token + Foundry Responses API |
| `McpBridgeClient.cs` | HTTP client to Python bridge `:8765` |

### Models

| File | Role |
|------|------|
| `Models/AdviseModels.cs` | `PageContext`, `AdviseRequest/Response`, citations |
| `Models/LineupModels.cs` | Lineup analyze/explain DTOs, slot results, swaps |

---

## MCP bridge (`src/DmbSidecar.McpBridge/`)

| File | Role |
|------|------|
| `app.py` | FastAPI routes: `/health`, `/lineup/analyze`, MCP proxy endpoints |
| `lineup_engine.py` | RC+def optimal nine, platoon splits, defensive runs |
| `lineup_config.py` | Vendored `dmb_config` batting-order (`order_lineup`) |
| `vendor/dmb_config_scripts/` | Synced from DiamondMind `scripts/dmb_config/` |

**Data:** Reads `data/player-pool/*.csv` at repo root (hitters-fielding, hitters-splits).

---

## Tests (`tests/`)

| Path | Covers |
|------|--------|
| `DmbSidecar.Api.Tests/LineupExplain/` | Classifier, router, helpers, handlers |
| `DmbSidecar.Api.Tests/Integration/` | `WebApplicationFactory` — health, auth, `/lineup/explain` |
| `DmbSidecar.Api.Tests/Services/` | `McpDataFilter`, `OfflineRosterReview`, `LocalIqService` |
| `python/test_lineup_engine.py` | `_def_runs`, parsing helpers |
| `extension/src/shared/lineup-format.test.ts` | Grid number formatting |

---

## Scripts

| Script | Purpose |
|--------|---------|
| `start-dev.sh` | API + MCP bridge; syncs `dmb_config` vendor |
| `stop-dev.sh` | Kill dev ports |
| `ci.sh` | Local mirror of GitHub Actions (test + SBOM) |
| `coverage-report.sh` | HTML coverage report → `coveragereport/` |
| `sync-iq-sources.sh` | Copy IQ markdown from DiamondMind |
| `sync-player-pool.sh` | Refresh lineup CSVs |
| `verify-stack.sh` | curl smoke against running stack |
