using System.Text.Json;
using Microsoft.JSInterop;

namespace DND.Services;

/// <summary>
/// <see cref="ISheetSectionStateService"/> implementation backed directly by
/// <c>window.localStorage</c> (no wrapper JS module needed for a single key/value pair).
/// Section open/closed state is per-device by definition, so localStorage — not the
/// IndexedDB-backed <see cref="ICharacterStore"/> — is the correct store. Every operation
/// catches <see cref="JSException"/>, reports it to <see cref="IStorageStatusService"/>, and
/// returns a safe default instead of rethrowing, so a private-browsing storage denial never
/// trips Blazor's fatal-error UI.
/// </summary>
public sealed class SheetSectionStateService(IJSRuntime jsRuntime, IStorageStatusService storageStatus)
    : ISheetSectionStateService
{
    private const string StorageKey = "dnd.sectionState";
    private const string StorageFailureMessage = "Your section layout could not be saved to this browser.";

    private static readonly JsonSerializerOptions SerializerOptions = new();

    private Dictionary<string, bool>? _cache;

    /// <inheritdoc />
    public async Task<bool> IsExpandedAsync(string sectionKey, bool defaultExpanded = true, CancellationToken cancellationToken = default)
    {
        var state = await LoadAsync(cancellationToken);
        return state.TryGetValue(sectionKey, out var isExpanded) ? isExpanded : defaultExpanded;
    }

    /// <inheritdoc />
    public async Task SetExpandedAsync(string sectionKey, bool isExpanded, CancellationToken cancellationToken = default)
    {
        var state = await LoadAsync(cancellationToken);
        state[sectionKey] = isExpanded;
        await SaveAsync(state, cancellationToken);
    }

    private async Task<Dictionary<string, bool>> LoadAsync(CancellationToken cancellationToken)
    {
        if (_cache is not null)
        {
            return _cache;
        }

        try
        {
            var json = await jsRuntime.InvokeAsync<string?>("localStorage.getItem", cancellationToken, StorageKey);
            _cache = json is null
                ? new Dictionary<string, bool>()
                : JsonSerializer.Deserialize<Dictionary<string, bool>>(json, SerializerOptions) ?? new Dictionary<string, bool>();
        }
        catch (JSException exception)
        {
            storageStatus.ReportFailure(StorageFailureMessage, exception);
            _cache = new Dictionary<string, bool>();
        }

        return _cache;
    }

    private async Task SaveAsync(Dictionary<string, bool> state, CancellationToken cancellationToken)
    {
        try
        {
            var json = JsonSerializer.Serialize(state, SerializerOptions);
            await jsRuntime.InvokeVoidAsync("localStorage.setItem", cancellationToken, StorageKey, json);
        }
        catch (JSException exception)
        {
            storageStatus.ReportFailure(StorageFailureMessage, exception);
        }
    }
}
