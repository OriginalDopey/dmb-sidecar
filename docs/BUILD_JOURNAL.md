# Build journal (condensed)

Engineering log for interview review — what was built, what is automated vs manual.

## Stack

| Layer | Technology | Role |
|-------|------------|------|
| Extension | TypeScript / Chrome MV3 | DOM scrape, side panel, API client |
| API | ASP.NET Core 8 | Auth, orchestration, explain routing |
| Bridge | Python FastAPI | Lineup engine, MCP data HTTP |
| IQ | Foundry (optional) + `iq-sources/` | Grounded answers |

See [ARCHITECTURE.md](../ARCHITECTURE.md) and ADRs in `docs/decisions/`.

## Key milestones

1. **Page adapters** — lineup (`#phoneticlong li`), roster sections → `PageContext`
2. **Lineup Lab** — RC+def optimal nine + `dmb_config` batting order via vendored sync
3. **Explain router** — six typed question handlers (DH, batting order, position, comparison, summary, fallback)
4. **Quality pipeline** — xUnit + pytest + vitest, CI coverage artifacts, CycloneDX SBOM
5. **Documentation** — USER_GUIDE, CODEBASE_MAP, QUALITY, CONTRIBUTING

## Human vs automated

| Automated (CI) | Manual (operator) |
|----------------|-------------------|
| `dotnet test`, pytest, vitest | `az login` for Foundry |
| Coverage gates, SBOM | Load unpacked extension in Chrome |
| `sync-dmb-config.sh` in start-dev | `.env.local` paths to sibling repos |
| Pre-commit quality gate | ImagineSports session cookie for MCP scrape |

## AI assistance

Initial scaffold and iteration used Cursor Agent. All shipped code is reviewed, tested, and documented per [QUALITY.md](QUALITY.md).
