# Ropes & Gray — Interview 2 (Mon 6/15 3 PM ET)

**Who:** Aarthi Kumar (Mgr App Dev) + Erik Noren (Sr Solutions Architect)  
**No AI recording** — use **recorded demo link**, not live screen share of Cursor.

## 60-second architecture pitch

> I built a reference LOB copilot: Chrome extension captures page context from the app you're already in; ASP.NET Core 8 orchestrates with API-key auth and audit logging; Microsoft Foundry IQ grounds answers in a curated policy corpus; a Python tool service fetches live state from our MCP data layer. Read-only overlay — no writes to the host app. Same security questions you'd ask for iManage: what can the model see, what can it do, where are secrets.

## C# answer

> I deliberately implemented the integration layer in ASP.NET Core 8. My production depth is Python MCP and agent governance; I used Cursor to accelerate C# syntax and focused on boundaries I know well — auth, HttpClient to Foundry's Responses API, prompt assembly.

## Demo

Offer: "I have a 3-minute recorded walkthrough" → `docs/DEMO_SCRIPT.md` recording.

## Point to artifacts

- Public repo `dmb-sidecar`
- `docs/TEACHING_GUIDE.md`
- `docs/BUILD_JOURNAL.md` (human vs AI steps)
