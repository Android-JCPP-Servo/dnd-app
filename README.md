# DND

A digital version of the D&D 5e character sheet, built to make character creation and play easier.

**Live site:** https://android-jcpp-servo.github.io/dnd-app/

## Disclaimer

This is an unofficial, fan-made tool. The author claims no ownership of Dungeons & Dragons or any related game rights. The app exists only to make character creation easier, and no game content (spell text, stat tables, etc.) is bundled with it — users enter their own.

## Deployment

The site is published by [`.github/workflows/deploy.yml`](.github/workflows/deploy.yml) on every push to `main` (it can also be run manually via `workflow_dispatch`). For the deployment to work:

- GitHub Pages must be enabled under **Settings -> Pages**, with **Source** set to **GitHub Actions**.
- The repository must be public for the site to be reachable on a free personal account.

### Base-path warning

The workflow hardcodes the literal `/dnd-app/` when it rewrites `<base href>` in `wwwroot/index.html` and `const base` in the service worker (the published service worker output). If this repository is ever renamed, those literals must be updated in the **Rewrite base href for GitHub Pages subpath** and **Rewrite service worker base path** steps of `.github/workflows/deploy.yml`. Those steps include `grep -q ... || exit 1` guards, so the build will fail loudly instead of silently deploying a broken site if the literals are left stale.

## Local development

Prerequisite: the **.NET 10 SDK** (matches the deploy workflow's `dotnet-version: '10.0.x'` and the project's `net10.0` target).

```
git clone <repo-url>
cd DND
dotnet run
```

The app will be available locally at `https://localhost:7070` (or `http://localhost:5066`).
