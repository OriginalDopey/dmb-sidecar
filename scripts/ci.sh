#!/usr/bin/env bash
# Local CI mirror — run before interviews or opening a PR.
set -euo pipefail
ROOT="$(cd "$(dirname "$0")/.." && pwd)"
cd "$ROOT"

echo "=== .NET build + test + coverage ==="
dotnet restore dmb-sidecar.sln
dotnet build dmb-sidecar.sln --configuration Release --no-restore
dotnet test tests/DmbSidecar.Api.Tests/DmbSidecar.Api.Tests.csproj \
  --configuration Release \
  --no-build \
  --settings tests/DmbSidecar.Api.Tests/coverlet.runsettings \
  --collect:"XPlat Code Coverage" \
  --results-directory "$ROOT/TestResults"

echo "=== Python bridge tests ==="
python3 -m pip install -q -r src/DmbSidecar.McpBridge/requirements.txt \
  -r src/DmbSidecar.McpBridge/requirements-dev.txt
pytest tests/python -q

echo "=== Extension build + test ==="
NPM_CACHE="${NPM_CONFIG_CACHE:-$ROOT/.npm-cache}"
mkdir -p "$NPM_CACHE"
(cd extension && npm ci --cache "$NPM_CACHE" && npm run build && npm test)

echo "=== SBOM (local artifacts in sbom/) ==="
mkdir -p sbom
if command -v dotnet-CycloneDX >/dev/null 2>&1 || dotnet tool list -g | grep -q CycloneDX; then
  dotnet tool install --global CycloneDX 2>/dev/null || true
  export PATH="$PATH:$HOME/.dotnet/tools"
  dotnet-CycloneDX src/DmbSidecar.Api/DmbSidecar.Api.csproj -o sbom/dotnet -fn sbom-dotnet.json -F Json
fi
python3 -m pip install -q cyclonedx-bom
cyclonedx-py requirements src/DmbSidecar.McpBridge/requirements.txt -o sbom/sbom-python.json
(cd extension && npm exec --cache "$NPM_CACHE" -- @cyclonedx/cyclonedx-npm --output-file ../sbom/sbom-extension.json)

echo "CI mirror complete."
