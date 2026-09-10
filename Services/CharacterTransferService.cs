using System.Text.Json;
using DND.Models;
using Microsoft.JSInterop;

namespace DND.Services;

/// <summary>
/// <see cref="ICharacterTransferService"/> implementation that reads/writes character sheets and
/// portraits through <see cref="ICharacterStore"/> and triggers browser downloads through the
/// interop module at wwwroot/js/fileDownload.js.
/// </summary>
public sealed class CharacterTransferService(IJSRuntime jsRuntime, ICharacterStore characterStore, IStorageStatusService storageStatus)
    : ICharacterTransferService, IAsyncDisposable
{
    private const string ExportFailureMessage = "The character could not be exported.";
    private const string ImportFailureMessage = "The character could not be imported.";
    private const int MaxFileNameLength = 60;

    // PascalCase to match how sheets are already stored (see IndexedDbCharacterStore.SerializerOptions).
    // PropertyNameCaseInsensitive is set for both serialize and deserialize so a hand-edited export
    // file still round-trips even if its casing was changed.
    private static readonly JsonSerializerOptions SerializerOptions = new()
    {
        WriteIndented = true,
        PropertyNameCaseInsensitive = true,
    };

    private readonly Lazy<Task<IJSObjectReference>> _module = new(() =>
        jsRuntime.InvokeAsync<IJSObjectReference>("import", "./js/fileDownload.js").AsTask());

    /// <inheritdoc />
    public async Task ExportAsync(Guid characterId, CancellationToken cancellationToken = default)
    {
        try
        {
            var character = await characterStore.GetAsync(characterId, cancellationToken);
            if (character is null)
            {
                return;
            }

            var document = new CharacterExportDocument
            {
                Character = character,
            };

            if (!string.IsNullOrEmpty(character.PortraitReference))
            {
                var portrait = await characterStore.GetPortraitAsync(character.PortraitReference, cancellationToken);
                if (portrait is not null)
                {
                    document.PortraitContentType = portrait.ContentType;
                    document.PortraitBase64 = Convert.ToBase64String(portrait.Bytes);
                }
            }

            var json = JsonSerializer.Serialize(document, SerializerOptions);
            var fileName = BuildFileName(character.DisplayName);

            var module = await _module.Value;
            await module.InvokeVoidAsync("downloadJson", cancellationToken, fileName, json);
        }
        catch (JSException exception)
        {
            storageStatus.ReportFailure(ExportFailureMessage, exception);
        }
    }

    /// <inheritdoc />
    public async Task<CharacterExportDocument?> ParseAsync(Stream json, CancellationToken cancellationToken = default)
    {
        try
        {
            var document = await JsonSerializer.DeserializeAsync<CharacterExportDocument>(json, SerializerOptions, cancellationToken);
            if (document is null
                || !string.Equals(document.Format, CharacterExportDocument.CurrentFormat, StringComparison.Ordinal)
                || document.SchemaVersion != 1
                || document.Character is null)
            {
                return null;
            }

            return document;
        }
        catch (JsonException)
        {
            return null;
        }
        catch (NotSupportedException)
        {
            return null;
        }
    }

    /// <inheritdoc />
    public async Task<CharacterSheet?> ImportAsync(CharacterExportDocument document, ImportConflictResolution resolution, CancellationToken cancellationToken = default)
    {
        if (document.Character is null)
        {
            return null;
        }

        try
        {
            // Round-trip through JSON so the imported sheet shares no nested object or list with
            // the document that was parsed, mirroring CharacterDuplicator's deep-copy technique.
            var json = JsonSerializer.Serialize(document.Character, SerializerOptions);
            var character = JsonSerializer.Deserialize<CharacterSheet>(json, SerializerOptions);
            if (character is null)
            {
                return null;
            }

            string? oldPortraitReferenceToDelete = null;

            if (resolution == ImportConflictResolution.Replace)
            {
                var existing = (await characterStore.ListAllAsync(cancellationToken))
                    .FirstOrDefault(summary => string.Equals(summary.DisplayName, character.DisplayName, StringComparison.Ordinal));

                if (existing is not null)
                {
                    character.Id = existing.Id;
                    oldPortraitReferenceToDelete = existing.PortraitReference;
                }
                else
                {
                    character.Id = Guid.NewGuid();
                }
            }
            else
            {
                character.Id = Guid.NewGuid();
                character.DisplayName = await BuildDistinctImportDisplayNameAsync(character.DisplayName, cancellationToken);
            }

            // Never trust the portrait reference embedded in the file - it may collide with an
            // unrelated local blob. Mint a brand-new one and write it before deleting the old blob
            // (write new, then delete old) so a mid-operation failure leaves a readable portrait.
            character.PortraitReference = null;

            if (!string.IsNullOrEmpty(document.PortraitBase64))
            {
                try
                {
                    var bytes = Convert.FromBase64String(document.PortraitBase64);
                    var newPortraitReference = Guid.NewGuid().ToString();
                    await characterStore.SavePortraitAsync(
                        newPortraitReference,
                        new PortraitRecord { ContentType = document.PortraitContentType ?? string.Empty, Bytes = bytes },
                        cancellationToken);
                    character.PortraitReference = newPortraitReference;
                }
                catch (FormatException)
                {
                    character.PortraitReference = null;
                }
            }

            await characterStore.SaveAsync(character, cancellationToken);

            if (!string.IsNullOrEmpty(oldPortraitReferenceToDelete))
            {
                await characterStore.DeletePortraitAsync(oldPortraitReferenceToDelete, cancellationToken);
            }

            return character;
        }
        catch (JSException exception)
        {
            storageStatus.ReportFailure(ImportFailureMessage, exception);
            return null;
        }
    }

    /// <summary>
    /// Builds an import name distinguishable from every saved character: "(Imported)", then
    /// "(Imported 2)", "(Imported 3)", etc. until an unused name is found. Mirrors
    /// CharacterDuplicator.BuildDistinctDisplayNameAsync, which is private to that class and
    /// cannot be reused directly.
    /// </summary>
    private async Task<string> BuildDistinctImportDisplayNameAsync(string sourceDisplayName, CancellationToken cancellationToken)
    {
        var baseName = string.IsNullOrEmpty(sourceDisplayName) ? "Unnamed character" : sourceDisplayName;
        var existingNames = (await characterStore.ListAllAsync(cancellationToken))
            .Select(summary => summary.DisplayName)
            .ToHashSet(StringComparer.Ordinal);

        var candidate = $"{baseName} (Imported)";
        var suffix = 2;
        while (existingNames.Contains(candidate))
        {
            candidate = $"{baseName} (Imported {suffix})";
            suffix++;
        }

        return candidate;
    }

    /// <summary>
    /// Derives a recognizable, filesystem-safe file name from a character's display name: falls
    /// back to the same "Unnamed character" text used elsewhere in the panel, strips characters
    /// invalid in a file name, collapses whitespace to hyphens, and truncates to a sane length.
    /// </summary>
    private static string BuildFileName(string displayName)
    {
        var baseName = string.IsNullOrEmpty(displayName) ? "Unnamed character" : displayName;

        var allowed = baseName
            .Select(c => char.IsLetterOrDigit(c) || c is '-' or '_' or ' ' ? c : ' ')
            .ToArray();

        var sanitized = string.Join('-', new string(allowed).Split(' ', StringSplitOptions.RemoveEmptyEntries));

        if (sanitized.Length == 0)
        {
            sanitized = "character";
        }

        if (sanitized.Length > MaxFileNameLength)
        {
            sanitized = sanitized[..MaxFileNameLength];
        }

        return $"{sanitized}.json";
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
