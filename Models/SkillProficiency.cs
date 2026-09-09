namespace DND.Models;

/// <summary>
/// A single skill entry as recorded on the character sheet.
/// </summary>
/// <remarks>
/// NOTE: Bonus is entered by the user, not calculated from an ability modifier and
/// proficiency bonus. Kept as a separate type from SavingThrow because skills and
/// saving throws are distinct sheet sections that may diverge later.
/// </remarks>
public sealed class SkillProficiency
{
    public string Bonus { get; set; } = string.Empty;

    public bool IsProficient { get; set; }
}
