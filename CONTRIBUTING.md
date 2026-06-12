# Contributing

## Prerequisites

- .NET 8 SDK, Node 20+, Python 3.11+
- Read [docs/CODEBASE_MAP.md](docs/CODEBASE_MAP.md) before changing behavior

## Workflow

1. `./scripts/install-git-hooks.sh` once per clone (runs quality gate on commit)
2. **Never push directly to `main`** — branch + PR only (see [docs/GITHUB_BEST_PRACTICES.md](docs/GITHUB_BEST_PRACTICES.md))
3. Branch from `main`: `git checkout -b feat/short-description`
4. Make focused changes; match existing style (`.editorconfig`)
5. Add or update tests for behavior you change
6. Run `./scripts/pre-commit-quality.sh` before commit (or rely on the hook)
7. Run `./scripts/ci.sh` before opening a PR
8. Open PR — all three CI jobs must pass; use **squash merge**

**Cursor:** invoke skill `dmb-sidecar-quality-gate` before commits or PRs.

**Maintainers:** after GitHub Pro or making the repo public, run `./scripts/configure-github-protection.sh` to enforce checks on `main`.

## Adding a Lineup Explain question type

1. Add enum value in `LineupQuestionKind.cs` with XML summary
2. Add classification rules in `LineupQuestionClassifier.cs`
3. Create handler in `LineupExplain/Handlers/` implementing `ILineupExplainHandler`
4. Register in `LineupExplainRouter.cs`
5. Add tests in `tests/DmbSidecar.Api.Tests/LineupExplain/`
6. Optional: prompt chip in `extension/src/sidepanel/sidepanel.ts`

## Code documentation (required)

Every source file must include:

| Layer | File header | Functions / types |
|-------|-------------|-------------------|
| TypeScript | `/** @file ... */` — purpose, message flow, dependencies | JSDoc on every function; `// --- Section ---` in large modules |
| C# | `/// <summary>` on namespace types or top-of-file comment in `Program.cs` | XML docs on public/internal types and members |
| Python | Module docstring | Docstrings on functions/classes; `# --- Section ---` in large modules |
| Tests | File summary of what is under test | Class/method docs describing scenario |

Explain **why** and contracts, not obvious syntax. See `sidepanel.ts` and `LineupExplainRouter.cs` as reference.

## Commit messages

Complete sentences, focus on **why** (e.g. "Raise lineup_engine coverage gate so SS def regression is caught in CI").

## Do not commit

- `.env.local`, `.is_session`, production API keys
- `TestResults/`, `coveragereport/`, `sbom/` (generated locally)
