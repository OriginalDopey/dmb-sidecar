#!/usr/bin/env bash
# Start MCP bridge + API for local development.
set -euo pipefail
ROOT="$(cd "$(dirname "$0")/.." && pwd)"
export DOTNET_ROOT="${DOTNET_ROOT:-/opt/homebrew/opt/dotnet@8/libexec}"
export PATH="/opt/homebrew/opt/dotnet@8/bin:$PATH"

# dmb-mcp-server paths (override in .env.local)
export DMB_MCP_SRC="${DMB_MCP_SRC:-/Users/originaldopey/Documents/CursonProjects/dmb-mcp-server/src}"
export DMB_DB_PATH="${DMB_DB_PATH:-/Users/originaldopey/Documents/CursonProjects/DiamondMind/data/is_scout.db}"
export DMB_SESSION_PATH="${DMB_SESSION_PATH:-/Users/originaldopey/Documents/CursonProjects/DiamondMind/.is_session}"
export DMB_CONFIG_PATH="${DMB_CONFIG_PATH:-/Users/originaldopey/Documents/CursonProjects/dmb-mcp-server/config/leagues.json}"
export DMB_ENTRY_TEAM_ID="${DMB_ENTRY_TEAM_ID:-}"

if [[ -f "$ROOT/.env.local" ]]; then
  set -a
  # shellcheck disable=SC1091
  source "$ROOT/.env.local"
  set +a
fi

echo "=== DMB Sidecar dev stack ==="
echo "MCP DB: $DMB_DB_PATH"
echo "API:    http://127.0.0.1:5280"
echo "Bridge: http://127.0.0.1:8765"
echo ""

cleanup() { kill $(jobs -p) 2>/dev/null || true; }
trap cleanup EXIT

cd "$ROOT/src/DmbSidecar.McpBridge"
if [[ ! -d .venv ]]; then
  python3.11 -m venv .venv
  .venv/bin/pip install -q -r requirements.txt
fi
# shellcheck disable=SC1091
source .venv/bin/activate
export PYTHONPATH="$DMB_MCP_SRC${PYTHONPATH:+:$PYTHONPATH}"
uvicorn app:app --host 127.0.0.1 --port 8765 &
sleep 1

cd "$ROOT/src/DmbSidecar.Api"
dotnet run --launch-profile http
