#!/usr/bin/env bash
# Vendor dmb_config lineup modules from DiamondMind (implementation-plan source of truth).
set -euo pipefail
ROOT="$(cd "$(dirname "$0")/.." && pwd)"
SRC="${DMB_CONFIG_SCRIPTS:-$ROOT/../DiamondMind/scripts}"
DEST="$ROOT/src/DmbSidecar.McpBridge/vendor/dmb_config_scripts"

if [[ ! -d "$SRC/dmb_config" ]]; then
  echo "Missing $SRC/dmb_config — set DMB_CONFIG_SCRIPTS or clone DiamondMind alongside dmb-sidecar" >&2
  exit 1
fi

mkdir -p "$DEST/dmb_config"
for f in lineup.py rules_tables.py model.py __init__.py; do
  cp "$SRC/dmb_config/$f" "$DEST/dmb_config/"
done
echo "Synced dmb_config → $DEST/dmb_config"
ls -la "$DEST/dmb_config"
