#!/usr/bin/env bash
set -euo pipefail
ROOT="$(cd "$(dirname "$0")/.." && pwd)"
cd "$ROOT"

chmod +x scripts/pre-commit-quality.sh .githooks/pre-commit .githooks/pre-push
git config core.hooksPath .githooks
echo "Installed hooks from .githooks/:"
echo "  pre-commit → scripts/pre-commit-quality.sh"
echo "  pre-push   → blocks direct push to main"
echo "Skip once: git commit --no-verify | DMB_SIDECAR_ALLOW_MAIN_PUSH=1 git push"
