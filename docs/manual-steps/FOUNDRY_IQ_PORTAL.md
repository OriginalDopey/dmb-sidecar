# Manual: Foundry IQ + dmb-front-office agent

**Time:** ~60–90 minutes  
**Prerequisite:** `az login`, Skillsfest subscription, `iq-sources/` synced (`./scripts/sync-iq-sources.sh`)

## 1. Open project

1. Go to [https://ai.azure.com](https://ai.azure.com)
2. Select subscription **Skillsfest**
3. Open project **skillfestFoundry** (or your Foundry project)

## 2. Create knowledge base

1. Navigate to **Foundry IQ** or **Knowledge** in left nav
2. **Create knowledge base** → name: `dmb-classic-standard-kb`
3. Add **File Search** knowledge source
4. Upload all `.md` files from `iq-sources/` (see `iq-sources/MANIFEST.md`)
5. Wait for indexing (note start/end time in BUILD_JOURNAL — can take 5–20 min)

**Screenshot checkpoints:** KB created, file count, indexing complete

## 3. Create agent

1. **Agents** → **Create agent**
2. Name: `dmb-front-office`
3. Model: `gpt-4.1-mini` (or gpt-4o-mini)
4. **Instructions** (paste):

```
You are DMB Sidecar — a Diamond Mind Baseball Classic Standard front-office advisor.

Rules:
- Ground game mechanics in your Foundry IQ knowledge base. Cite when stating rules (transactions, roster, tactical settings, defense, bankroll).
- When live league data is provided in the user message under "Live team snapshot" or "League summary", treat it as authoritative for facts.
- Never invent player stats or standings.
- Frame advice as asset management: RC/600, salary cap, transaction timing, playoffs.
- Refuse questions unrelated to DMB team management.
- Be concise: bullets for recommendations, 1-2 paragraphs max unless asked for detail.
```

5. **Tools / Knowledge:** Attach `dmb-classic-standard-kb`
6. **Save** → version **1**
7. Test in playground: *"What is in-season release salary recovery?"* → expect **75%**

## 4. Wire API

Edit `src/DmbSidecar.Api/appsettings.Development.json`:

```json
"Foundry": {
  "AgentName": "dmb-front-office",
  "AgentVersion": "1"
}
```

Or user secrets:
```bash
cd src/DmbSidecar.Api
dotnet user-secrets set "Foundry:AgentName" "dmb-front-office"
```

## 5. Verify

```bash
./scripts/start-dev.sh
# other terminal:
curl -X POST http://127.0.0.1:5280/foundry/smoke -H "X-Api-Key: dev-key-change-me"
```

**Journal entry:** Record T04 PASS in `docs/BUILD_JOURNAL.md` with screenshot links or notes.
