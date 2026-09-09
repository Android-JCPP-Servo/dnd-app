using System.Text.Json;
using DND.Models;
using Microsoft.JSInterop;

namespace DND.Services;

/// <summary>
/// <see cref="ICharacterStore"/> implementation backed by the hand-written IndexedDB
/// interop module at wwwroot/js/characterStorage.js. Every operation catches
/// <see cref="JSException"/>, reports it to <see cref="IStorageStatusService"/>, and returns
/// a safe value instead of rethrowing, so a storage failure never trips Blazor's fatal-error UI.
/// </summary>
public sealed class IndexedDbCharacterStore(IJSRuntime jsRuntime, IStorageStatusService storageStatus)
    : ICharacterStore, IAsyncDisposable
{
    private const string StorageFailureMessage = "Your changes could not be saved to this browser.";

    private static readonly JsonSerializerOptions SerializerOptions = new();

    private readonly Lazy<Task<IJSObjectReference>> _module = new(() =>
        jsRuntime.InvokeAsync<IJSObjectReference>("import", "./js/characterStorage.js").AsTask());

    /// <inheritdoc />
    public async Task<CharacterSheet?> GetAsync(Guid id, CancellationToken cancellationToken = default)
    {
        try
        {
            var module = await _module.Value;
            var json = await module.InvokeAsync<string?>("getCharacter", cancellationToken, id.ToString());
            return json is null ? null : JsonSerializer.Deserialize<CharacterSheet>(json, SerializerOptions);
        }
        catch (JSException exception)
        {
            storageStatus.ReportFailure(StorageFailureMessage, exception);
            return null;
        }
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<CharacterSummary>> ListAllAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var module = await _module.Value;
            var records = await module.InvokeAsync<List<CharacterListRecord>>("listCharacters", cancellationToken);
            return records
                .Select(record => new CharacterSummary
                {
                    Id = Guid.Parse(record.Id),
                    DisplayName = record.DisplayName,
                    PortraitReference = record.PortraitReference,
                })
                .ToList();
        }
        catch (JSException exception)
        {
            storageStatus.ReportFailure(StorageFailureMessage, exception);
            return Array.Empty<CharacterSummary>();
        }
    }

    /// <inheritdoc />
    public async Task SaveAsync(CharacterSheet character, CancellationToken cancellationToken = default)
    {
        try
        {
            var module = await _module.Value;
            var json = JsonSerializer.Serialize(character, SerializerOptions);
            await module.InvokeVoidAsync(
                "saveCharacter",
                cancellationToken,
                character.Id.ToString(),
                character.DisplayName,
                character.PortraitReference,
                json);
        }
        catch (JSException exception)
        {
            storageStatus.ReportFailure(StorageFailureMessage, exception);
        }
    }

    /// <inheritdoc />
    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        try
        {
            var module = await _module.Value;
            await module.InvokeVoidAsync("deleteCharacter", cancellationToken, id.ToString());
        }
        catch (JSException exception)
        {
            storageStatus.ReportFailure(StorageFailureMessage, exception);
        }
    }

    /// <inheritdoc />
    public async Task SavePortraitAsync(string portraitReference, PortraitRecord portrait, CancellationToken cancellationToken = default)
    {
        try
        {
            var module = await _module.Value;
            var base64 = Convert.ToBase64String(portrait.Bytes);
            await module.InvokeVoidAsync("savePortrait", cancellationToken, portraitReference, portrait.ContentType, base64);
        }
        catch (JSException exception)
        {
            storageStatus.ReportFailure(StorageFailureMessage, exception);
        }
    }

    /// <inheritdoc />
    public async Task<PortraitRecord?> GetPortraitAsync(string portraitReference, CancellationToken cancellationToken = default)
    {
        try
        {
            var module = await _module.Value;
            var record = await module.InvokeAsync<PortraitInteropRecord?>("getPortrait", cancellationToken, portraitReference);
            return record is null
                ? null
                : new PortraitRecord
                {
                    ContentType = record.ContentType,
                    Bytes = Convert.FromBase64String(record.Base64),
                };
        }
        catch (JSException exception)
        {
            storageStatus.ReportFailure(StorageFailureMessage, exception);
            return null;
        }
    }

    /// <inheritdoc />
    public async Task DeletePortraitAsync(string portraitReference, CancellationToken cancellationToken = default)
    {
        try
        {
            var module = await _module.Value;
            await module.InvokeVoidAsync("deletePortrait", cancellationToken, portraitReference);
        }
        catch (JSException exception)
        {
            storageStatus.ReportFailure(StorageFailureMessage, exception);
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

    /// <summary>
    /// Wire shape of a single record returned by the JS module's listCharacters export.
    /// </summary>
    private sealed class CharacterListRecord
    {
        public string Id { get; set; } = string.Empty;

        public string DisplayName { get; set; } = string.Empty;

        public string? PortraitReference { get; set; }
    }

    /// <summary>
    /// Wire shape of the record returned by the JS module's getPortrait export.
    /// </summary>
    private sealed class PortraitInteropRecord
    {
        public string ContentType { get; set; } = string.Empty;

        public string Base64 { get; set; } = string.Empty;
    }
}
