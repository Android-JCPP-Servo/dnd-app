using DND.Models;

namespace DND.Services;

/// <summary>
/// Exports a character sheet and its portrait to a single downloadable JSON file, and imports
/// such a file back in as a character. Serves both as the backup against browser storage being
/// cleared and as the manual bridge for moving a character between devices.
/// </summary>
public interface ICharacterTransferService
{
    /// <summary>
    /// Reads the character sheet identified by <paramref name="characterId"/> and its portrait
    /// (if any), serializes them into a single export document, and triggers a browser download
    /// of the resulting JSON file. No-op if the character does not exist.
    /// </summary>
    Task ExportAsync(Guid characterId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Validates and deserializes a candidate export file. Returns null - never throws - when the
    /// stream is malformed JSON or does not contain a recognized character export document.
    /// Performs no writes, so a rejected file never touches stored data.
    /// </summary>
    Task<CharacterExportDocument?> ParseAsync(Stream json, CancellationToken cancellationToken = default);

    /// <summary>
    /// Persists a previously parsed export document as a character sheet, honoring
    /// <paramref name="resolution"/> when a character with the same display name already exists.
    /// Returns the saved character, or null if the save failed.
    /// </summary>
    Task<CharacterSheet?> ImportAsync(CharacterExportDocument document, ImportConflictResolution resolution, CancellationToken cancellationToken = default);
}

/// <summary>
/// How to resolve an import whose character name collides with an existing saved character.
/// </summary>
public enum ImportConflictResolution
{
    /// <summary>
    /// Save the imported character alongside the existing one, under a distinct display name.
    /// </summary>
    KeepBoth,

    /// <summary>
    /// Overwrite the existing character that shares the imported character's display name.
    /// </summary>
    Replace,
}
