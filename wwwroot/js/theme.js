// ES module wrapping localStorage + matchMedia for the app's light/dark theme preference.
// Deliberately does NOT use IndexedDB (ticket 208): the pre-boot script in index.html must read
// the stored preference synchronously, before first paint, to avoid a light-mode flash, and
// IndexedDB access is always asynchronous. STORAGE_KEY must stay identical to the inline
// pre-boot script in wwwroot/index.html; a mismatch silently breaks persistence.
const STORAGE_KEY = "dnd-theme";

/**
 * Reads the stored theme preference. Returns "light", "dark", or null when nothing is stored
 * (or localStorage is unavailable), in which case the caller should fall back to the OS
 * preference via getSystemPreference().
 */
export function getTheme() {
    try {
        const value = localStorage.getItem(STORAGE_KEY);
        return value === "light" || value === "dark" ? value : null;
    } catch {
        return null;
    }
}

/**
 * Persists the given theme and applies it to the document immediately.
 */
export function setTheme(theme) {
    try {
        localStorage.setItem(STORAGE_KEY, theme);
    } catch {
        // localStorage can throw in some privacy modes; the attribute below still applies for
        // the rest of this session, it just will not persist across reloads.
    }

    document.documentElement.dataset.theme = theme;
    document.documentElement.dataset.bsTheme = theme;
}

/**
 * Returns "dark" or "light" based on the OS-level color scheme preference.
 */
export function getSystemPreference() {
    return window.matchMedia && window.matchMedia("(prefers-color-scheme: dark)").matches
        ? "dark"
        : "light";
}
