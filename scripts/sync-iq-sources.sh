#!/usr/bin/env bash
# Sync curated markdown into iq-sources/ for Foundry IQ upload.
# Run from repo root: ./scripts/sync-iq-sources.sh

set -euo pipefail
ROOT="$(cd "$(dirname "$0")/.." && pwd)"
DM="${DIAMONDMIND_ROOT:-$ROOT/../DiamondMind}"
NDMO="${NEW_DMO_ROOT:-$ROOT/../New-DMO}"
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
echo "  DiamondMind: $DM"
echo "  New-DMO:     $NDMO"

# Rules (mechanics)
copy "$DM/rules/dmb_unified_rules.md" "$OUT/rules/dmb_unified_rules.md"
copy "$DM/rules/defensive_ratings_range_error_tradeoff.md" "$OUT/rules/defensive_ratings_range_error_tradeoff.md"
copy "$DM/rules/rc_runs_created_formula.md" "$OUT/rules/rc_runs_created_formula.md"

# Strategy (curated)
for f in "$DM/strategy/dmb_strategy_guide.md"; do
  if [[ -f "$f" ]]; then
    cp "$f" "$OUT/strategy/$(basename "$f")"
    echo "  OK strategy/$(basename "$f")"
  fi
done
if [[ -f "$DM/strategy/PLATOON_VALUE_ANALYSIS.md" ]]; then
  cp "$DM/strategy/PLATOON_VALUE_ANALYSIS.md" "$OUT/strategy/PLATOON_VALUE_ANALYSIS.md"
  echo "  OK strategy/PLATOON_VALUE_ANALYSIS.md"
fi

# Defensive analysis
if [[ -d "$DM/defensive_analysis" ]]; then
  find "$DM/defensive_analysis" -maxdepth 1 -name '*.md' -exec cp {} "$OUT/defense/" \;
  echo "  OK defense/*.md ($(ls -1 "$OUT/defense" 2>/dev/null | wc -l | tr -d ' ') files)"
fi

# New-DMO knowledge advisors (skip duplicate decision_tree copies — advisors are canonical)
for f in "$NDMO/knowledge"/*-advisor.md "$NDMO/knowledge/team-player-settings-map.md"; do
  [[ -f "$f" ]] && copy "$f" "$OUT/knowledge/$(basename "$f")"
done
copy "$NDMO/knowledge/decision_trees/lineup_construction.md" "$OUT/knowledge/lineup_construction.md"

# Configuration surface
if [[ -f "$NDMO/docs/DMB_CONFIGURATION_SURFACE_FOR_DISCOVERY.md" ]]; then
  copy "$NDMO/docs/DMB_CONFIGURATION_SURFACE_FOR_DISCOVERY.md" "$OUT/configuration/configuration_surface.md"
fi

# Manifest for upload checklist
MANIFEST="$OUT/MANIFEST.md"
{
  echo "# IQ Sources Manifest"
  echo "Generated: $(date -u +%Y-%m-%dT%H:%MZ)"
  echo ""
  echo "Upload all \`.md\` files below to your Foundry IQ knowledge base."
  echo ""
  find "$OUT" -name '*.md' ! -name MANIFEST.md | sort | while read -r f; do
    echo "- \`${f#$OUT/}\` ($(wc -c <"$f" | tr -d ' ') bytes)"
  done
} >"$MANIFEST"

echo ""
echo "Done. $(find "$OUT" -name '*.md' | wc -l | tr -d ' ') markdown files ready."
echo "Optional: docs/manual-steps/FOUNDRY_IQ_PORTAL.md"
