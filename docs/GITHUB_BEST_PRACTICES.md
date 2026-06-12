# GitHub best practices

How this repo is configured for safe, reviewable merges.

---

## Branch policy

| Rule | Status |
|------|--------|
| Default branch | `main` |
| Direct pushes to `main` | **Discouraged** — use PRs |
| Merge method | **Squash merge only** (merge commits & rebase disabled) |
| Delete head branch after merge | **Enabled** |
| CI on every PR | **Yes** — [.github/workflows/ci.yml](../.github/workflows/ci.yml) |
| Dependabot | **Weekly** — NuGet, npm, pip, GitHub Actions |
| CODEOWNERS | [@OriginalDopey](../.github/CODEOWNERS) |

### Branch protection on `main`

**Current blocker:** This repo is **private** on **GitHub Free**. Branch protection and rulesets require **GitHub Pro** or a **public** repository.

When you have Pro or go public, run:

```bash
./scripts/configure-github-protection.sh
```

That enables:

- Required status checks (all 3 CI jobs, strict / up-to-date)
- No force push
- No admin bypass
- Resolved conversations required before merge

**Manual setup (UI):** Settings → Branches → Add rule → branch name `main`:

1. ☑ Require a pull request before merging
2. ☑ Require status checks to pass — select all three CI job names
3. ☑ Require branches to be up to date before merging
4. ☑ Require conversation resolution before merging
5. ☐ Do not allow bypassing the above settings (admins included)
6. ☑ Block force pushes

---

## Workflow for contributors

```bash
git checkout -b feat/my-change
# ... edit ...
./scripts/pre-commit-quality.sh
git push -u origin feat/my-change
gh pr create
# Wait for CI green → Squash and merge
```

Never `git push origin main` for feature work.

**Local enforcement:** `./scripts/install-git-hooks.sh` installs a `pre-push` hook that rejects pushes to `main`. Emergency override: `DMB_SIDECAR_ALLOW_MAIN_PUSH=1 git push origin main`.

---

## Repo hygiene (already applied)

| Setting | Value |
|---------|-------|
| Squash merge | Enabled |
| Merge commit | Disabled |
| Rebase merge | Disabled |
| Delete branch on merge | Enabled |
| Issues | Enabled (structured templates) |
| Wiki | Disabled |
| PR template | [.github/pull_request_template.md](../.github/pull_request_template.md) |
| Security policy | [SECURITY.md](../SECURITY.md) |

---

## CI required checks (names for branch protection)

When configuring protection, require these **exact** job names from Actions:

1. `.NET API — build, test, coverage, SBOM`
2. `MCP bridge — test, SBOM`
3. `Chrome extension — build, test, SBOM`

---

## Making the repo interview-visible

If reviewers need read access without Pro:

- **Public repo** — enables branch protection on Free; link in resume
- **Private + collaborator** — add reviewer as read collaborator (no protection on Free)

Recommended for code-review samples: **public** + branch protection script.
