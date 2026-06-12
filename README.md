# DMB Sidecar

**Foundry IQ-grounded Chrome copilot for [Diamond Mind Baseball](https://www.imaginesports.com) (ImagineSports).**

A side panel reads the IS screen you're on, grounds advice in **Microsoft Foundry IQ** (your rules/strategy docs), and enriches with **live league data** via [dmb-mcp-server](https://github.com/OriginalDopey/dmb-mcp-server).

> **Not affiliated with ImagineSports.** Personal assistant for Classic Standard league owners.

Built for **Microsoft Build Agents League** (Reasoning Agents + IQ tools) and as an interview portfolio piece (agentic LOB overlay pattern).

## Architecture

```
Chrome Extension (TS)  →  ASP.NET Core 8 API (C#)  →  Foundry Agent + IQ
                              ↓
                        MCP HTTP Bridge (Python)  →  SQLite league cache
```

See [ARCHITECTURE.md](ARCHITECTURE.md) and [docs/TEACHING_GUIDE.md](docs/TEACHING_GUIDE.md).

## Quick start

### 1. Prerequisites

- macOS + Chrome
- .NET 8 SDK (`brew install dotnet@8`)
- Node 18+, Python 3.11+
- Azure CLI (`az login`) + Foundry project
- Existing [dmb-mcp-server](https://github.com/OriginalDopey/dmb-mcp-server) + DiamondMind `.is_session`

### 2. Configure

```bash
cp .env.local.example .env.local
# Edit DMB_ENTRY_TEAM_ID and paths
./scripts/sync-iq-sources.sh
```

Complete [docs/manual-steps/FOUNDRY_IQ_PORTAL.md](docs/manual-steps/FOUNDRY_IQ_PORTAL.md) — create `dmb-front-office` agent.

### 3. Run backend

```bash
chmod +x scripts/start-dev.sh
./scripts/start-dev.sh
```

API: http://127.0.0.1:5280/swagger  
Bridge: http://127.0.0.1:8765/health

### 4. Load extension

```bash
cd extension && npm install && npm run build
```

[Load unpacked in Chrome](docs/manual-steps/CHROME_LOAD_UNPACKED.md) → open IS lineup → click extension icon.

## API

| Endpoint | Auth | Purpose |
|----------|------|---------|
| `GET /health` | None | Status |
| `POST /foundry/smoke` | `X-Api-Key` | Test Foundry agent |
| `POST /advise` | `X-Api-Key` | Page context + question → answer |

Default API key: `dev-key-change-me` (change in `appsettings.json` + extension options).

## Build journal

[docs/BUILD_JOURNAL.md](docs/BUILD_JOURNAL.md) — what Cursor/Composer built vs manual steps (for interviews).

## Related

- [dmb-mcp-server](https://github.com/OriginalDopey/dmb-mcp-server) — league data MCP tools
- Agents League submission: Reasoning Agents track

## License

MIT — see [LICENSE](LICENSE).
