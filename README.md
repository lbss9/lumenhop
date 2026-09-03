<p align="center">
  <img src="src/Lumenhop/Assets/Lumenhop.png" width="120" alt="Lumenhop" />
</p>

<h1 align="center">Lumenhop</h1>

<p align="center">
  <strong>A quiet ping monitor for Windows.</strong><br />
  A night-acrylic pane in the corner you choose, showing whether what matters is up.
</p>

<p align="center">
  <a href="https://github.com/lbss9/lumenhop/releases/latest">
    <img src="https://img.shields.io/github/v/release/lbss9/lumenhop?style=flat-square&color=2EE6C7&label=release" alt="Latest release" />
  </a>
  <a href="https://github.com/lbss9/lumenhop/releases">
    <img src="https://img.shields.io/github/downloads/lbss9/lumenhop/total?style=flat-square&color=2EE6C7&label=downloads" alt="Downloads" />
  </a>
  <img src="https://img.shields.io/badge/Windows-10%20%2F%2011-0078D4?style=flat-square&logo=windows&logoColor=white" alt="Windows 10 / 11" />
  <img src="https://img.shields.io/badge/WinUI-3-59C8C8?style=flat-square" alt="WinUI 3" />
  <img src="https://img.shields.io/badge/.NET-8-512BD4?style=flat-square&logo=dotnet&logoColor=white" alt=".NET 8" />
  <img src="https://img.shields.io/badge/i18n-pt--BR%20%C2%B7%20en-2EE6C7?style=flat-square" alt="pt-BR and English" />
</p>

<p align="center">
  <a href="#install">Install</a> ·
  <a href="#what-it-does">Features</a> ·
  <a href="#build-from-source">Build</a> ·
  <a href="#releasing">Releasing</a> ·
  <a href="CHANGELOG.md">Changelog</a>
</p>

---

## Why Lumenhop

Not a dashboard. Not a clone. Lumenhop is a single acrylic flyout that lives in the
corner you pick and tells you, at a glance, whether your targets are online — then
gets out of the way.

- **Native.** WinUI 3 on .NET 8, self-contained, no runtime to install.
- **Continuous.** ICMP in a loop, per target, with its own interval.
- **Quiet.** Close or minimize and it keeps measuring from the tray.
- **Yours.** Fluent icons or your own image, four corners, light/dark, two languages.

---

## What it does

Each target becomes a living card:

<table>
  <tr>
    <td align="center">🌐</td>
    <td>
      <strong>Title</strong> on top<br />
      <code>IP or host</code> below, in mono
    </td>
    <td align="center">
      animated dot<br />
      <code>15ms</code> · <code>off</code>
    </td>
  </tr>
</table>

Tap the dot to turn a target on or off. The card menu edits or removes it.

<p align="center">
  <code>●</code> <strong>cyan</strong> online &nbsp;·&nbsp;
  <code>●</code> <strong>amber</strong> slow ≥ 200 ms &nbsp;·&nbsp;
  <code>●</code> <strong>red</strong> offline &nbsp;·&nbsp;
  <code>●</code> <strong>gray</strong> off
</p>

| | |
| :--- | :--- |
| **Continuous ping** | ICMP in a loop, with a per-target interval (1–60 s). |
| **The corner you pick** | Four quadrants in Settings. The flyout opens where you mark. |
| **Quiet tray** | Leaves the screen, keeps measuring. Single or double-click to open; right-click for Open / Close. |
| **Your look** | Fluent icons or your own image. Editable title, host, and interval. |
| **Acrylic, tuned** | Toggle the glass and dial the opacity to taste. Follows your light/dark theme. |
| **Two languages** | Follows the system, or locks to Portuguese (Brazil) / English. |
| **Updates itself** | An MSI install reads the changelog and lets you choose when to install. |

First launch includes Cloudflare (`1.1.1.1`) and Google DNS (`8.8.8.8`).
Everything lives in `%LOCALAPPDATA%\Lumenhop`.

---

## Install

Grab **`Lumenhop-win.msi`** from the [latest release](https://github.com/lbss9/lumenhop/releases/latest).
It installs per-user into `%LOCALAPPDATA%\Lumenhop` — no admin prompt.

> [!NOTE]
> The installer isn't code-signed yet, so Windows SmartScreen may warn on first run.
> Choose **More info → Run anyway**. Only the MSI from Releases self-updates —
> `dotnet run` and the `.exe` under `bin/` do not, and that's expected.

---

## Build from source

Requires the **.NET 8 SDK** on **Windows 10 1809+** (Windows 11 recommended).

```powershell
dotnet test Lumenhop.sln
dotnet build src/Lumenhop/Lumenhop.csproj -c Debug
```

The debug executable lands at:

```
src/Lumenhop/bin/Debug/net8.0-windows10.0.19041.0/win-x64/Lumenhop.exe
```

Pack a self-contained release (MSI via [Velopack](https://velopack.io)):

```powershell
pwsh scripts/pack.ps1
```

---

## Project layout

```
src/Lumenhop/
  Logic/       pure, testable core   (ping, validation, placement, i18n)
  Services/    side effects          (monitor loop, settings, updates, tray)
  Models/      data + view-models
  Pages/       Home · Outage · Settings · About
  Controls/    StatusDot · CornerPicker · TargetEditorDialog
tests/         xUnit over the Logic layer
scripts/       pack.ps1 and icon generators
```

The `Logic` layer holds no UI, so it's unit-tested without spinning up WinUI.

---

## Releasing

`<Version>` in `src/Lumenhop/Lumenhop.csproj` is the single source of truth.
When it lands on `main`, CI does the rest — **no manual tagging**:

```
bump <Version>  →  push to main  →  Tag workflow creates vX.Y.Z  →  Release workflow ships the MSI
```

Full guide: **[docs/RELEASING.md](docs/RELEASING.md)**.

| | Role |
| :--- | :--- |
| `main` | code and releases |
| tag `vX.Y.Z` | created automatically when `<Version>` lands on `main` |

---

<p align="center">
  <sub>Windows · WinUI 3 · .NET 8 · pt-BR and English</sub>
</p>
