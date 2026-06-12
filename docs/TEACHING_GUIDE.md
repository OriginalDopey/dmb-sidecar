# Teaching Guide — How to explain DMB Sidecar in an interview

Use this to **teach** the architecture, not just demo it.

## The one-sentence pitch

> A Chrome side panel reads the web app you're already using, a .NET API orchestrates a Foundry IQ-grounded agent, and a Python tool layer supplies live league data — read-only, cited, auditable.

## The three questions enterprises ask

1. **What can the model see?** Page context (DOM), IQ docs (rules), MCP cache (standings/roster) — never the whole internet.
2. **What can it do?** Advise only — no auto-submit on ImagineSports.
3. **How do you trust it?** Citations, API key, secrets server-side, BUILD_JOURNAL for traceability.

## Layer-by-layer (whiteboard order)

### 1. Browser extension (TypeScript)
- **Page adapters** = strategy pattern per URL (`lineup`, `roster`)
- Content script extracts **structured JSON**, not HTML dumps
- Service worker **brokers messages** (side panel cannot talk to content script directly — Chrome security)

**Teach:** "This is how you'd overlay Copilot on iManage or any LOB portal."

### 2. API (C# ASP.NET Core 8)
- **Single job:** assemble prompt, call Foundry, return response
- `ApiKeyMiddleware` — extension never holds Azure credentials
- `AdviseService` — merges page context + MCP snapshot + user question

**Teach:** "In a .NET shop, this is your integration layer — controllers stay thin."

### 3. Foundry IQ (Azure portal)
- Knowledge base = your **rules corpus** (markdown you wrote)
- Agent = model + instructions + IQ attachment
- C# calls **Responses API** with `agent_reference` — agent config lives in portal

**Teach:** "IQ replaces bespoke RAG pipelines — one SLA-backed retrieval layer."

### 4. MCP bridge (Python)
- Reuses `dmb-mcp-server` SQLite cache
- HTTP for hackathon speed; same data as MCP `query` / `report` tools

**Teach:** "Tool servers stay in whatever language they're already in."

## Data flow (walk through live)

```
User on IS lineup page
  → content.ts extracts slots
  → side panel "Explain this screen"
  → background.ts POST /advise
  → AdviseService fetches team_snapshot from :8765
  → FoundryAgentService POST agent Responses endpoint
  → Answer + citations back to panel
```

## Speed-to-POC story (Roboyo)

- Unfamiliar: C# API, Foundry IQ portal, Chrome MV3
- Familiar: MCP pattern, agent instructions, presales demos
- **Proof:** BUILD_JOURNAL + public repo + 19 IQ docs synced in one script

## C# honesty (Ropes)

> "I used Cursor to scaffold C# syntax; I owned architecture, security boundaries, and the Foundry integration I validated in Python first."

## Files to open while teaching

| File | What it shows |
|------|---------------|
| `extension/src/content/adapters/lineup.ts` | DOM → JSON |
| `src/DmbSidecar.Api/Services/AdviseService.cs` | Orchestration |
| `src/DmbSidecar.Api/Services/FoundryAgentService.cs` | Azure auth + Responses API |
| `src/DmbSidecar.McpBridge/app.py` | Tool layer |
| `docs/decisions/ADR-002-*.md` | Why we didn't use .NET 10 |
