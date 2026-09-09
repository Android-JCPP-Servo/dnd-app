namespace DND.Models;

/// <summary>
/// A lightweight projection of a character sheet used to render the character list.
/// </summary>
public sealed class CharacterSummary
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public string DisplayName { get; set; } = string.Empty;

    public string? PortraitReference { get; set; }
}
