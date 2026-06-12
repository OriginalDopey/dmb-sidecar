# Contributing

## Prerequisites

- .NET 8 SDK, Node 20+, Python 3.11+
- Read [docs/CODEBASE_MAP.md](docs/CODEBASE_MAP.md) before changing behavior

## Workflow

1. Branch from `main`
2. Make focused changes; match existing style (`.editorconfig`)
3. Add or update tests for behavior you change
4. Run `./scripts/ci.sh` locally
5. Open PR — CI must pass (build, test, coverage gates, SBOM)

## Adding a Lineup Explain question type

1. Add enum value in `LineupQuestionKind.cs` with XML summary
2. Add classification rules in `LineupQuestionClassifier.cs`
3. Create handler in `LineupExplain/Handlers/` implementing `ILineupExplainHandler`
4. Register in `LineupExplainRouter.cs`
5. Add tests in `tests/DmbSidecar.Api.Tests/LineupExplain/`
6. Optional: prompt chip in `extension/src/sidepanel/sidepanel.ts`

## Commit messages

Complete sentences, focus on **why** (e.g. "Raise lineup_engine coverage gate so SS def regression is caught in CI").

## Do not commit

- `.env.local`, `.is_session`, production API keys
- `TestResults/`, `coveragereport/`, `sbom/` (generated locally)
