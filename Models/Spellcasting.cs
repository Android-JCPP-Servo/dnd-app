namespace DND.Models;

/// <summary>
/// Spellcasting information as recorded on the character sheet.
/// </summary>
public sealed class Spellcasting
{
    public string SpellcastingAbility { get; set; } = string.Empty;

    public int SpellSaveDc { get; set; }

    public string SpellAttackBonus { get; set; } = string.Empty;

    public List<SpellEntry> Spells { get; set; } = new();

    public List<SpellSlotLevel> SlotLevels { get; set; } = CreateSlotLevels();

    private static List<SpellSlotLevel> CreateSlotLevels()
    {
        List<SpellSlotLevel> slotLevels = new();

        for (int level = 1; level <= 9; level++)
        {
            slotLevels.Add(new SpellSlotLevel { Level = level });
        }

        return slotLevels;
    }
}
