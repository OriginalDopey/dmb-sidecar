# ADR-003: MCP bridge imports dmb_mcp (not stdio MCP protocol)

## Status
Accepted — 2026-06-12

## Context
Calling stdio MCP from C# in 48 hours is high risk. `dmb-mcp-server` already exposes typed `Repository` queries.

## Decision
FastAPI service imports `dmb_mcp.context.AppContext` and calls `Repository` directly. C# uses `HttpClient` to `http://127.0.0.1:8765`.

## Consequences
- **Positive:** Ships fast; same data as MCP tools; easy to test with curl.
- **Negative:** Not a "pure" MCP architecture diagram — document honestly as "MCP-aligned tool layer."
- **Future:** Could wrap same app as real MCP server for Foundry remote tool registration.
