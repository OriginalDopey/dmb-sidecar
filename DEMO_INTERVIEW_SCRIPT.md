# Front Office IQ — Interview Code Review Script

> **Purpose:** Walkthrough bullets for job interview screen shares and code review discussions.
> Not word-for-word — use as a guide and speak naturally.
> For hackathon video, see `HACKATHON_VIDEO_SCRIPT.md`.

---

## Part 1: The Hook (30 seconds)

- "This project proves one thing: **speed to POC on unfamiliar tech.**"
- "Before this weekend, I had never touched Microsoft Foundry, never built a Chrome extension, and hadn't written C# in production in years."
- "In 6 hours, using Cursor (AI-enabled IDE) and roughly $20 in tokens, I shipped a complete enterprise-grade system — end to end, live, grounded in real documents."
- "Let me show you what it does."

---

## Part 2: Live Demo (2 minutes)

### Show the extension working:
- Open ImagineSports.com → Edit Lineup page
- Side panel appears with **Lineup Lab** context (lineup name, vs LHP/RHP)
- Type: "What are the roster requirements?" → hit Ask
- **Show the grounded answer come back with citations** (75% salary recovery, KB sources cited)
- "That answer came from a Foundry IQ agent backed by 17 indexed documents — not hallucinated."

### Show the architecture:
- Quick flash of the OVERVIEW.md diagram or ARCHITECTURE.md mermaid
- "Chrome extension reads the DOM → sends context to ASP.NET Core API → routes to Foundry agent → agent queries Azure AI Search → returns cited answer"

### Show the Foundry playground (optional):
- Ask same question in Azure portal playground
- Same grounded answer, same citations

---

## Part 3: Code Quality Walkthrough (1.5 minutes)

### Pull up `FoundryAgentService.cs`:
- "Let me show you what production-quality AI integration looks like in C#."
- Point out:
  - **Full XML documentation** — every public method, parameters, exceptions, return types
  - **Section separators** — Token Acquisition, Response Parsing (visual code organization)
  - **Defensive error handling** — graceful failures with structured logging
  - **Response parsing** — handles both `output_text` shortcut and nested `output[].content[].text`
- "This isn't prototype code. Every file in this repo has this level of documentation."

### Pull up `FoundryOptions.cs`:
- "Typed configuration with `IOptions<T>` pattern — no magic strings, no scattered config"
- "URL construction is encapsulated — change the endpoint format in one place"

---

## Part 4: What I Learned / Struggled With (1 minute)

### New tech I skilled up on:
- **Microsoft Foundry** — project setup, agent creation, KB indexing, connection auth
- **Foundry IQ** — knowledge base creation, MCP tool connections, agentic retrieval
- **Chrome MV3 extensions** — first one ever (content scripts, service workers, side panels)
- **Azure AI Search** — MCP endpoint, RBAC vs API key auth, connection types

### What we struggled with (and solved):
- **Foundry auth chain** — 401 → wrong audience → wrong endpoint → wrong api-version. Took 8 sequential hypotheses to resolve. Documented in code comments.
- **KB 403** — Foundry connection used `ProjectManagedIdentity` but needed `CustomKeys`. Fixed via REST API.
- **.NET `DefaultAzureCredential`** — produces wrong token audience on macOS for Foundry v1 endpoints. Workaround: shell to `az` CLI directly.

---

## Part 5: Architecture & What's Behind the Scenes (30 seconds)

- **Existing MCP I built** — `dmb-mcp-server` (separate project) scrapes league data, caches in SQLite, exposes via MCP protocol. The Python bridge wraps it for HTTP.
- **Existing knowledge base** — 17 markdown docs I'd already written for my own DMB research. Uploaded to Foundry IQ.
- **Static recommendation + AI hybrid** — Lineup optimization is pure math (RC formula + defensive value). Only "why" questions hit the LLM. Best of both worlds.

