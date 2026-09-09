namespace DND.Models;

/// <summary>
/// The six saving throws tracked on the character sheet.
/// </summary>
public sealed class SavingThrowSet
{
    public SavingThrow Strength { get; set; } = new();

    public SavingThrow Dexterity { get; set; } = new();

    public SavingThrow Constitution { get; set; } = new();

    public SavingThrow Intelligence { get; set; } = new();

    public SavingThrow Wisdom { get; set; } = new();

    public SavingThrow Charisma { get; set; } = new();
}
