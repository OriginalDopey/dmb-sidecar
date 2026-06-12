# Quality & Coverage

Enterprise expectations for this repo: every merge to `main` must pass CI, publish coverage artifacts, and keep critical paths tested.

---

## CI pipeline

**Workflow:** [`.github/workflows/ci.yml`](../.github/workflows/ci.yml)

| Job | Gate | Artifacts |
|-----|------|-----------|
| .NET API | `dotnet test` (49 tests) | `coverage-dotnet` + **`coverage-dotnet-html`** |
| MCP bridge | `pytest` + **80%** line coverage on `lineup_engine` | `sbom-python` |
| Extension | `vitest` + **90%** on `lineup-format.ts` | `sbom-extension` |

**View results:** GitHub → **Actions** → latest **CI** run → **Summary** (coverage %) and **Artifacts** (download HTML/XML).

---

## Coverage targets

| Layer | Metric | Minimum | Stretch |
|-------|--------|---------|---------|
| .NET `DmbSidecar.Api` | Line | **55%** | 70% |
| `lineup_engine.py` | Line | **80%** | 90% |
| `lineup-format.ts` | Line | **90%** | 100% |

**Critical paths (must stay tested):**

- `LineupExplain/*` — classifier, router, all six handlers
- `McpDataFilter` — stale cache rejection
- `lineup_engine._def_runs` — SS/2B error penalty
- API integration — `/health`, `/lineup/explain` auth + routing
- Extension formatting — OBP/OPS/RC/600 display

**Explicitly out of scope for unit coverage** (integration / manual only):

- `FoundryAgentService` live Azure calls
- `McpBridgeClient` against real SQLite (use `verify-stack.sh`)
- ImagineSports DOM adapters (manual on real pages)

---

## Local commands

```bash
# Full CI mirror
./scripts/ci.sh

# .NET tests only
dotnet test dmb-sidecar.sln -c Release

# HTML coverage report (opens in browser after)
./scripts/coverage-report.sh

# Python coverage
pytest tests/python --cov=lineup_engine --cov-report=term-missing

# Extension coverage
cd extension && npm test -- --coverage
```

After `coverage-report.sh`, open `coveragereport/index.html`.

---

## Code documentation standards

| Area | Standard |
|------|----------|
| Public API types | XML `///` summary on records and service public methods |
| Question routing | `LineupQuestionKind` enum documents each handler family |
| Handlers | File-level comment: purpose + example user question |
| Extension adapters | Comment block: DOM selectors and IS URL pattern |
| Security | [SECURITY.md](../SECURITY.md) |

---

## SBOM & dependencies

Every CI run produces CycloneDX SBOMs per stack. Download from Actions artifacts or run `./scripts/ci.sh` locally (`sbom/`).

Dependabot (`.github/dependabot.yml`) opens weekly update PRs for NuGet, npm, pip, and Actions.

---

## Before demo / interview

1. `./scripts/ci.sh` — all green locally
2. `./scripts/start-dev.sh` + reload extension
3. Skim [USER_GUIDE.md](USER_GUIDE.md) and [DEMO_SCRIPT.md](DEMO_SCRIPT.md)
4. Optional: download latest `coverage-dotnet` artifact to show audit trail
