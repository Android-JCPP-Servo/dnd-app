namespace DND.Models;

/// <summary>
/// A single attack row as recorded on the character sheet.
/// </summary>
public sealed class AttackEntry
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public string Name { get; set; } = string.Empty;

    public string AttackBonus { get; set; } = string.Empty;

    public string DamageAndType { get; set; } = string.Empty;
}
