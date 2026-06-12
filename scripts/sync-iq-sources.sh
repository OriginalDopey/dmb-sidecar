#!/usr/bin/env bash
# Sync curated markdown into iq-sources/ for Foundry IQ upload.
# Run from repo root: ./scripts/sync-iq-sources.sh

set -euo pipefail
ROOT="$(cd "$(dirname "$0")/.." && pwd)"
DM="/Users/originaldopey/Documents/CursonProjects/DiamondMind"
NDMO="/Users/originaldopey/Documents/CursonProjects/New-DMO"
OUT="$ROOT/iq-sources"

mkdir -p "$OUT"/{rules,strategy,defense,knowledge,configuration}

copy() {
  local src="$1" dest="$2"
  if [[ -f "$src" ]]; then
    cp "$src" "$dest"
    echo "  OK $(basename "$dest")"
  else
    echo "  SKIP missing: $src"
  fi
}

echo "Syncing IQ sources to $OUT ..."

# Rules (mechanics)
copy "$DM/rules/dmb_unified_rules.md" "$OUT/rules/dmb_unified_rules.md"
copy "$DM/rules/defensive_ratings_range_error_tradeoff.md" "$OUT/rules/defensive_ratings_range_error_tradeoff.md"
copy "$DM/rules/rc_runs_created_formula.md" "$OUT/rules/rc_runs_created_formula.md"

# Strategy (curated — bankroll + valuation highlights)
for f in \
  "$DM/strategy/dmb_strategy_guide.md" \
  "$DM/strategy/PLATOON_VALUE_ANALYSIS.md" \
  ; do
  if [[ -f "$f" ]]; then
    cp "$f" "$OUT/strategy/$(basename "$f")"
    echo "  OK strategy/$(basename "$f")"
  fi
done

# Defensive analysis
if [[ -d "$DM/defensive_analysis" ]]; then
  find "$DM/defensive_analysis" -maxdepth 1 -name '*.md' -exec cp {} "$OUT/defense/" \;
  echo "  OK defense/*.md ($(ls -1 "$OUT/defense" 2>/dev/null | wc -l | tr -d ' ') files)"
fi

# New-DMO knowledge advisors
for f in "$NDMO/knowledge"/*-advisor.md "$NDMO/knowledge/team-player-settings-map.md"; do
  [[ -f "$f" ]] && copy "$f" "$OUT/knowledge/$(basename "$f")"
done
copy "$NDMO/knowledge/decision_trees/lineup_construction.md" "$OUT/knowledge/lineup_construction.md"
copy "$NDMO/knowledge/decision_trees/team_instructions.md" "$OUT/knowledge/team_instructions_decision_tree.md"
copy "$NDMO/knowledge/decision_trees/player_instructions.md" "$OUT/knowledge/player_instructions_decision_tree.md"

# Configuration surface
if [[ -f "$NDMO/docs/DMB_CONFIGURATION_SURFACE_FOR_DISCOVERY.md" ]]; then
  copy "$NDMO/docs/DMB_CONFIGURATION_SURFACE_FOR_DISCOVERY.md" "$OUT/configuration/configuration_surface.md"
elif [[ -f "$DM/configuration" ]]; then
  find "$DM/configuration" -maxdepth 1 -name '*.md' 2>/dev/null | head -3 | while read -r f; do
    copy "$f" "$OUT/configuration/$(basename "$f")"
  done
fi

# Manifest for upload checklist
MANIFEST="$OUT/MANIFEST.md"
{
  echo "# IQ Sources Manifest"
  echo "Generated: $(date -u +%Y-%m-%dT%H:%MZ)"
  echo ""
  echo "Upload all \`.md\` files below to Foundry IQ knowledge base \`dmb-classic-standard-kb\`."
  echo ""
  find "$OUT" -name '*.md' ! -name MANIFEST.md | sort | while read -r f; do
    echo "- \`${f#$OUT/}\` ($(wc -c <"$f" | tr -d ' ') bytes)"
  done
} >"$MANIFEST"

echo ""
echo "Done. $(find "$OUT" -name '*.md' | wc -l | tr -d ' ') markdown files ready."
echo "Next: docs/manual-steps/FOUNDRY_IQ_PORTAL.md (upload in ai.azure.com)"
