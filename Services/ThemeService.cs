using Microsoft.JSInterop;

namespace DND.Services;

/// <summary>
/// <see cref="IThemeService"/> implementation backed by the hand-written localStorage/matchMedia
/// interop module at wwwroot/js/theme.js. Every interop call catches <see cref="JSException"/>
/// so a theme failure never trips Blazor's fatal-error UI; the page simply keeps whatever theme
/// the pre-boot script already applied.
/// </summary>
public sealed class ThemeService(IJSRuntime jsRuntime) : IThemeService, IAsyncDisposable
{
    private const string DefaultTheme = "light";

    private readonly Lazy<Task<IJSObjectReference>> _module = new(() =>
        jsRuntime.InvokeAsync<IJSObjectReference>("import", "./js/theme.js").AsTask());

    /// <inheritdoc />
    public string CurrentTheme { get; private set; } = DefaultTheme;

    /// <inheritdoc />
    public event Action? ThemeChanged;

    /// <inheritdoc />
    public async Task InitializeAsync()
    {
        try
        {
            var module = await _module.Value;
            var stored = await module.InvokeAsync<string?>("getTheme");
            var theme = stored ?? await module.InvokeAsync<string>("getSystemPreference");

            if (theme != CurrentTheme)
            {
                CurrentTheme = theme;
                ThemeChanged?.Invoke();
            }
        }
        catch (JSException)
        {
            // Leave CurrentTheme at its default; the pre-boot script already themed the page.
        }
    }

    /// <inheritdoc />
    public async Task SetThemeAsync(string theme)
    {
        try
        {
            var module = await _module.Value;
            await module.InvokeVoidAsync("setTheme", theme);

            CurrentTheme = theme;
            ThemeChanged?.Invoke();
        }
        catch (JSException)
        {
            // Interop failed; leave the theme unchanged rather than risking an inconsistent state.
        }
    }

    /// <inheritdoc />
    public async ValueTask DisposeAsync()
    {
        if (_module.IsValueCreated)
        {
            var module = await _module.Value;
            await module.DisposeAsync();
        }
    }
}
