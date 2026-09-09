namespace DND.Models;

/// <summary>
/// The six ability scores tracked on the character sheet.
/// </summary>
public sealed class AbilitySet
{
    public AbilityScore Strength { get; set; } = new();

    public AbilityScore Dexterity { get; set; } = new();

    public AbilityScore Constitution { get; set; } = new();

    public AbilityScore Intelligence { get; set; } = new();

    public AbilityScore Wisdom { get; set; } = new();

    public AbilityScore Charisma { get; set; } = new();
}