### What's configured vs. what's not:
| Configured | Not configured (future work) |
|-----------|------------------------------|
| Foundry IQ with 17 docs | Multi-turn conversation memory |
| Typed explain handlers (6 types) | Streaming responses |
| Offline fallback (LocalIqService) | User auth beyond API key |
| CI/CD with coverage gates | Hosted deployment (Docker) |
| Pre-commit quality hooks | Production credential management |

---

## Part 6: Coding Principles

### My engineering principles:
1. **Offline-first** — Never depend on a cloud service being available. Degrade gracefully.
2. **Typed everything** — No `dynamic`, no `any`, no unvalidated JSON. Typed config, typed responses, typed question routing.
3. **Math first, LLM second** — If a question can be answered deterministically, don't spend tokens. Promote common LLM answers to hard-coded logic over time.
4. **Guardrails are architecture** — Rate limiting, domain restriction, API key auth, and context fencing aren't afterthoughts. The API is the control plane.
5. **Test the boundaries** — Integration tests prove auth works. Unit tests prove logic works. Both matter.
6. **Documentation is code** — XML docs, JSDoc, and docstrings are part of the deliverable, not afterthoughts.
7. **Smallest correct diff** — Every commit does one thing. Every PR is reviewable.
8. **CI is the source of truth** — If it doesn't pass CI, it doesn't ship. Coverage gates prevent regression.

### On AI-assisted development and why process matters MORE now:
- "I used Cursor with Claude as my pair programmer. The AI wrote first drafts; I reviewed, corrected, and directed."
- "The key in a vibe-coding world: **strict CI/CD pipelines + quality gates + documentation standards.** Speed without guardrails is tech debt at 10x velocity."
- "Shared team skills matter more than ever — when AI generates code at speed, you need teammates who can READ and REVIEW at the same level. Code review becomes the critical skill."
- "This project proves AI-assisted development works for enterprise-quality output — not just prototypes. But ONLY with process discipline."
- "The $20 cost story isn't about being cheap — it's about proving that quality and speed aren't tradeoffs when you have the right guardrails."

### The hybrid architecture pitch (for solution architect interviews):
- "Not every question should hit an LLM. We classify first, route second."
- "6 typed handlers answer instantly with deterministic math. Only unknowns go to Foundry."
- "As we see repeated question patterns, we PROMOTE them from LLM to hard-coded. System gets cheaper and faster over time."
- "The AI isn't the product — the decision support is the product. AI is one implementation detail behind the routing layer."
- "Guardrails: domain-locked agent, API key auth, context-specific activation, no freeform LLM access. This can't become someone's free ChatGPT."

---

## Part 7: The Numbers

| Metric | Value |
|--------|-------|
| Total dev time | ~6 hours |
| Token cost | ~$20 |
| Lines of production code | 3,000+ |
| Tests passing | 54 |
| Files created/modified | 80+ |
| Azure resources provisioned | 4 |
| New tech learned | Foundry, Foundry IQ, Chrome MV3, Azure AI Search |
| Equivalent human time (estimate) | 40-60 hours |

---

## Closing (15 seconds)

- "One person. One AI-enabled IDE. One afternoon. Enterprise-grade result."
- "The pattern — DOM observation → typed routing → grounded AI — works for any legacy app."
- "Questions?"

---

## Hackathon Submission Checklist

Per [Official Rules](https://github.com/microsoft/Agents-League-AISF-Regulations/blob/main/OFFICIAL%20RULES.md):

- [x] Project description on Contest Website
- [ ] **Demo video (5 min max) uploaded to YouTube or Vimeo** ← REQUIRED
- [x] Public GitHub repository: https://github.com/OriginalDopey/dmb-sidecar
- [x] Architecture diagram (in ARCHITECTURE.md)
- [x] Challenge linked: Reasoning Agents

### Demo video recording tips:
- Screen record with QuickTime (Cmd+Shift+5 on Mac)
- Show: extension working → Foundry answer with citations → code walkthrough → architecture
- Upload to YouTube as **Unlisted** (doesn't need to be public)
- Add the YouTube link to your project page
- **Deadline: 11:59 PM PT tonight (2:59 AM ET Monday)**
