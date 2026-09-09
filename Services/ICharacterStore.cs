using DND.Models;

namespace DND.Services;

/// <summary>
/// Provides create, read, update, delete, and list-all persistence for character sheets
/// and their portraits, entirely within the browser (no server, account, or network call).
/// </summary>
public interface ICharacterStore
{
    /// <summary>
    /// Reads a single character sheet by id, or null if it does not exist or storage fails.
    /// </summary>
    Task<CharacterSheet?> GetAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Lists every saved character as a lightweight summary, without parsing full sheet JSON.
    /// </summary>
    Task<IReadOnlyList<CharacterSummary>> ListAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates or updates a character sheet. IndexedDB's put is an upsert, so this single
    /// method covers both create and update.
    /// </summary>
    Task SaveAsync(CharacterSheet character, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a character sheet and its associated portrait, if any.
    /// </summary>
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates or updates the portrait bytes referenced by <paramref name="portraitReference"/>.
    /// </summary>
    Task SavePortraitAsync(string portraitReference, PortraitRecord portrait, CancellationToken cancellationToken = default);

    /// <summary>
    /// Reads a portrait's bytes by its reference, or null if it does not exist or storage fails.
    /// </summary>
    Task<PortraitRecord?> GetPortraitAsync(string portraitReference, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a portrait by its reference.
    /// </summary>
    Task DeletePortraitAsync(string portraitReference, CancellationToken cancellationToken = default);
}
