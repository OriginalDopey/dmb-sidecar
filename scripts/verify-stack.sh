#!/usr/bin/env bash
set -euo pipefail
API_KEY="${API_KEY:-dev-key-change-me}"
echo "=== DMB Sidecar stack verification ==="
echo -n "MCP bridge: "
curl -sf http://127.0.0.1:8765/health | python3 -m json.tool
echo -n "API health: "
curl -sf http://127.0.0.1:5280/health | python3 -m json.tool
echo "Foundry smoke:"
curl -sf -X POST http://127.0.0.1:5280/foundry/smoke -H "X-Api-Key: $API_KEY" | python3 -m json.tool || echo "(Foundry smoke failed — see docs/MANUAL_REQUIRED.md §2)"
echo "Advise (offline IQ + MCP):"
curl -sf -X POST http://127.0.0.1:5280/advise -H "X-Api-Key: $API_KEY" -H "Content-Type: application/json" \
  -d '{"question":"What is in-season release salary recovery?","context":{"pageType":"roster","url":"https://www.imaginesports.com/bball/team/roster","curTeam":"mine","slots":[]}}' \
  | python3 -c "import sys,json; r=json.load(sys.stdin); print('answer:', r['answer'][:200]+'...'); print('warning:', r.get('warning','none'))"
echo "Done."
