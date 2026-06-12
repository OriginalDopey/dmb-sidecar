# ADR-001: Three-tier stack split (TypeScript + C# + Python)

## Status
Accepted — 2026-06-12

## Context
We need a Chrome overlay, enterprise-credible API orchestration, and live DMB league data. A single-language rewrite would delay delivery without adding capability.

## Decision
| Layer | Language | Responsibility |
|-------|----------|----------------|
| Browser | TypeScript | DOM adapters, side panel UI, message broker |
| API | C# ASP.NET Core 8 | Auth, audit, Foundry HttpClient, prompt assembly |
| Data | Python | dmb-mcp-server repository via HTTP bridge |

## Consequences
- **Positive:** Reuses production `dmb-mcp-server`; C# story for interviews; extension is standard pattern.
- **Negative:** Three runtimes in dev (`start-dev.sh` orchestrates).
- **Teaching point:** LOB copilots often split "face" (browser) from "brain" (API) from "hands" (tool services).
