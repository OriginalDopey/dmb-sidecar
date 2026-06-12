# Security

## Reporting

Report vulnerabilities privately to the repository owner. Do not open public issues for exploit details.

## Surface area

| Component | Exposure | Notes |
|-----------|----------|-------|
| ASP.NET API (`:5280`) | Localhost by default | `X-Api-Key` required except `/health` and `/swagger` |
| MCP bridge (`:8765`) | Localhost by default | No auth — intended for local dev only |
| Chrome extension | User's browser | Reads ImagineSports DOM only; calls configured API URL |

## Secrets

- Never commit `.env.local`, `.is_session`, or production API keys.
- Rotate `Security:ApiKey` in `appsettings.json` before any shared deployment.

## Dependencies

CI generates CycloneDX SBOM artifacts (`.NET`, Python, npm) on every push to `main`. Review `sbom-*` workflow artifacts after dependency updates.
