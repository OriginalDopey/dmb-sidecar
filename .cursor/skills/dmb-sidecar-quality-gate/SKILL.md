---
name: dmb-sidecar-quality-gate
description: >-
  Enforces enterprise-grade quality before commits or PRs in dmb-sidecar.
  Runs tests, syncs extension dist, blocks secrets/personal paths, validates
  XML/JSDoc/docstring coverage, and ensures CONTRIBUTING standards are met.
  Activate when: committing, opening a PR, preparing for code review, or when
  the user asks to maintain production-quality standards in this repo.
globs:
  - "src/**"
  - "extension/**"
  - "tests/**"
  - "scripts/**"
---

# DMB Sidecar — Quality Gate

Run this checklist **before every commit** and **before opening a PR**. The git hook runs `scripts/pre-commit-quality.sh` automatically after `./scripts/install-git-hooks.sh`.

---

## Pre-commit Checklist (Required)

```bash
./scripts/pre-commit-quality.sh
```

This script enforces:

| Check | What It Does |
|-------|-------------|
| Secret scan | Blocks staged files containing API keys, tokens, or `.env` values |
| Path scan | Blocks hardcoded `/Users/...` or `C:\Users\...` paths |
| Extension dist sync | Rebuilds `extension/dist` if `extension/src/` changed |
| .NET tests | `dotnet test` — all must pass |
| Python tests | `pytest` — all must pass |
| Extension tests | `npx vitest run` — all must pass |

---

## Documentation Standards (Every File)

### C# (.cs)
- **File-level**: `/// <summary>` XML doc block describing the class purpose, collaborators, and design rationale
- **Public members**: `/// <summary>` on every public method, property, and constructor
- **Parameters**: `/// <param name="">` for non-obvious parameters
- **Returns/Throws**: `/// <returns>` and `/// <exception cref="">` where applicable

### TypeScript (.ts)
- **File-level**: JSDoc `/** ... */` block at the top describing the module
- **Exported functions**: JSDoc with `@param`, `@returns`, `@throws`
- **Section comments**: `// ─── Section Name ───` separators for logical groupings

### Python (.py)
- **Module-level**: Triple-quote docstring at the top of every file
- **Functions/Classes**: Google-style docstrings with Args/Returns/Raises
- **Type hints**: All function signatures fully typed

### General Rules
- **No narration comments** — Don't say "Import the module" or "Define the function"
- **No "demo"/"hackathon"/"TODO" language** — This is production code
- **Explain WHY, not WHAT** — Comments add context the code can't express
- **Section separators** — Use visual separators for logical code blocks

---

## Code Standards (Must Not Regress)

| Area | Rule |
|------|------|
| **Scope** | Smallest correct diff; no drive-by refactors |
| **Tests** | Behavior changes need tests in the matching layer |
| **Explain handlers** | New question kind → enum + classifier + handler + router + tests |
| **Secrets** | Never commit `.env.local`, `.is_session`, real API keys or team IDs |
| **Paths** | Use env vars or relative paths — never `/Users/...` |
| **Extension** | Commit `extension/dist` after `npm run build` when `src/` changes |
| **CORS** | Only `chrome-extension://`, `localhost`, `127.0.0.1` origins |
| **Foundry** | Offline-first; empty `ProjectEndpoint` in committed `appsettings.json` |
| **Error handling** | Graceful degradation; never crash on Foundry/MCP unavailability |

---

## Critical Paths (Must Stay Tested)

- `LineupExplain/*` — classifier, router, all six handlers
- `McpDataFilter` — stale cache rejection
- `lineup_engine._def_runs` — SS/2B error penalty
- API integration — `/health`, `/lineup/explain`, `/lineup/analyze` auth
- `lineup-format.ts` — OBP/OPS/RC/600 display
- `FoundryAgentService` — token acquisition, response parsing

---

## Branch Policy

- **Never push to `main` directly** — feature branch + PR + squash merge
- Pre-push hook blocks `main` (override: `DMB_SIDECAR_ALLOW_MAIN_PUSH=1`)
- After GitHub Pro or public repo: `./scripts/configure-github-protection.sh`
- See [docs/GITHUB_BEST_PRACTICES.md](../../docs/GITHUB_BEST_PRACTICES.md)

---

## Commit Message Format

```
<verb> <what changed>

<1-2 sentences explaining WHY this change was made>
```

Examples:
- `fix Foundry auth: use agent-specific endpoint with api-version`
- `add typed explain handler for position comparison questions`

---

## Pre-PR (Full CI Mirror)

```bash
dotnet test && cd src/DmbSidecar.McpBridge && pytest && cd ../../extension && npx vitest run
```

Full CI adds: coverage collection, SBOM generation, Release build — same gates as GitHub Actions.

---

## If the Gate Fails

1. Fix the failing test or sync `extension/dist`
2. Re-run `./scripts/pre-commit-quality.sh`
3. Only use `git commit --no-verify` for emergencies — document why in the PR

---

## Reference Docs

- [OVERVIEW.md](../../OVERVIEW.md) — Full system architecture and design decisions
- [ELI5.md](../../ELI5.md) — Plain-language project explainer
- [docs/QUALITY.md](../../docs/QUALITY.md) — Coverage targets and CI artifacts
- [CONTRIBUTING.md](../../CONTRIBUTING.md) — Workflow and explain-handler extension guide
- [docs/CODEBASE_MAP.md](../../docs/CODEBASE_MAP.md) — Where to change behavior
- [ARCHITECTURE.md](../../ARCHITECTURE.md) — ADRs and design rationale
