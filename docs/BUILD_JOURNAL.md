# DMB Sidecar — Build Journal

> **Purpose:** Catalog what Dave did manually vs what Cursor/Composer generated — for interviews, LinkedIn, and teaching the architecture.

**Project:** Agentic web overlay for Diamond Mind Baseball (ImagineSports)  
**Stack:** Chrome MV3 extension (TypeScript) + ASP.NET Core 8 (C#) + Foundry IQ + Python MCP bridge  
**Repo:** `dmb-sidecar` (standalone public)

---

## Journal template (use for each future task)

```markdown
## T{nn} — {title} — {date}
**Goal:**
**Owner:** Composer | Dave manual | Both
**Prompt given to Composer:**
**Composer produced:**
**Dave manual steps:**
**Verification:**
**Result:** PASS | FAIL | PARTIAL
**Decision recorded:**
**Time spent:**
**Notes for next chunk:**
```

---

## T00 — Journal + repo scaffold — 2026-06-12 (overnight)

**Goal:** Create documented foundation before feature code.  
**Owner:** Composer (overnight build session)

**Composer produced:**
- Full repo tree under `/Users/originaldopey/Documents/CursonProjects/dmb-sidecar`
- This journal, ADRs, manual-step guides, teaching guide
- `.gitignore`, `.env.local.example`, `LICENSE` (MIT)

**Dave manual steps (tomorrow):**
- Review journal entries
- Fill `.env.local` with your `DMB_ENTRY_TEAM_ID`

**Verification:** `ls dmb-sidecar/docs` shows BUILD_JOURNAL, decisions/, manual-steps/

**Result:** PASS

---

## T01 — .NET 8 SDK — 2026-06-12

**Goal:** Unblock ASP.NET Core 8 (Mac had .NET 5 only).  
**Owner:** Composer

**Composer produced:**
```bash
brew install dotnet@8
export DOTNET_ROOT="/opt/homebrew/opt/dotnet@8/libexec"
export PATH="/opt/homebrew/opt/dotnet@8/bin:$PATH"
dotnet --version  # → 8.0.128
```

**Dave manual steps:** Add to `~/.zshrc` if desired:
```bash
export DOTNET_ROOT="/opt/homebrew/opt/dotnet@8/libexec"
export PATH="/opt/homebrew/opt/dotnet@8/bin:$PATH"
```

**Result:** PASS

---

## T02 — GitHub repo structure — 2026-06-12

**Goal:** Standalone public repo layout.  
**Owner:** Composer

**Composer produced:**
```
dmb-sidecar/
├── src/DmbSidecar.Api/       # ASP.NET Core 8
├── src/DmbSidecar.McpBridge/ # FastAPI → dmb_mcp
├── extension/                # Chrome MV3
├── iq-sources/               # Foundry IQ corpus
├── scripts/
└── docs/
```

**Dave manual steps (tomorrow):**
```bash
cd /Users/originaldopey/Documents/CursonProjects/dmb-sidecar
git init
git add .
git commit -m "Initial DMB Sidecar scaffold for Agents League"
gh repo create dmb-sidecar --public --source=. --push
```

**Result:** PASS (local); push pending Dave

---

## T03 — IQ sources sync — 2026-06-12

**Goal:** Curated markdown for Foundry IQ upload.  
**Owner:** Composer

**Composer produced:** `scripts/sync-iq-sources.sh` → **19 markdown files** in `iq-sources/`

**Verification:**
```bash
./scripts/sync-iq-sources.sh
cat iq-sources/MANIFEST.md
```

**Dave manual steps:** Run T04 — upload files in Foundry portal (see `docs/manual-steps/FOUNDRY_IQ_PORTAL.md`)

**Result:** PASS

---

## T04 — Foundry IQ + agent — PENDING (Dave manual)

**Goal:** Create `dmb-front-office` agent with IQ knowledge base.  
**Owner:** Dave manual (portal)

**Status:** NOT DONE — blocking full Foundry answers until complete.

**Workaround in place:** `appsettings.Development.json` uses `computing-historian` agent until `dmb-front-office` exists. API returns offline fallback if Foundry fails.

**Dave checklist:** See `docs/manual-steps/FOUNDRY_IQ_PORTAL.md`

**Result:** PENDING

---

## T05 — Foundry smoke — PARTIAL

**Goal:** Verify Azure auth + agent endpoint from this machine.  
**Owner:** Dave manual tomorrow

**Command (after `az login`):**
```bash
curl -X POST http://127.0.0.1:5280/foundry/smoke \
  -H "X-Api-Key: dev-key-change-me"
```

**Expected:** JSON with `answer` field (may be computing-historian until T04 done).

**Result:** PENDING Dave `az login`

---

## T06 — MCP HTTP bridge — 2026-06-12

**Goal:** Expose dmb-mcp-server data over HTTP for C# API.  
**Owner:** Composer

**Composer produced:** `src/DmbSidecar.McpBridge/app.py` (FastAPI)

**Endpoints:**
| Method | Path | Purpose |
|--------|------|---------|
| GET | `/health` | Liveness |
| GET | `/config/status` | Session + DB paths |
| GET | `/roster` | Cached roster |
| GET | `/standings` | Standings |
| GET | `/report/team_snapshot` | Text summary |
| GET | `/report/league_summary` | League text |
| POST | `/scrape/refresh` | Incremental scrape |

**Verification:**
```bash
cd src/DmbSidecar.McpBridge && python3.11 -m venv .venv && source .venv/bin/activate
pip install -r requirements.txt
export PYTHONPATH=.../dmb-mcp-server/src DMB_DB_PATH=... DMB_ENTRY_TEAM_ID=...
uvicorn app:app --port 8765
curl http://127.0.0.1:8765/health
```

**Result:** PASS (if DB + session valid)

---

## T07–T09 — ASP.NET Core API — 2026-06-12

**Goal:** C# orchestration layer with `/advise`, Foundry HttpClient, MCP client.  
**Owner:** Composer

**Composer produced:**
- `FoundryAgentService` — Responses API + `DefaultAzureCredential`
- `McpBridgeClient` — HTTP to bridge
- `AdviseService` — assembles prompt (page context + MCP + question)
- `ApiKeyMiddleware` — `X-Api-Key` header
- `POST /advise`, `POST /foundry/smoke`, `GET /health`

**Verification:**
```bash
export DOTNET_ROOT=... PATH=...
cd src/DmbSidecar.Api && dotnet build
dotnet run
curl http://127.0.0.1:5280/health
```

**Result:** PASS (build)

**Overnight verification (automated):**
```text
curl http://127.0.0.1:8765/health → {"status":"ok","service":"mcp-bridge"}
curl http://127.0.0.1:5280/health → {"status":"ok","foundryConfigured":true,"mcpBridgeReachable":true,"version":"0.1.0"}
dotnet build → 0 errors
npm run build → dist/ created
```

---

## T10–T12 — Chrome extension — 2026-06-12

**Goal:** MV3 side panel + content adapters + API wire-up.  
**Owner:** Composer

**Composer produced:**
- `extension/manifest.json` (MV3, sidePanel)
- `LineupAdapter`, `RosterAdapter`
- Service worker message broker
- Side panel UI (dark theme)

**Dave manual steps:**
```bash
cd extension && npm install && npm run build
# Chrome → chrome://extensions → Load unpacked → select extension/
```

**Known gap:** Lineup DOM selectors are heuristic — verify on live IS page (T11 journal update).

**Result:** PARTIAL until live IS test

---

## T14 — Documentation package — 2026-06-12

**Owner:** Composer

**Files:** README, ARCHITECTURE, TEACHING_GUIDE, DEMO_SCRIPT, interview docs, ADRs

**Result:** PASS

---

## Overnight summary (for Dave in the morning)

| Done | Pending your action |
|------|---------------------|
| .NET 8 installed | `az login` |
| 19 IQ source files synced | T04 Foundry portal upload + agent |
| MCP bridge code | Set `DMB_ENTRY_TEAM_ID` in `.env.local` |
| Full C# API | `git init` + push to GitHub |
| Chrome extension built | Load unpacked + test on IS lineup page |
| All docs | Demo video + hackathon submit |

**Fastest path to working demo tomorrow:**
1. `cp .env.local.example .env.local` → fill team ID
2. `az login`
3. Complete T04 in Foundry portal (~60 min)
4. `./scripts/start-dev.sh`
5. Load extension → open lineup → "Explain this screen"
