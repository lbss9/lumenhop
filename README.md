<p align="center">
  <img src="src/Lumenhop/Assets/Lumenhop.png" width="128" alt="Lumenhop" />
</p>

<h1 align="center">Lumenhop</h1>

<p align="center">
  A Windows ping monitor.
</p>

<p align="center">
  <a href="https://github.com/lbss9/lumenhop/releases"><img src="https://img.shields.io/badge/version-1.0.0-2EE6C7?style=flat-square" alt="1.0.0" /></a>
  <img src="https://img.shields.io/badge/Windows-10%20%2F%2011-0078D4?style=flat-square&logo=windows&logoColor=white" alt="Windows" />
  <img src="https://img.shields.io/badge/WinUI-3-59C8C8?style=flat-square" alt="WinUI 3" />
  <img src="https://img.shields.io/badge/.NET-8-512BD4?style=flat-square&logo=dotnet&logoColor=white" alt=".NET 8" />
  <img src="https://img.shields.io/badge/language-pt--BR%20·%20en-2EE6C7?style=flat-square" alt="pt-BR and English" />
</p>

<p align="center">
  Native flyout in the corner. Living cards. Continuous ping. No noise.
</p>

---

## What it is

Lumenhop is a ping monitor for Windows only. Not a dashboard. Not a clone. A night acrylic pane with a cyan aurora accent that sits in the corner you pick and shows whether what matters is up.

Each target becomes a card:

| Icon | Target | Pulse |
| :---: | :--- | :---: |
| yours | title on top<br />IP or host below, in mono | animated dot<br />`15ms` · `off` |

Tap the dot to turn a target on or off. The card menu edits or removes it.

<p align="center">
  <code>cyan</code> online &nbsp;·&nbsp; <code>amber</code> slow ≥ 200&nbsp;ms &nbsp;·&nbsp; <code>red</code> offline &nbsp;·&nbsp; <code>gray</code> off
</p>

Close or minimize to the tray. Ping keeps running.

---

## What it does

| | |
| :--- | :--- |
| **Continuous ping** | ICMP in a loop, with a per-target interval (1–60 s). |
| **The corner you pick** | Four quadrants in Settings. The window opens where you mark. |
| **Quiet tray** | Leaves the screen. Keeps measuring in the background. Single or double-click opens it. Right-click has Open and Close. |
| **Your look** | Fluent icons or your own image. Editable title, host, and interval. |
| **Two languages** | Follows the system, or locks to Portuguese (Brazil) / English. |
| **Updates itself** | An MSI install gets the notice, reads the changelog, and you choose when to install. |

First launch includes Cloudflare (`1.1.1.1`) and Google DNS (`8.8.8.8`).  
Everything lives in `%LOCALAPPDATA%\Lumenhop`.

---

## Install

The installer is on [Releases](https://github.com/lbss9/lumenhop/releases): `Lumenhop-win.msi`.

`dotnet run` and the `.exe` under `bin` are for development. They do not self-update. That is expected.

---

## Develop

```powershell
dotnet test Lumenhop.sln
dotnet build src/Lumenhop/Lumenhop.csproj -c Debug
```

```
src/Lumenhop/bin/Debug/net8.0-windows10.0.19041.0/win-x64/Lumenhop.exe
```

Windows 10 1809+ · Windows 11 recommended · .NET 8 SDK

Pack a release:

```powershell
pwsh scripts/pack.ps1
```

Full guide: [docs/RELEASING.md](docs/RELEASING.md).

| | Role |
| :--- | :--- |
| `main` | code and releases |
| tag `vX.Y.Z` | created automatically when `<Version>` lands on `main` |

---

<p align="center">
  <sub>Windows · WinUI 3 · pt-BR and English</sub>
</p>
