using DND.Models;

namespace DND.Services;

/// <summary>
/// Debounces character edits into a single durable write, so components can report every
/// field edit, toggle, stepper click, and row add/remove without thrashing storage.
/// </summary>
public interface ICharacterAutosaveService
{
    /// <summary>
    /// Queues a debounced save of the given character.
    /// </summary>
    void NotifyChanged(CharacterSheet character);

    /// <summary>
    /// Writes any pending change immediately.
    /// </summary>
    Task FlushAsync(CancellationToken cancellationToken = default);
}
