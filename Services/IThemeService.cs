namespace DND.Services;

/// <summary>
/// Exposes the app's light/dark theme preference, backed by the localStorage-based JS interop
/// module at wwwroot/js/theme.js, and notifies subscribers when it changes so the UI (starting
/// with <see cref="DND.Components.ThemeToggle"/>) can re-render.
/// </summary>
public interface IThemeService
{
    /// <summary>
    /// The current theme, "light" or "dark".
    /// </summary>
    string CurrentTheme { get; }

    /// <summary>
    /// Raised whenever <see cref="CurrentTheme"/> changes.
    /// </summary>
    event Action? ThemeChanged;

    /// <summary>
    /// Reconciles this service's notion of the current theme with what is actually applied to
    /// the document. The pre-boot inline script in wwwroot/index.html has already applied a
    /// theme (from localStorage, falling back to the OS preference) before Blazor starts, so
    /// this only synchronizes state - it does not itself change what is on screen.
    /// </summary>
    Task InitializeAsync();

    /// <summary>
    /// Applies and persists an explicit user choice, "light" or "dark".
    /// </summary>
    Task SetThemeAsync(string theme);
}
