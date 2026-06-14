# Front Office IQ — Interview Demo Script

> **Not a word-for-word script. Bullet points to hit in your own voice.**
> Built solo: me + Cursor (Claude). No team. One afternoon.

---

## 1. Opening Hook — Why This Exists

- Wanted to **prove speed-to-POC on tech I hadn't touched before** — Foundry, Azure AI Search, Chrome extensions
- Set a constraint: one session, ship something real, not a tutorial clone
- The domain: Diamond Mind Baseball — a legacy web game with no API, no mobile, no analytics
- The question I was answering: *"Can you retrofit AI onto a legacy web app you don't control?"*
- Answer: yes — and here's the receipts

---

## 2. What I Learned / Skilled Up On

- **Microsoft Foundry IQ** — knowledge base creation, Azure AI Search indexing, agent configuration, MCP connection types, auth model (`ProjectManagedIdentity` vs `CustomKeys`), Responses API endpoint patterns
- **Azure AI Search** — RBAC vs API key auth, the `knowledgebases/{name}/mcp` endpoint, `2026-05-01-Preview` API surface
- **Foundry Responses API** — agent-specific endpoint URL pattern, correct `api-version`, `agent_reference` body format, audience mismatch debugging (`cognitiveservices` vs `ai.azure.com`)
- **Chrome MV3 extensions** — first one I've built; service workers, content scripts, side panel API, message passing between layers
- Used an **existing MCP server** I had already built (dmb-mcp-server) — wired it in as a Python FastAPI bridge rather than rebuilding it

---

## 3. The Architecture (30-second version)

Walk through the ASCII diagram in OVERVIEW.md — hit these points:

- Chrome extension reads the DOM → no API needed on the host site
- Content script extracts structured context (lineup, players, pitcher hand)
- Sends to local ASP.NET Core 8 API (localhost:5280) with an API key
- API routes to either:
  - **Typed explain handlers** — deterministic, instant, no LLM (lineup math)
  - **Foundry IQ agent** — grounded Q&A with citations from 17 indexed docs
- Optional: Python MCP bridge for live league data
- **Key insight**: this pattern works for ANY legacy web app — SAP, Oracle, internal tools

---

## 4. Code Quality Demo — Walk a Method Live

Open `FoundryAgentService.cs` — walk through:

- **File-level XML doc** — explains the class, why it exists, why Azure CLI credential vs DefaultAzureCredential (document the decision, not the obvious)
- **`InvokeAsync`** — show the `/// <param>`, `/// <returns>`, `/// <exception>` blocks
- **`AcquireTokenAsync`** — show the section separator style, explain the WHY comment (audience mismatch, not a bug we ignored — a decision we documented)
- **`ExtractOutputText`** — show the fallback comment explaining the intent
- Point out: **no narration comments** — "import the module", "define the function" — none of that
- Show `FoundryOptions.cs` — typed configuration, `BuildResponsesUrl()`, every property documented

**Key talking point:** *"This is the standard I hold all code to. Not because someone reviewed it — because I built the quality gate into the commit hook."*

---

## 5. The Foundry IQ Integration — What We Configured

**What we set up:**
- Azure AI Foundry project (`front-office-iq`) with managed identity
- Azure AI Search service (Standard S1, West US 2) — `dmb-classic-standard-kb`
- Knowledge base `dmb-rules-kb` — 17 markdown documents from an existing KB I'd been building
- Agent `dmb-front-office` v2 — gpt-4.1-mini, system instructions, KB attached
- KB MCP connection — changed from `ProjectManagedIdentity` to `CustomKeys` to resolve the 403

**What we intentionally did NOT configure:**
- No Fabric IQ — out of scope for this POC
- No Work IQ / M365 integration
- No hosted agent deployment (container) — ran locally for demo
- No streaming responses — synchronous for simplicity
- No conversation history / multi-turn — single-shot Q&A

**What we debugged the hard way:**
- `.NET DefaultAzureCredential` produces wrong token audience for Foundry v1 endpoints → shelled out to `az` CLI instead
- Foundry connection type `ProjectManagedIdentity` → RBAC assignment didn't propagate → switched to `CustomKeys`
- Foundry endpoint URL has a specific agent-specific pattern with `api-version=2025-11-15-preview` — not documented obviously

---

