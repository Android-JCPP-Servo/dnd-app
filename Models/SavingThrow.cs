namespace DND.Models;

/// <summary>
/// A single saving throw entry as recorded on the character sheet.
/// </summary>
public sealed class SavingThrow
{
    public string Bonus { get; set; } = string.Empty;

    public bool IsProficient { get; set; }
}
