namespace DND.Models;

/// <summary>
/// A single ally or organization row as recorded on the character sheet.
/// </summary>
public sealed class AllyEntry
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public string Name { get; set; } = string.Empty;

    public string Details { get; set; } = string.Empty;
}
