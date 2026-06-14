# Front Office IQ — System Overview

> **Grounded AI for a legacy web app: DOM context → ASP.NET → Foundry IQ → MCP league data.**

---

## What This Is

Front Office IQ is a **Chrome extension + backend system** that adds AI-powered decision support to [Diamond Mind Baseball](https://imaginesports.com/bball), a legacy simulation baseball management game with no API.

The system demonstrates how to retrofit intelligent features onto any legacy web application using a layered architecture that respects the host site's boundaries while providing enterprise-grade AI assistance.

---

## Architecture

```
┌─────────────────────────────────────────────────────────────┐
│  Chrome Extension (MV3 Side Panel)                          │
│  • Content script observes DOM → extracts page context      │
│  • Side panel renders lineup analysis + advisor chat        │
│  • Communicates with API via X-Api-Key authenticated HTTP   │
└─────────────────────┬───────────────────────────────────────┘
                      │ HTTP (localhost:5280)
┌─────────────────────▼───────────────────────────────────────┐
│  ASP.NET Core 8 API (DmbSidecar.Api)                        │
│  • /lineup/analyze — positional optimization engine         │
│  • /lineup/explain — typed question routing + handlers      │
│  • /advise — general Foundry-grounded Q&A                   │
│  • /foundry/smoke — diagnostic probe                        │
│  • /health — readiness check                                │
│  • ApiKeyMiddleware — authenticates all requests            │
└──────────┬──────────────────────────────┬───────────────────┘
           │                              │
           ▼                              ▼
┌──────────────────────┐    ┌─────────────────────────────────┐
│  Python MCP Bridge   │    │  Microsoft Foundry Agent         │
│  (FastAPI :8765)     │    │  (Azure AI Foundry)              │
│  • Lineup engine     │    │  • KB: 17 DMB rules/strategy     │
│  • Cached league     │    │    docs indexed in Azure Search  │
│  •   data via MCP    │    │  • Agent: dmb-front-office v2    │
│  • Park factors      │    │  • Model: gpt-4.1-mini           │
└──────────────────────┘    │  • Grounded answers + citations  │
                            └─────────────────────────────────┘
```

---

## Key Design Decisions

### 1. DOM-Driven Context (No API Required)

The legacy site has no API. A content script (`content.ts`) observes the DOM, extracts structured context (lineup names, player data, pitcher handedness), and passes it to the side panel. This pattern works for **any** legacy web app.

### 2. Typed Explain System

Instead of sending every question to the LLM (expensive, slow), the `/lineup/explain` endpoint classifies questions into known categories (DH assignment, batting order, position comparison, recommendation summary) and routes to specialized handlers that produce deterministic, instant answers. Only fallback/unknown questions go to Foundry.

### 3. Offline-First with IQ Fallback

The `LocalIqService` provides offline keyword search against the same documents in the KB. When Foundry is unreachable (no internet, no Azure creds), the system gracefully degrades to local search.

### 4. Foundry IQ Knowledge Grounding

The Foundry agent is connected to a **Foundry IQ knowledge base** backed by Azure AI Search. This ensures:
- Answers cite specific rules documents
- No hallucination of game mechanics
- Source attribution in every response

### 5. MCP Bridge for Live Data

The Python MCP bridge wraps the `dmb-mcp-server` (a separate project) to provide cached league data — standings, rosters, transactions. The ASP.NET API calls this bridge for lineup optimization context.

---

## Technology Stack

| Layer | Technology | Purpose |
|-------|-----------|---------|
| Extension | TypeScript, Chrome MV3 | DOM observation, user interface |
| API | ASP.NET Core 8, C# | Request routing, auth, orchestration |
| AI | Microsoft Foundry, Azure AI Search | Grounded Q&A, knowledge retrieval |
| Data | Python FastAPI, dmb-mcp-server | League data, lineup engine |
| CI/CD | GitHub Actions | Test gates, SBOM, coverage |
| Quality | Git hooks, Cursor skill | Pre-commit checks, standard enforcement |

---

## Security Model

- **API Key Auth**: All extension → API traffic requires `X-Api-Key` header
- **CORS**: Restricted to `chrome-extension://` and localhost origins
- **Azure AD**: Foundry calls use bearer tokens (Azure CLI credential)
- **No PII**: System never stores user credentials; session tokens stay in `.env.local`
- **SBOM**: CycloneDX bill-of-materials generated in CI

---

## Running Locally

```bash
# Prerequisites: .NET 8 SDK, Node 18+, Python 3.11+, Azure CLI (logged in)

# 1. Clone and install hooks
git clone https://github.com/OriginalDopey/dmb-sidecar.git
cd dmb-sidecar && ./scripts/install-git-hooks.sh

# 2. Start the API
dotnet run --project src/DmbSidecar.Api --environment Development

# 3. (Optional) Start MCP bridge
cd src/DmbSidecar.McpBridge && python -m uvicorn app:app --port 8765

# 4. Load extension in Chrome
# chrome://extensions → Developer mode → Load unpacked → extension/dist
```

---

## Test Coverage

| Component | Framework | Coverage Target |
|-----------|-----------|----------------|
| .NET API | xUnit | 55%+ |
| Python Bridge | pytest | 80%+ |
| Extension | vitest | 90%+ |

Run all: `dotnet test && cd src/DmbSidecar.McpBridge && pytest && cd ../../extension && npx vitest run`

---

## Project Structure

```
dmb-sidecar/
├── src/
│   ├── DmbSidecar.Api/           # ASP.NET Core 8 backend
│   │   ├── Configuration/        # Typed options (Foundry, MCP, Security)
│   │   ├── Middleware/            # API key auth
│   │   ├── Models/                # Request/response DTOs
│   │   └── Services/             # Business logic
│   │       ├── FoundryAgentService.cs   # Foundry Responses API client
│   │       ├── LocalIqService.cs        # Offline KB search
│   │       ├── LineupAnalyzeService.cs  # Position optimizer
│   │       ├── LineupExplainService.cs  # Question router
│   │       └── McpBridgeClient.cs       # Python bridge HTTP client
│   └── DmbSidecar.McpBridge/     # Python FastAPI data bridge
├── extension/                     # Chrome MV3 extension
│   └── src/
│       ├── background/            # Service worker
│       ├── content/               # DOM observer
│       ├── sidepanel/             # Main UI
│       ├── adapters/              # Page-specific data extractors
│       └── shared/                # Types, config, formatters
├── tests/                         # xUnit integration + unit tests
├── docs/                          # Setup guides, demo script, teaching guide
├── iq-sources/                    # Markdown docs uploaded to Foundry IQ KB
├── scripts/                       # Dev tooling, hooks, deployment
└── .github/                       # CI workflows, PR templates, CODEOWNERS
```

---

## Foundry IQ Integration Details

| Component | Value |
|-----------|-------|
| Project | `front-office-iq` |
| Resource | `front-office-iq-resource` (East US 2) |
| Agent | `dmb-front-office` v2 |
| Model | gpt-4.1-mini |
| KB | `dmb-rules-kb` (17 documents) |
| Search Service | `dmb-classic-standard-kb` (Standard S1, West US 2) |
| Auth | Azure AD → `https://ai.azure.com` audience |
| API Version | `2025-11-15-preview` |
| Endpoint Pattern | `{project}/agents/{agent}/endpoint/protocols/openai/responses` |

---

## Enterprise Patterns Demonstrated

1. **Typed configuration** — `IOptions<T>` pattern with validation
2. **Structured logging** — Consistent `ILogger<T>` with message templates
3. **HTTP client factory** — Named/typed `HttpClient` via DI
4. **Health checks** — `/health` endpoint for orchestrator readiness
5. **Middleware pipeline** — Auth enforcement via custom middleware
6. **CORS policy** — Origin-allowlist (not wildcard)
7. **CI/CD gates** — Build → test → coverage → SBOM on every PR
8. **Git hooks** — Pre-commit secret scanning, pre-push branch protection
9. **ADRs** — Architecture Decision Records in `ARCHITECTURE.md`
10. **SBOM** — CycloneDX software bill-of-materials in CI artifacts
