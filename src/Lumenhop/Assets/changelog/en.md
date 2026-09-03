# 1.3.1

3 September 2026 · a cleaner header

- The Home header no longer shows the status count line (`4 online · 1 offline · …`).

---

# 1.3.0

3 September 2026 · start and stop, all at once

- **Start all** and **Stop all** buttons on Home, next to Add — turn every target on or off in a single tap.

---

# 1.2.1

3 September 2026 · import/export moved to Settings

- Import and export now live in **Settings**, next to Quit, instead of the Home menu.

---

# 1.2.0

3 September 2026 · share your targets

- **Import and export** your list of targets, from the `⋯` menu on Home. Export writes a signed `.lumenhop` file — title, host, icon and interval — that anyone can import. The importer verifies the file's signature and checksum, skips hosts you already have, and never carries over local-only data like custom icon files.

---

# 1.1.2

3 September 2026 · steady latency

- The latency number **no longer flickers on each poll**. It used to blink to `…` between readings; now it updates in place, and the probing hint shows only on the first reading. The dot no longer flashes on every cycle either.

---

# 1.1.1

3 September 2026 · the dot matches the number

- The status dot now takes the **same color as the latency number**. It was stuck on gray while the number was already colored by band.

---

# 1.1.0

3 September 2026 · colors you choose

- **Configurable latency colors** in Settings. Four bands — great, good, fair, poor — each with an editable threshold and its own color picker. The dot and the latency number take the band's color. Restore the defaults anytime.

---

# 1.0.2

3 September 2026 · updates that find you

- The app now **checks for updates while it's open**, not only at launch — a quiet re-check every 6 hours.
- When a new version is ready, a **tray notification** appears — open Lumenhop to install. The update window no longer pops in front of you on its own.

---

# 1.0.1

3 September 2026 · polish and stability

- The side navigation is now a **fixed icon rail** — the "Open navigation" toggle and its collapse animation are gone, so no more stutter when the pane snapped shut.
- Hardened crash logging: writing `crash.log` can never mask the original error.

---

# 1.0.0

31 August 2026 · first public release

A Windows ping monitor. Flyout in the corner, living cards, continuous ping.

Installer: `Lumenhop-win.msi`.

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
- Single or double-click the tray icon to open; right-click for Open / Close
- Light, dark, or system theme
- Acrylic and opacity controls
- System language, Portuguese (Brazil), or English
- Start with Windows

## Updates

- In-app notice when a new version ships
- Changelog in the current language
- Install now, or later from Settings

Preferences live in `%LOCALAPPDATA%\Lumenhop`.