## 6. The Chrome Extension — First One I've Built

- MV3 (Manifest V3) — modern Chrome extension standard
- Three moving parts: **service worker** (background), **content script** (DOM reader), **side panel** (UI)
- Content script has page-specific adapters — one per ImagineSports screen type
- No scraping — reads the page the user is already authenticated on (their data)
- Communicates with local API over HTTP with a shared API key
- First time I'd done this — ramped up entirely within the session

---

## 7. What the MCP Bridge Does Behind the Scenes

- I had already built `dmb-mcp-server` — a separate project that scrapes ImagineSports data and caches it locally
- Wrapped it as a Python FastAPI HTTP bridge rather than the standard MCP stdio protocol — made it easier to call from C#
- Provides: player stats, park factors, league standings, roster data
- The lineup optimizer uses this to calculate RC+defense scores and recommend positional swaps
- **Key point**: reused existing work rather than rebuilding — this is how real engineering works

---

## 8. Static Logic + AI KB — The Hybrid Model

**This is the architecture insight worth emphasizing:**

- Not everything needs an LLM
- Lineup math is **deterministic** — I know the formula, I know the rules, compute it directly
- Rules questions are **KB-grounded** — the answer exists in a document, retrieve it with citations
- Only true unknowns go to pure generation
- Result: faster responses, lower cost, higher accuracy, auditable answers
- *"The typed explain system classifies your question before deciding whether to compute, retrieve, or generate"*

---

## 9. Time and Cost

- **Total wall time**: ~6 hours, single session, June 14 2026
- **Token cost**: ~$20 in Cursor Pro usage (Claude Opus 4 primary)
- **Lines of production code**: ~3,000+
- **Test count**: 54 passing tests across .NET, Python, TypeScript
- **Files created/modified**: 80+
- **Azure resources provisioned**: 4 (Foundry project, AI Search, KB, Agent)
- **My prior experience with Foundry/Chrome extensions**: zero

See `COST_ANALYSIS.md` at the repo root for the full breakdown.

---

## 10. My Coding Principles

These don't change whether I'm vibe-coding or writing production Java:

- **Document the WHY, not the WHAT** — if a comment just restates the code, delete it
- **Smallest correct diff** — no drive-by refactors, no scope creep mid-PR
- **Offline-first, graceful degradation** — never let an external dependency crash the user experience
- **Typed configuration** — no magic strings, no hardcoded values, everything bound and validated
- **Tests prove behavior, not coverage numbers** — test the thing that would hurt if it broke
- **Security by default** — API key auth, CORS allowlist, no secrets in committed code, `.gitignore` is not optional
- **CI/CD is the contract** — if it doesn't pass the pipeline, it doesn't ship
- **Commit messages explain decisions** — future me should understand why, not just what

---

## 11. How AI-Assisted Development Actually Works (The Real Story)

- Cursor + Claude is not "autocomplete on steroids" — it's a **pairing partner that never gets tired**
- I still made every architecture decision — the AI implemented them at speed
- The hardest parts (Foundry auth debugging, Chrome extension message passing, Azure RBAC) still required **me to understand the problem** — the AI tried approaches and reported back
- Quality didn't drop because I enforced it: **pre-commit hooks, CI gates, code review standards baked into a Cursor skill** that runs before every commit
- *"The risk with vibe coding isn't speed — it's discipline. The answer is the same as it's always been: CI/CD, code standards, and ownership."*

---

## 12. The Demo (Live if Possible)

1. Open Chrome on ImagineSports Edit Lineup page
2. Show extension side panel — explain what the content script extracted
3. Type: *"What are the roster requirements?"*
4. Show response arrive with citations from the KB
5. Point to the source docs (iq-sources/) — *"These are real documents I've been writing for two years"*
6. If Lineup Lab is working: show Optimize → explain the typed handler path (no LLM, pure logic)
7. Pull up `FoundryAgentService.cs` in the IDE — walk the method
8. Show `COST_ANALYSIS.md` — land the $20 / 6 hours number

---

## 13. Links

- **GitHub**: https://github.com/OriginalDopey/dmb-sidecar
- **OVERVIEW.md** — full architecture
- **ELI5.md** — plain-language explainer
- **COST_ANALYSIS.md** — token cost breakdown
- **ARCHITECTURE.md** — ADRs and design decisions
