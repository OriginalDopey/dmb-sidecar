# DMB Sidecar

[![CI](https://github.com/OriginalDopey/dmb-sidecar/actions/workflows/ci.yml/badge.svg)](https://github.com/OriginalDopey/dmb-sidecar/actions/workflows/ci.yml)

**Foundry IQ-grounded Chrome copilot for [Diamond Mind Baseball](https://www.imaginesports.com) (ImagineSports).**

A side panel reads the IS screen you're on, grounds advice in **Microsoft Foundry IQ** (your rules/strategy docs), and enriches with **live league data** via [dmb-mcp-server](https://github.com/OriginalDopey/dmb-mcp-server).

> **Not affiliated with ImagineSports.** Personal assistant for Classic Standard league owners.

---

## Documentation

| Audience | Document | What you'll learn |
|----------|----------|-------------------|
| **End user** | [docs/USER_GUIDE.md](docs/USER_GUIDE.md) | Install, Lineup Lab, prompt chips, troubleshooting |
| **Developer** | [docs/CODEBASE_MAP.md](docs/CODEBASE_MAP.md) | What every folder and service does |
| **Quality / CI** | [docs/QUALITY.md](docs/QUALITY.md) | Coverage gates, how to read reports, SBOM |
| **Architecture** | [ARCHITECTURE.md](ARCHITECTURE.md) | System diagram, security boundaries, ADRs |
| **Demo** | [docs/DEMO_SCRIPT.md](docs/DEMO_SCRIPT.md) | 5-minute interview recording script |
| **Contributing** | [CONTRIBUTING.md](CONTRIBUTING.md) | PR workflow, adding explain question types |

---

## What it does (30 seconds)

```
Chrome side panel  →  reads IS page DOM (lineup, roster)
        ↓
ASP.NET API :5280  →  routes questions, calls Foundry or offline handlers
        ↓
Python bridge :8765  →  lineup optimization (RC+def), MCP league cache
```

**Lineup Lab** (Edit Lineup): optimize vs LHP/RHP, side-by-side grid, typed explain ("why not Cobb at DH?", "why Knight over Mackanin at SS?").

**General Ask** (any screen): roster review, rules Q&A, MCP-enriched advice when Foundry is live.

---

## Quick start

```bash
cp .env.local.example .env.local   # set DMB_ENTRY_TEAM_ID
./scripts/sync-iq-sources.sh
./scripts/start-dev.sh             # API :5280, bridge :8765

cd extension && npm install && npm run build
# Chrome → Load unpacked → extension/
```

Open ImagineSports **Edit Lineup** → extension icon → **Optimize this lineup**.

Details: [docs/USER_GUIDE.md](docs/USER_GUIDE.md) · [docs/SETUP.md](docs/SETUP.md)

---

## API

| Endpoint | Auth | Purpose |
|----------|------|---------|
| `GET /health` | None | Liveness + Foundry/MCP flags |
| `POST /lineup/analyze` | `X-Api-Key` | Current vs recommended lineup |
| `POST /lineup/explain` | `X-Api-Key` | Typed explain (`questionKind` in response) |
| `POST /advise` | `X-Api-Key` | General page Q&A |
| `POST /foundry/smoke` | `X-Api-Key` | Foundry connectivity test |

Swagger (dev): http://127.0.0.1:5280/swagger

---

## Quality & coverage

```bash
./scripts/ci.sh                  # full local CI mirror
./scripts/coverage-report.sh     # HTML report → coveragereport/index.html
dotnet test dmb-sidecar.sln      # 49 unit/integration tests
pytest tests/python              # lineup_engine (≥80% gate)
cd extension && npm test         # vitest (≥90% on format helpers)
```

**CI** uploads coverage to each run's **Summary** tab and **Artifacts** (`coverage-dotnet-html`, `coverage-python`, `coverage-extension`).

See [docs/QUALITY.md](docs/QUALITY.md) for gates and what's in / out of scope.

---

## Repository

[github.com/OriginalDopey/dmb-sidecar](https://github.com/OriginalDopey/dmb-sidecar)

## License

MIT — see [LICENSE](LICENSE).
