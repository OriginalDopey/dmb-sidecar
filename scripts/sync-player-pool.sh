#!/usr/bin/env bash
# Copy latest SearchResults CSV exports from Downloads into data/player-pool/
set -euo pipefail
ROOT="$(cd "$(dirname "$0")/.." && pwd)"
DEST="$ROOT/data/player-pool"
DL="$HOME/Downloads"
mkdir -p "$DEST"

latest() {
  ls -t "$DL"/SearchResults*.csv 2>/dev/null | head -1
}

# Heuristic: fielding export has FLD: columns; splits has OBP vs L
for f in "$DL"/SearchResults*.csv; do
  [[ -f "$f" ]] || continue
  if head -1 "$f" | grep -q 'FLD:C'; then
    cp "$f" "$DEST/hitters-fielding.csv"
    echo "fielding ← $(basename "$f")"
  elif head -1 "$f" | grep -q 'OBP vs L'; then
    cp "$f" "$DEST/hitters-splits.csv"
    echo "splits ← $(basename "$f")"
  elif head -1 "$f" | grep -q 'PB:'; then
    cp "$f" "$DEST/catchers.csv"
    echo "catchers ← $(basename "$f")"
  fi
done

echo "Player pool in $DEST:"
ls -la "$DEST"
