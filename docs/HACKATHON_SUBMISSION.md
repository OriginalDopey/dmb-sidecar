# Agents League submission checklist

**Deadline:** June 14, 2026 11:59 PM PT  
**Track:** Reasoning Agents (+ Best use of IQ tools)

## Required artifacts

- [ ] Public GitHub repo URL
- [ ] Architecture diagram → [ARCHITECTURE.md](../ARCHITECTURE.md)
- [ ] Demo video (≤5 min) on YouTube or Vimeo
- [ ] Project description (paste below into submission form)

## Project description (draft)

**DMB Sidecar** is an agentic web overlay for Diamond Mind Baseball (ImagineSports). A Chrome side panel reads the management screen the owner is already on (lineup, roster) and returns grounded advice via Microsoft Foundry IQ (rules, strategy, configuration corpus) plus live league data through an MCP-aligned Python tool bridge.

**Problem:** Classic Standard owners juggle 8+ configuration screens and scattered rules; small mistakes cost real bankroll.

**Solution:** Read-only copilot — extension captures page context, ASP.NET Core 8 orchestrates, Foundry agent cites IQ for mechanics and MCP for live standings/roster.

**Stack:** Chrome MV3 (TypeScript), ASP.NET Core 8 (C#), Microsoft Foundry + Foundry IQ, Python/FastAPI bridge to [dmb-mcp-server](https://github.com/OriginalDopey/dmb-mcp-server).

**Reasoning:** Multi-step flow — classify page → fetch league snapshot → IQ-grounded synthesis with citations.

**Not affiliated with ImagineSports.**

## Technologies checkbox

- Microsoft Foundry / Foundry IQ
- MCP (tool data layer)
- ASP.NET Core 8
- Chrome Extension MV3
