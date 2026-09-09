namespace DND.Models;

/// <summary>
/// A complete D&amp;D 5e character sheet. Every value is user-entered; nothing is calculated.
/// </summary>
public sealed class CharacterSheet
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public string DisplayName { get; set; } = string.Empty;

    public string CharacterName { get; set; } = string.Empty;

    public string ClassAndLevel { get; set; } = string.Empty;

    public string Background { get; set; } = string.Empty;

    public string PlayerName { get; set; } = string.Empty;

    public string Race { get; set; } = string.Empty;

    public string Alignment { get; set; } = string.Empty;

    public string ExperiencePoints { get; set; } = string.Empty;

    public bool Inspiration { get; set; }

    public string ProficiencyBonus { get; set; } = string.Empty;

    public int PassiveWisdom { get; set; }

    public int ArmorClass { get; set; }

    public string Initiative { get; set; } = string.Empty;

    public string Speed { get; set; } = string.Empty;

    public AbilitySet Abilities { get; set; } = new();

    public SavingThrowSet SavingThrows { get; set; } = new();

    public SkillSet Skills { get; set; } = new();

    public HitPoints HitPoints { get; set; } = new();

    public DeathSaves DeathSaves { get; set; } = new();

    public CoinPurse Coins { get; set; } = new();

    public Spellcasting Spellcasting { get; set; } = new();

    public PersonalityProfile Personality { get; set; } = new();

    public PhysicalTraits PhysicalTraits { get; set; } = new();

    public List<AttackEntry> Attacks { get; set; } = new();

    public List<TextEntry> Equipment { get; set; } = new();

    public List<TextEntry> OtherProficienciesAndLanguages { get; set; } = new();

    public List<AllyEntry> AlliesAndOrganizations { get; set; } = new();

    public string FeaturesAndTraits { get; set; } = string.Empty;

    public string CharacterAppearance { get; set; } = string.Empty;

    public string AdditionalFeaturesAndTraits { get; set; } = string.Empty;

    public string Backstory { get; set; } = string.Empty;

    public string Treasure { get; set; } = string.Empty;

    /// <summary>
    /// Key of this character's portrait in browser storage, or null when no portrait is set.
    /// </summary>
    /// <remarks>
    /// NOTE: This is a reference only. Portrait bytes are never stored on the model — they live
    /// in the portraits object store (see ticket 189). Do not add an image-data property here.
    /// </remarks>
    public string? PortraitReference { get; set; }
}
