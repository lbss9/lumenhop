# How to cut a release

Lumenhop is an unpackaged WinUI 3 app. The installer and auto-update use [Velopack](https://docs.velopack.io) from **GitHub Releases**.

Commits, pull requests, release titles, and release notes are written in English.

## Version numbers (SemVer)

One number, three parts: `MAJOR.MINOR.PATCH`.

| Part | When it goes up | Example |
|---|---|---|
| **MAJOR** | A change that breaks what the user already has | `1.0.0` → `2.0.0` |
| **MINOR** | A new feature that does not break existing use | `1.0.0` → `1.1.0` |
| **PATCH** | A fix | `1.0.0` → `1.0.1` |

The first public version is **1.0.0**. Do not use `0.x` from here on — that reads as “not a product yet”. Do not use four numbers (`1.0.0.123`): Windows fills the fourth itself; the tag and Velopack use only three.

`<Version>` in `src/Lumenhop/Lumenhop.csproj` is the source of truth. When it lands on `main`, the `Tag` workflow creates `vX.Y.Z` if that tag does not exist, then starts the `Release` workflow. GitHub does not chain workflows from a `GITHUB_TOKEN` tag push, so Tag dispatches Release itself. The Release publishes only the MSI.

If the csproj version and the tag diverge, the updater cannot find the right package.

The feed repo is `https://github.com/lbss9/lumenhop` (public, so the app can download the Release without a token). The release build only uses that compiled URL. `LUMENHOP_REPO_URL` is for CI / `scripts/pack.ps1`.

## Once

```powershell
dotnet tool install -g vpk
```

## Cut a version

Everything starts from `main`. Open a `feat/...` or `fix/...` branch, then a PR into `main`.

1. Update `src/Lumenhop/Assets/changelog/pt-BR.md` and `en.md`.
2. Bump `<Version>` in `src/Lumenhop/Lumenhop.csproj` (e.g. `1.0.1`).
3. Open the PR to `main` and merge it.

The `v1.0.1` tag and the Release are created for you. Do not create the tag by hand.

## Pack on your machine

```powershell
pwsh scripts/pack.ps1
# or
pwsh scripts/pack.ps1 -Version 1.0.1
```

Output in `artifacts/release/`:

- `Lumenhop-win.msi` — the only file published on the Release (per-user, `%LOCALAPPDATA%\Lumenhop`)

To upload to GitHub:

```powershell
pwsh scripts/pack.ps1 -Version 1.0.1 -Upload
```

`GH_TOKEN` needs permission to create Releases.

## What the user sees

Builds installed from the MSI notify in the app and under **Settings → Updates**.  
`dotnet run` and the `.exe` under `bin` do not self-update — that is expected.
