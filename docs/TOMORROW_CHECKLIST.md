# Tomorrow morning checklist (Dave)

**Goal:** Working end-to-end demo in ~2 hours.

## Critical path (in order)

- [ ] **1. Shell env** — add to terminal or `~/.zshrc`:
  ```bash
  export DOTNET_ROOT="/opt/homebrew/opt/dotnet@8/libexec"
  export PATH="/opt/homebrew/opt/dotnet@8/bin:$PATH"
  ```

- [ ] **2. `.env.local`**
  ```bash
  cp .env.local.example .env.local
  # Set DMB_ENTRY_TEAM_ID from any IS URL (curTeam=...)
  ```

- [ ] **3. Azure login**
  ```bash
  az login
  az account set --subscription Skillsfest   # or your subscription name
  ```

- [ ] **4. Foundry portal (60 min)** — [FOUNDRY_IQ_PORTAL.md](manual-steps/FOUNDRY_IQ_PORTAL.md)
  - Upload `iq-sources/*.md`
  - Create agent `dmb-front-office`
  - Update `appsettings.Development.json` AgentName

- [ ] **5. Start stack**
  ```bash
  ./scripts/start-dev.sh
  ```

- [ ] **6. Smoke tests**
  ```bash
  curl http://127.0.0.1:5280/health
  curl http://127.0.0.1:8765/health
  curl -X POST http://127.0.0.1:5280/foundry/smoke -H "X-Api-Key: dev-key-change-me"
  ```

- [ ] **7. Chrome extension** — [CHROME_LOAD_UNPACKED.md](manual-steps/CHROME_LOAD_UNPACKED.md)
  ```bash
  cd extension && npm run build
  ```

- [ ] **8. Live IS test**
  - Log into ImagineSports
  - Open Edit Lineup
  - Side panel → **Explain this screen**
  - If slots empty → DevTools → update `lineup.ts` selectors → journal T11

- [ ] **9. GitHub**
  ```bash
  git init && git add . && git commit -m "DMB Sidecar: Foundry IQ Chrome copilot for Agents League"
  gh repo create dmb-sidecar --public --source=. --push
  ```

- [ ] **10. Demo video** — [DEMO_SCRIPT.md](DEMO_SCRIPT.md) → YouTube → hackathon submit

## If something breaks

| Issue | Doc |
|-------|-----|
| Foundry 401/403 | `az login`, check subscription |
| MCP bridge session invalid | Re-auth dmb-mcp-server cookie |
| Empty lineup | BUILD_JOURNAL T11 — DOM selectors |
| npm EACCES | `npm install --cache /tmp/npm-cache-dmb-sidecar` |

## Interview prep (after demo works)

- [ ] Read [TEACHING_GUIDE.md](TEACHING_GUIDE.md) once out loud
- [ ] [ROPES_INTERVIEW.md](ROPES_INTERVIEW.md) — Mon 3 PM
- [ ] [ROBOYO_INTERVIEW.md](ROBOYO_INTERVIEW.md) — Tue 10 AM
