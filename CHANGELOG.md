# Changelog

Every change, newest first. The same notes appear in the app, in the current language:

- [Portuguese](src/Lumenhop/Assets/changelog/pt-BR.md)
- [English](src/Lumenhop/Assets/changelog/en.md)

This project follows [Semantic Versioning](https://semver.org): `MAJOR.MINOR.PATCH`.

---

## 1.1.2 — 3 September 2026

### Fixed

- The latency number **no longer flickers on each poll**. It used to blink to `…`
  between readings; now it updates in place, and the probing hint shows only on the
  first reading. The dot no longer flashes on every cycle either.

---

## 1.1.1 — 3 September 2026

### Fixed

- The status dot now takes the **same color as the latency number**. It was stuck on
  gray while the number was already colored by band.

---

## 1.1.0 — 3 September 2026

Colors you choose.

### Added

- **Configurable latency colors** in Settings. Four bands — great, good, fair, poor —
  each with an editable threshold and its own color picker. The status dot and the
  latency number both take the band's color. Restore the defaults anytime.

---

## 1.0.2 — 3 September 2026

Updates that find you.

### Added

- **Automatic update checks while the app is open**, not just at launch — a quiet
  re-check every 6 hours.
- A **discreet tray notification** when a new version is ready — open Lumenhop to
  install. The update window no longer pops in front of you on its own.

---

## 1.0.1 — 3 September 2026

Polish and stability.

### Changed

- The side navigation is now a **fixed icon rail**. The "Open navigation" toggle and
  its collapse animation are gone — no more stutter when the pane snapped shut.

### Fixed

- Crash logging is hardened: writing `crash.log` can never itself throw and mask the
  original error.

### Internal

- Removed dead pane-glass code left over from the collapsible pane.

---

## 1.0.0 — 31 August 2026

First public release.

A Windows ping monitor. Flyout in the corner, living cards, continuous ping.
Installer: `Lumenhop-win.msi` (per-user, `%LOCALAPPDATA%\Lumenhop`).

### Targets

- Card with **icon**, **title**, **IP or host**, and **latency**
- Pulse dot: `cyan` online · `amber` slow (≥ 200 ms) · `red` offline · `gray` off
- Tap the dot to turn a target on or off
- Add, edit, and remove
- Per-target interval, 1 to 60 seconds
- Fluent icons or your own image
- Paste a URL, host, or IP — the app normalizes it
- First launch includes Cloudflare (`1.1.1.1`) and Google DNS (`8.8.8.8`)

### Window

- WinUI 3 flyout, 400 × 600, not maximizable
- Four corners in Settings
- Close or minimize to the tray — ping keeps running
- Single or double-click the tray icon to open; right-click for Open / Close
- Light, dark, or system theme
- Acrylic and opacity controls
- System language, Portuguese (Brazil), or English
- Start with Windows

### Updates

- In-app notice when a new version ships
- Changelog in the current language
- Install now, or later from Settings
