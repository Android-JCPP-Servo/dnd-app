namespace DND.Models;

/// <summary>
/// A single free-text row used for equipment and other proficiencies &amp; languages
/// sections of the character sheet.
/// </summary>
public sealed class TextEntry
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public string Text { get; set; } = string.Empty;
}
