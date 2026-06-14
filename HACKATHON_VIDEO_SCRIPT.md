# Front Office IQ — Hackathon Demo Video Script (5 min max)

> **Core message:** A hybrid system where deterministic math handles what it can, and Foundry IQ handles what it should — with guardrails to keep it domain-specific and cost-efficient.

---

## Opening: What This Is (30 seconds)

- "Front Office IQ adds AI-powered decision support to a legacy web app that has no API."
- "It's a Chrome extension that reads the page you're on, understands the context, and provides grounded advice — but here's the key design choice:"
- "**Not every question burns tokens.** The system routes between hard-coded math and Foundry IQ based on what the question actually needs."

---

## Demo: The Hybrid in Action (2.5 minutes)

### Show the deterministic path first:

- Open ImagineSports → Edit Lineup page
- Side panel appears with **Lineup Lab** (detects lineup name, pitcher side automatically from DOM)
- Click **"Optimize this lineup"**
- "This optimization is pure math — Runs Created formula plus defensive value calculations. Zero LLM calls. Instant. Deterministic. Reproducible."
- Point out the result: "+8.2 RC+def improvement, Knight over Mackanin at SS"
- Click one of the explain chips: **"At SS: Knight > Mack"**
- "That explain answer is also deterministic — a typed handler looked at the defensive error ratings and calculated the run value difference. Still no tokens spent."

### Now show the Foundry IQ path:

- Type a question that NEEDS the knowledge base: "What happens if I release a player mid-season?"
- "THIS question goes to Foundry IQ — because it's a rules question that requires grounded, cited knowledge."
- Show the answer come back with citations: **75% recovery, sources cited**
- "The system classified this as a question the math can't answer, so it routed to the agent. Grounded. Cited. Not hallucinated."

### Explain the routing logic:

- "The typed question classifier has 6 categories: DH assignment, batting order, position assignment, position comparison, recommendation summary, and fallback."
- "Only fallback hits the LLM. Everything else is instant math."
- "As we learn what users commonly ask, we promote patterns from LLM-answered to hard-coded. The system gets CHEAPER over time, not more expensive."

---

## Architecture: The Tech Stack (1 minute)

### Flash the diagram (OVERVIEW.md or draw on whiteboard):

```
Chrome Extension (DOM observation)
    ↓ X-Api-Key HTTP
ASP.NET Core 8 API (question router)
    ↓                    ↓
Deterministic Math    Foundry IQ Agent
(lineup engine,       (17 KB docs,
 typed handlers)       Azure AI Search,
                       cited answers)
    ↓
Python MCP Bridge
(live league data)
```

- "Three layers. The API decides which path a question takes."
- "The extension never talks to Foundry directly — the API is the guardrail."

### Guardrails:

- "The agent has strict instructions: only answer Diamond Mind Baseball questions. Decline everything else."
- "API key auth on every request — this can't become someone's free GPT."
- "Context-specific: the extension only activates on ImagineSports pages. The agent only has DMB rules in its knowledge base."
- "The API is the control plane — we can add rate limiting, user tracking, or cost budgets without touching the extension or the agent."

---

## What Makes This Enterprise-Ready (45 seconds)

- "The pattern scales: as common questions get identified, promote them from LLM to deterministic. Token cost drops, response time drops, reliability goes up."
- "Offline-first: when Foundry is unreachable, falls back to local keyword search against the same documents. Never leaves the user stuck."
- "54 tests across .NET, Python, and TypeScript. CI/CD with coverage gates. Pre-commit hooks. SBOM generation."
- "This isn't a chatbot wrapper. It's an **asset management decision system** with AI grounding where it adds value and math where it doesn't."

---

## Close (15 seconds)

- "One person, one AI-enabled IDE, one afternoon, ~$20 in tokens."
- "The reusable pattern: DOM context → typed routing → math where possible, grounded AI where necessary → guardrails everywhere."
- "Thanks for watching."

---

## Recording Checklist

- [ ] Screen record with QuickTime (Cmd+Shift+5)
- [ ] Show: Optimize (math) → Explain chip (typed handler) → Ask question (Foundry IQ)
- [ ] Flash architecture diagram
- [ ] Mention guardrails and cost-over-time story
- [ ] Upload to YouTube (Unlisted is fine)
- [ ] Add link to hackathon project page
- [ ] **Deadline: 11:59 PM PT tonight**
