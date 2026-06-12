# Setup (detailed)

## Prerequisites

- macOS or Linux, Chrome, .NET 8 SDK, Node 20+, Python 3.11+
- Sibling repos (optional for full MCP): [DiamondMind](https://github.com/OriginalDopey/DiamondMind), [dmb-mcp-server](https://github.com/OriginalDopey/dmb-mcp-server)

## Clone and configure

```bash
git clone https://github.com/OriginalDopey/dmb-sidecar.git
cd dmb-sidecar
cp .env.local.example .env.local
# Edit paths and DMB_ENTRY_TEAM_ID
./scripts/install-git-hooks.sh   # pre-commit quality gate
```

## dmb-mcp-server (optional — live league data)

1. Valid `config/leagues.json` with your entry team
2. `DiamondMind/.is_session` from `dmb_mcp.cli auth`
3. At least one `scrape --mode refresh` so SQLite has data

Lineup Lab works offline with `data/player-pool/` CSVs only.

## Foundry (optional)

See [OPTIONAL_FOUNDRY.md](OPTIONAL_FOUNDRY.md).

## Run and verify

```bash
./scripts/start-dev.sh          # terminal 1
./scripts/verify-stack.sh     # terminal 2
cd extension && npm run build
# Chrome → Load unpacked → extension/
```

## Quality before you commit

```bash
./scripts/pre-commit-quality.sh   # or rely on installed git hook
./scripts/ci.sh                   # full CI mirror + SBOM
```
