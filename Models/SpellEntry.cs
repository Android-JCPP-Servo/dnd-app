namespace DND.Models;

/// <summary>
/// A single spell entry as recorded on the character sheet.
/// </summary>
/// <remarks>
/// NOTE: Level is user-selected and stored, not inferred from the spell name. It
/// records which level heading the spell was entered under so the grouping survives
/// a round-trip. Use 0 for cantrips.
/// </remarks>
public sealed class SpellEntry
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public int Level { get; set; }

    public string Name { get; set; } = string.Empty;

    public string CastingTime { get; set; } = string.Empty;

    public string Range { get; set; } = string.Empty;

    public string Components { get; set; } = string.Empty;

    public string Duration { get; set; } = string.Empty;

    public string Effect { get; set; } = string.Empty;
}
