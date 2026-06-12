# Manual steps only you can do

Everything else is automated in the repo. Complete these in order.

## 1. Innovation Studio activation (5 min)

**There is no public login URL** — access only comes from the activation email.

1. Search all folders for **`InnoStudio-noreply@microsoft.com`** (also Spam, Promotions, Gmail “Updates”).
2. You should also have a **registration confirmation** from Microsoft Azure Team with a link to the “Contest Website” — try that link; it may prompt profile activation.
3. If nothing after 24h, email **`hackathonsupport@microsoft.com`**:

   **Subject:** `Agents League — Innovation Studio activation not received`

   **Body:** Registered email, registration date, confirmation that you completed [Agents League registration](https://info.microsoft.com/Agents-League-Hackathon-Registration.html), and that the InnoStudio activation email never arrived. Ask them to resend the profile activation link.

4. After activation: complete hackathon profile → confirm **Projects** / **Create Project** is visible.

**You can keep building without Innovation Studio** — local stack + offline Lineup Lab work fine. Innovation Studio is only needed for **final submission** (repo + video + description).

## 2. Foundry IQ + agent (~60 min) — REQUIRED for full agent answers

**Blocked as of 2026-06-12:** `skillfestfoundry-resource` returns **404** (`Subdomain does not map to a resource`). The Skillsfest subscription has **no** Cognitive Services / Foundry resource in Azure CLI. You must **create or open a Foundry project** in [ai.azure.com](https://ai.azure.com) and paste the new **Project endpoint** into `appsettings.json` → `Foundry:ProjectEndpoint`.

Until then, `/advise` still works with **MCP live data + local `iq-sources/` search** (offline mode).

- [ ] Follow [manual-steps/FOUNDRY_IQ_PORTAL.md](manual-steps/FOUNDRY_IQ_PORTAL.md)
- [ ] Playground must answer: *"What is in-season release salary recovery?"* → **75%**
- [ ] Then run: `dotnet user-secrets set "Foundry:AgentName" "dmb-front-office" --project src/DmbSidecar.Api`  
  OR edit `appsettings.Development.json` AgentName to `dmb-front-office`

## 3. Chrome extension (10 min)
- [ ] `chrome://extensions` → Developer mode → **Load unpacked**
- [ ] Folder: `/Users/originaldopey/Documents/CursonProjects/dmb-sidecar/extension`
- [ ] Log into ImagineSports → **Edit Lineup** → click extension → **Explain this screen**
- [ ] If lineup slots empty: DevTools → update `extension/src/content/adapters/lineup.ts` → `cd extension && npm run build` → Reload extension

## 4. Demo video + submit (Sat/Sun)
- [ ] Record per [DEMO_SCRIPT.md](DEMO_SCRIPT.md)
- [ ] `gh repo edit OriginalDopey/dmb-sidecar --visibility public` (required for judges)
- [ ] Innovation Studio → **Create Project** → paste repo URL + video + [HACKATHON_SUBMISSION.md](HACKATHON_SUBMISSION.md) description

---

**Already done for you (2026-06-12):**
- `.zshrc` fixed (dotnet 8 + git)
- `.env.local` created (Perfectest Plan team `B3Z6J4FGKR4Y37366NEE`)
- `az account set` → Skillsfest
- IS session valid; league data paths wired

**Start stack anytime:**
```bash
cd /Users/originaldopey/Documents/CursonProjects/dmb-sidecar
./scripts/start-dev.sh
```
