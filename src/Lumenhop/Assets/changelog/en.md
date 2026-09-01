# 1.1.0

31 August 2026

Portable build and MSI installer on Releases.

- `Lumenhop-win-Portable.zip` — extract and run, no install
- `Lumenhop-win-Setup.msi` — Windows installer, with automatic updates

# 1.0.0

31 August 2026 · first public release

A Windows ping monitor. Flyout in the corner, living cards, continuous ping.

## Targets

- Card with **icon**, **title**, **IP or host**, and **latency**
- Pulse dot: `cyan` online · `amber` slow (≥ 200 ms) · `red` offline · `gray` off
- Tap the dot to turn a target on or off
- Add, edit, and remove
- Per-target interval, 1 to 60 seconds
- Fluent icons or your own image
- Paste a URL, host, or IP — the app normalizes it
- First launch includes Cloudflare (`1.1.1.1`) and Google DNS (`8.8.8.8`)

## Window

- WinUI 3 flyout, 400 × 600, not maximizable
- Four corners in Settings
- Close or minimize to the tray — ping keeps running
- Light, dark, or system theme
- Acrylic and opacity controls
- System language, Portuguese (Brazil), or English
- Start with Windows

## Updates

- In-app notice when a new version ships
- Changelog in the current language
- Install now, or later from Settings

Preferences live in `%LOCALAPPDATA%\Lumenhop`.
