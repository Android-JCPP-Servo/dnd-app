namespace DND.Models;

/// <summary>
/// A single ability score as entered on the character sheet.
/// </summary>
/// <remarks>
/// NOTE: Modifier is entered by the user, not calculated from Score. The client's
/// sheet treats every numeric box as free entry so a player can house-rule any value.
/// </remarks>
public sealed class AbilityScore
{
    public int Score { get; set; }

    public string Modifier { get; set; } = string.Empty;
}
