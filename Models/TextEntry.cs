namespace DND.Models;

/// <summary>
/// A single free-text row used for equipment, other proficiencies &amp; languages, and
/// allies &amp; organizations sections of the character sheet.
/// </summary>
public sealed class TextEntry
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public string Text { get; set; } = string.Empty;
}
