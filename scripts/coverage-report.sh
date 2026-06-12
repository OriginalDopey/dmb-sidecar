#!/usr/bin/env bash
# Generate HTML .NET coverage report → coveragereport/index.html
set -euo pipefail
ROOT="$(cd "$(dirname "$0")/.." && pwd)"
cd "$ROOT"

echo "=== Running .NET tests with coverage ==="
dotnet test tests/DmbSidecar.Api.Tests/DmbSidecar.Api.Tests.csproj \
  -c Release \
  --settings tests/DmbSidecar.Api.Tests/coverlet.runsettings \
  --collect:"XPlat Code Coverage" \
  --results-directory "$ROOT/TestResults" \
  -v q

COV_FILE="$(find TestResults -name coverage.cobertura.xml | head -1)"
if [[ -z "$COV_FILE" ]]; then
  echo "No coverage file found." >&2
  exit 1
fi

LINE_RATE="$(grep -o 'line-rate="[0-9.]*"' "$COV_FILE" | head -1 | cut -d'"' -f2)"
PCT="$(python3 -c "print(f'{float(\"$LINE_RATE\") * 100:.1f}')")"
echo ".NET line coverage: ${PCT}%"

echo "=== Generating HTML report ==="
dotnet tool install --global dotnet-reportgenerator-globaltool 2>/dev/null || true
export PATH="$PATH:$HOME/.dotnet/tools"
reportgenerator \
  "-reports:$COV_FILE" \
  "-targetdir:$ROOT/coveragereport" \
  "-reporttypes:Html;HtmlSummary" \
  "-assemblyfilters:+DmbSidecar.Api"

echo "Open: file://$ROOT/coveragereport/index.html"
