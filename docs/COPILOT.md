# GitHub Copilot / Cursor assist log (Agents League Creative track)

Document how AI-assisted development was used in this project.

## Tools

- **Cursor Composer 2.5** — overnight scaffold session (2026-06-12)
- **GitHub Copilot** — (Dave: note any Copilot Chat usage during tomorrow's fixes)

## Composer generated (overnight)

| Area | Files | Human review needed |
|------|-------|---------------------|
| ASP.NET Core API | `Program.cs`, Services/*, Middleware/* | Foundry endpoint config |
| MCP bridge | `app.py` | Env paths, session cookie |
| Chrome extension | `extension/src/**` | DOM selectors on live IS |
| Documentation | `docs/**`, README, ARCHITECTURE | Accuracy, interview tone |
| IQ sync | `scripts/sync-iq-sources.sh` | File selection |

## Dave manual (required)

- Foundry IQ portal upload (T04)
- `az login`
- `.env.local` / `DMB_ENTRY_TEAM_ID`
- Chrome Load unpacked
- DOM selector verification on ImagineSports
- Demo video recording
- Hackathon submission form

## Prompts that worked (overnight session)

> "Build DMB Sidecar per plan: C# API, Python MCP bridge, Chrome MV3 extension, overly document everything"

## Honest assessment for interviews

AI accelerated **scaffolding and boilerplate** (~70% of files). Human judgment required for **Azure portal, auth, DOM selectors, and demo narrative** — the parts buyers and architects actually challenge.
