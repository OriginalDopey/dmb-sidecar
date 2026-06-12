#!/usr/bin/env bash
# Configure branch protection + rules for main (requires GitHub Pro OR public repo).
set -euo pipefail

REPO="${GITHUB_REPO:-OriginalDopey/dmb-sidecar}"
BRANCH="${PROTECT_BRANCH:-main}"

echo "Configuring protection for ${REPO}@${BRANCH}..."

if ! gh api "repos/${REPO}/branches/${BRANCH}/protection" \
  --method PUT \
  --input - <<'EOF'
{
  "required_status_checks": {
    "strict": true,
    "contexts": [
      ".NET API — build, test, coverage, SBOM",
      "MCP bridge — test, SBOM",
      "Chrome extension — build, test, SBOM"
    ]
  },
  "enforce_admins": true,
  "required_pull_request_reviews": {
    "dismiss_stale_reviews": true,
    "require_code_owner_reviews": false,
    "required_approving_review_count": 0
  },
  "restrictions": null,
  "required_linear_history": false,
  "allow_force_pushes": false,
  "allow_deletions": false,
  "block_creations": false,
  "required_conversation_resolution": true
}
EOF
then
  cat >&2 <<'MSG'

⚠ Branch protection API failed.

Private repos on GitHub Free cannot use branch protection or rulesets.
Options:
  1. Upgrade to GitHub Pro (Settings → Billing)
  2. Make the repo public (Settings → Danger zone)
  3. Apply manually: Settings → Branches → Add rule for "main"

Until then, follow the team policy in docs/GITHUB_BEST_PRACTICES.md:
  - Never push directly to main — use PRs only
  - Wait for all three CI jobs green before merge

MSG
  exit 1
fi

echo "✓ Branch protection enabled on ${BRANCH}"
echo "  Required checks: dotnet, python, extension CI jobs"
echo "  Force push: blocked | Admin bypass: blocked"
