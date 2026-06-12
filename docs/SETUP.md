# Setup (detailed)

## Install toolchain

```bash
brew install dotnet@8
echo 'export DOTNET_ROOT="/opt/homebrew/opt/dotnet@8/libexec"' >> ~/.zshrc
echo 'export PATH="/opt/homebrew/opt/dotnet@8/bin:$PATH"' >> ~/.zshrc
```

## Clone / init repo

```bash
cd /Users/originaldopey/Documents/CursonProjects/dmb-sidecar
git init
cp .env.local.example .env.local
```

## dmb-mcp-server prerequisites

1. `dmb-mcp-server` installed with `config/leagues.json` containing your entry team
2. `DiamondMind/.is_session` valid (`python3.11 -m dmb_mcp.cli auth --cookie "..."`)
3. At least one `scrape --mode refresh` run so SQLite has data

## Foundry

Follow [manual-steps/FOUNDRY_IQ_PORTAL.md](manual-steps/FOUNDRY_IQ_PORTAL.md)

## Verify stack

```bash
# Terminal 1
./scripts/start-dev.sh

# Terminal 2
curl http://127.0.0.1:5280/health
curl http://127.0.0.1:8765/health
curl -X POST http://127.0.0.1:5280/foundry/smoke -H "X-Api-Key: dev-key-change-me"
```

## Extension

[manual-steps/CHROME_LOAD_UNPACKED.md](manual-steps/CHROME_LOAD_UNPACKED.md)
