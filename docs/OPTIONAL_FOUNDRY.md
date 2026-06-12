# Optional: Microsoft Foundry setup

Lineup Lab and offline explain work **without** Foundry. Configure Foundry only for full agent Q&A on roster/bank screens.

## Steps

1. Copy `src/DmbSidecar.Api/appsettings.Development.json.example` → `appsettings.Development.json` (gitignored if local).
2. Set `Foundry:ProjectEndpoint` to your Azure AI project URL.
3. Create agent `dmb-front-office` and upload `iq-sources/` per [manual-steps/FOUNDRY_IQ_PORTAL.md](manual-steps/FOUNDRY_IQ_PORTAL.md).
4. `az login` then verify: `curl -X POST http://127.0.0.1:5280/foundry/smoke -H "X-Api-Key: dev-key-change-me"`

When `ProjectEndpoint` is empty, `/advise` uses offline IQ search and roster review handlers.
