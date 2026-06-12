# Morning status — read this first

**Built overnight 2026-06-12 while you slept.**

## What's done

| Component | Status |
|-----------|--------|
| .NET 8 SDK | Installed (`8.0.128`) |
| `iq-sources/` | 19 markdown files synced |
| MCP HTTP bridge | Code complete; imports verified |
| ASP.NET Core API | **Builds** — `/health`, `/advise`, `/foundry/smoke` |
| Chrome extension | **Built** — `extension/dist/` ready |
| Documentation | 15+ docs including TEACHING_GUIDE, BUILD_JOURNAL, interview scripts |
| ADRs | 3 architecture decision records |

## What you must do (≈2 hours)

**Start here:** [docs/TOMORROW_CHECKLIST.md](docs/TOMORROW_CHECKLIST.md)

1. `cp .env.local.example .env.local` + set `DMB_ENTRY_TEAM_ID`
2. `az login`
3. **Foundry portal** — create `dmb-front-office` agent + IQ upload ([manual-steps/FOUNDRY_IQ_PORTAL.md](docs/manual-steps/FOUNDRY_IQ_PORTAL.md))
4. `./scripts/start-dev.sh`
5. Load Chrome extension → test on IS lineup page
6. Fix lineup DOM selectors if slots are empty (DevTools)
7. Record demo → push GitHub → hackathon submit

## Repo location

```
/Users/originaldopey/Documents/CursonProjects/dmb-sidecar
```

## Quick teach-yourself path

1. [README.md](README.md) — overview
2. [docs/TEACHING_GUIDE.md](docs/TEACHING_GUIDE.md) — how to explain it
3. [ARCHITECTURE.md](ARCHITECTURE.md) — diagram
4. [docs/BUILD_JOURNAL.md](docs/BUILD_JOURNAL.md) — what AI vs you did

Good luck — this is interview-grade scaffolding. The Foundry portal step is the only big manual gate.
