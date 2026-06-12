---
name: dmb-sidecar-quality-gate
description: >-
  Enforces resume-grade quality before commits or PRs in dmb-sidecar: run tests,
  sync extension dist, block secrets/personal paths, match CONTRIBUTING standards.
  Use when committing, opening a PR, preparing for code review, or when the user
  asks to maintain interview-grade quality in this repo.
---

# DMB Sidecar — Quality Gate

Run this checklist **before every commit** and **before opening a PR**. The git hook runs `scripts/pre-commit-quality.sh` automatically after `./scripts/install-git-hooks.sh`.

## Pre-commit (required)

```bash
./scripts/pre-commit-quality.sh
```

This runs:
- Staged-file scan for personal paths (`/Users/...`) and obvious secrets
- `extension/dist` rebuild check when `extension/src/` is staged
- `dotnet test` (API)
- `pytest tests/python`
- `npm test` in `extension/`

## Pre-PR (full CI mirror)

```bash
./scripts/ci.sh
```

Adds coverage collection, SBOM generation, and Release build — same gates as GitHub Actions.

## Branch policy

- **Never push to `main` directly** — feature branch + PR + squash merge
- Pre-push hook blocks `main` (override: `DMB_SIDECAR_ALLOW_MAIN_PUSH=1`)
- After GitHub Pro or public repo: `./scripts/configure-github-protection.sh`
- See [docs/GITHUB_BEST_PRACTICES.md](../../docs/GITHUB_BEST_PRACTICES.md)

## Code standards (do not regress)

| Area | Rule |
|------|------|
| **Scope** | Smallest correct diff; no drive-by refactors |
| **Tests** | Behavior changes need tests in the matching layer |
| **Explain handlers** | New question kind → enum + classifier + handler + router + tests |
| **Comments** | Every source file: file header + documented functions (see CONTRIBUTING.md); no "demo"/"hackathon" language |
| **Secrets** | Never commit `.env.local`, `.is_session`, real team IDs in docs |
| **Paths** | Use `../DiamondMind` or env vars in scripts — no `/Users/...` |
| **Extension** | Commit `extension/dist` after `npm run build` when `src/` changes |
| **CORS** | Only `chrome-extension://`, `localhost`, `127.0.0.1` |
| **Foundry** | Offline-first; empty `ProjectEndpoint` in committed `appsettings.json` |

## Critical paths (must stay tested)

- `LineupExplain/*` — classifier, router, all six handlers
- `McpDataFilter` — stale cache rejection
- `lineup_engine._def_runs` — SS/2B error penalty
- API integration — `/health`, `/lineup/explain`, `/lineup/analyze` auth
- `lineup-format.ts` — OBP/OPS/RC/600 display

## Commit message

Complete sentences; focus on **why** (see CONTRIBUTING.md).

## If the gate fails

1. Fix the failing test or sync `extension/dist`
2. Re-run `./scripts/pre-commit-quality.sh`
3. Only use `git commit --no-verify` for emergencies — document why in the PR

## Reference docs

- [docs/QUALITY.md](../../docs/QUALITY.md) — coverage targets and CI artifacts
- [CONTRIBUTING.md](../../CONTRIBUTING.md) — workflow and explain-handler extension guide
- [docs/CODEBASE_MAP.md](../../docs/CODEBASE_MAP.md) — where to change behavior
