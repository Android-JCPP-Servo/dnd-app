using System.Text.Json;
using DND.Models;
using Microsoft.JSInterop;

namespace DND.Services;

/// <summary>
/// <see cref="ICharacterDuplicator"/> implementation that deep-copies a character sheet via a
/// System.Text.Json round-trip, so the copy shares no nested object or list with the source and
/// editing one never affects the other.
/// </summary>
public sealed class CharacterDuplicator(ICharacterStore characterStore, IStorageStatusService storageStatus)
    : ICharacterDuplicator
{
    private const string DuplicateFailureMessage = "The character could not be duplicated.";

    /// <inheritdoc />
    public async Task<CharacterSheet?> DuplicateAsync(Guid sourceId, CancellationToken cancellationToken = default)
    {
        try
        {
            var source = await characterStore.GetAsync(sourceId, cancellationToken);
            if (source is null)
            {
                return null;
            }

            var json = JsonSerializer.Serialize(source);
            var copy = JsonSerializer.Deserialize<CharacterSheet>(json);
            if (copy is null)
            {
                return null;
            }

            copy.Id = Guid.NewGuid();
            copy.DisplayName = await BuildDistinctDisplayNameAsync(source.DisplayName, cancellationToken);

            if (!string.IsNullOrEmpty(source.PortraitReference))
            {
                var portrait = await characterStore.GetPortraitAsync(source.PortraitReference, cancellationToken);
                if (portrait is not null)
                {
                    var newPortraitReference = Guid.NewGuid().ToString();
                    await characterStore.SavePortraitAsync(newPortraitReference, portrait, cancellationToken);
                    copy.PortraitReference = newPortraitReference;
                }
                else
                {
                    copy.PortraitReference = null;
                }
            }

            await characterStore.SaveAsync(copy, cancellationToken);
            return copy;
        }
        catch (JSException exception)
        {
            storageStatus.ReportFailure(DuplicateFailureMessage, exception);
            return null;
        }
    }

    /// <summary>
    /// Builds a copy name distinguishable from the source and from every other saved character:
    /// "(Copy)", then "(Copy 2)", "(Copy 3)", etc. until an unused name is found.
    /// </summary>
    private async Task<string> BuildDistinctDisplayNameAsync(string sourceDisplayName, CancellationToken cancellationToken)
    {
        var baseName = string.IsNullOrEmpty(sourceDisplayName) ? "Unnamed character" : sourceDisplayName;
        var existingNames = (await characterStore.ListAllAsync(cancellationToken))
            .Select(summary => summary.DisplayName)
            .ToHashSet(StringComparer.Ordinal);

        var candidate = $"{baseName} (Copy)";
        var suffix = 2;
        while (existingNames.Contains(candidate))
        {
            candidate = $"{baseName} (Copy {suffix})";
            suffix++;
        }

        return candidate;
    }
}
