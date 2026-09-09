using DND.Models;

namespace DND.Services;

/// <summary>
/// Produces a fully independent copy of a saved character sheet, including its rows, spell
/// slots, and portrait, so a variant can be tried without altering the original.
/// </summary>
public interface ICharacterDuplicator
{
    /// <summary>
    /// Deep-copies the character sheet identified by <paramref name="sourceId"/>, persists the
    /// copy under a new id, and returns it. Returns null if the source does not exist or the
    /// copy could not be saved.
    /// </summary>
    Task<CharacterSheet?> DuplicateAsync(Guid sourceId, CancellationToken cancellationToken = default);
}
