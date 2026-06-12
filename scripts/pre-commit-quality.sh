#!/usr/bin/env bash
# Resume-grade quality gate — run before every commit (also invoked by git pre-commit hook).
set -euo pipefail
ROOT="$(cd "$(dirname "$0")/.." && pwd)"
cd "$ROOT"

RED='\033[0;31m'
GREEN='\033[0;32m'
NC='\033[0m'
fail() { echo -e "${RED}✗ $1${NC}" >&2; exit 1; }
ok()   { echo -e "${GREEN}✓ $1${NC}"; }

echo "=== DMB Sidecar quality gate ==="

# --- Secrets / personal paths in staged files ---
if git rev-parse --git-dir >/dev/null 2>&1; then
  STAGED="$(git diff --cached --name-only --diff-filter=ACM 2>/dev/null || true)"
  if [[ -n "$STAGED" ]]; then
    while IFS= read -r f; do
      [[ -z "$f" || ! -f "$f" ]] && continue
      case "$f" in
        scripts/pre-commit-quality.sh|.cursor/skills/*) continue ;;
      esac
      if grep -qE '/Users/[^/]+/Documents|\.is_session|sk-[a-zA-Z0-9]{20,}' "$f" 2>/dev/null; then
        fail "Staged file may contain personal paths or secrets: $f"
      fi
    done <<< "$STAGED"
    ok "No personal paths or obvious secrets in staged files"
  fi
fi

# --- Extension dist must match src when extension sources change ---
if git rev-parse --git-dir >/dev/null 2>&1; then
  EXT_CHANGED="$(git diff --cached --name-only | grep -E '^extension/src/' || true)"
  if [[ -n "$EXT_CHANGED" ]]; then
    NPM_CACHE="${NPM_CONFIG_CACHE:-$ROOT/.npm-cache}"
    mkdir -p "$NPM_CACHE"
    (cd extension && npm run build --silent)
    if ! git diff --exit-code extension/dist >/dev/null 2>&1; then
      fail "extension/src changed but extension/dist is stale — run: cd extension && npm run build"
    fi
    ok "extension/dist is in sync with src"
  fi
fi

# --- .NET ---
echo "--- .NET tests ---"
dotnet test tests/DmbSidecar.Api.Tests/DmbSidecar.Api.Tests.csproj \
  -c Release \
  --verbosity quiet \
  --nologo
ok ".NET tests passed"

# --- Python ---
echo "--- Python tests ---"
if [[ -d .venv ]]; then
  # shellcheck disable=SC1091
  source .venv/bin/activate
fi
python3 -m pytest tests/python -q --tb=line
ok "Python tests passed"

# --- Extension unit tests ---
echo "--- Extension tests ---"
NPM_CACHE="${NPM_CONFIG_CACHE:-$ROOT/.npm-cache}"
mkdir -p "$NPM_CACHE"
(cd extension && npm test --silent)
ok "Extension tests passed"

echo ""
echo -e "${GREEN}Quality gate passed.${NC}"
