# Demo Script (5 min max — interviews)

## Setup (before recording)

- [ ] `./scripts/start-dev.sh` running
- [ ] `az login` done
- [ ] Foundry agent `dmb-front-office` live (or note fallback mode)
- [ ] Chrome extension loaded
- [ ] Logged into ImagineSports, lineup page open

## Recording structure

### 0:00 — Problem (30 sec)
> "Diamond Mind owners juggle dozens of IS screens, rules docs, and live league data. I built a sidecar copilot that sits on the page you're already on."

### 0:30 — Architecture (45 sec)
Show `ARCHITECTURE.md` diagram: Extension → C# API → Foundry IQ + MCP.

### 1:15 — Live demo (2 min)
1. Edit Lineup vs RHP visible
2. Open side panel → show detected lineup slots
3. Click **Explain this screen**
4. Point to answer + citations (IQ vs MCP)

### 3:15 — Roster + data (1 min)
1. Navigate to roster page
2. Ask: "Am I balanced for cap and IR?"
3. Mention MCP snapshot in response

### 4:15 — Close (45 sec)
> "Read-only overlay, grounded in Foundry IQ, live data via MCP — same pattern for any enterprise web app. BUILD_JOURNAL documents every step for audit and learning."

## Fallback if Foundry down

Show offline fallback message in panel — explain T04 pending — still demo page context + MCP snapshot from API.

## Recording tip

Keep an unlisted video link in your portfolio README for async reviewers.
