#!/usr/bin/env bash
# One-time / after-pull dependency install for DMB Sidecar.
set -euo pipefail
ROOT="$(cd "$(dirname "$0")/.." && pwd)"
export DOTNET_ROOT="${DOTNET_ROOT:-/opt/homebrew/opt/dotnet@8/libexec}"
export PATH="/opt/homebrew/opt/dotnet@8/bin:$PATH"

echo "=== MCP bridge Python deps ==="
cd "$ROOT/src/DmbSidecar.McpBridge"
python3.11 -m venv .venv
.venv/bin/pip install -q -r requirements.txt

echo "=== .NET API ==="
cd "$ROOT/src/DmbSidecar.Api"
dotnet restore -v q
dotnet build -v q

if [[ -d "$ROOT/extension/node_modules" ]] || command -v npm >/dev/null 2>&1; then
  echo "=== Chrome extension (optional) ==="
  cd "$ROOT/extension"
  if [[ -f package.json ]]; then
    npm ci 2>/dev/null || npm install
    npm run build
  fi
fi

echo "Done. Run: ./scripts/start-dev.sh"
