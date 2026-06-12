# ADR-002: Foundry via HttpClient Responses API (not Agent Framework .NET 10)

## Status
Accepted — 2026-06-12

## Context
Microsoft's newest hosted-agent samples require .NET 10. Mac had .NET 5; we installed .NET 8. Dave already validated Foundry via Python `agent.py` using `responses.create` + `agent_reference`.

## Decision
`FoundryAgentService` POSTs JSON to:
```
{project}/agents/{name}/endpoint/protocols/openai/responses
```
with `DefaultAzureCredential` bearer token. Agent + IQ configured in **Foundry portal**, not in C# code.

## Alternatives rejected
- **Agent Framework .NET 10 hosted agent** — wrong SDK version, overkill for hackathon.
- **Python API only** — weaker Ropes C# narrative.

## Consequences
- Portal is source of truth for instructions, IQ, tools.
- C# stays thin — good for teaching separation of concerns.
- Until `dmb-front-office` exists, Development profile uses `computing-historian` as smoke-test fallback.
