#!/usr/bin/env bash
# Stop MCP bridge + API (free ports 8765 / 5280).
set -euo pipefail

free_port() {
  local port=$1
  local pids
  pids=$(lsof -nP -iTCP:"$port" -sTCP:LISTEN -t 2>/dev/null || true)
  if [[ -n "$pids" ]]; then
    echo "Stopping port $port (PIDs: $pids)"
    kill $pids 2>/dev/null || kill -9 $pids 2>/dev/null || true
  fi
}

pkill -f "uvicorn app:app.*8765" 2>/dev/null || true
pkill -f "DmbSidecar.Api" 2>/dev/null || true

free_port 8765
free_port 5280

sleep 1
if lsof -nP -iTCP:8765 -sTCP:LISTEN >/dev/null 2>&1 || lsof -nP -iTCP:5280 -sTCP:LISTEN >/dev/null 2>&1; then
  echo "Warning: some listeners may still be running."
  lsof -nP -iTCP:8765 -sTCP:LISTEN 2>/dev/null || true
  lsof -nP -iTCP:5280 -sTCP:LISTEN 2>/dev/null || true
  exit 1
fi

echo "Dev stack stopped (8765, 5280 free)."
