namespace DND.Models;

/// <summary>
/// The eighteen skills tracked on the character sheet.
/// </summary>
public sealed class SkillSet
{
    public SkillProficiency Acrobatics { get; set; } = new();

    public SkillProficiency AnimalHandling { get; set; } = new();

    public SkillProficiency Arcana { get; set; } = new();

    public SkillProficiency Athletics { get; set; } = new();

    public SkillProficiency Deception { get; set; } = new();

    public SkillProficiency History { get; set; } = new();

    public SkillProficiency Insight { get; set; } = new();

    public SkillProficiency Intimidation { get; set; } = new();

    public SkillProficiency Investigation { get; set; } = new();

    public SkillProficiency Medicine { get; set; } = new();

    public SkillProficiency Nature { get; set; } = new();

    public SkillProficiency Perception { get; set; } = new();

    public SkillProficiency Performance { get; set; } = new();

    public SkillProficiency Persuasion { get; set; } = new();

    public SkillProficiency Religion { get; set; } = new();

    public SkillProficiency SleightOfHand { get; set; } = new();

    public SkillProficiency Stealth { get; set; } = new();

    public SkillProficiency Survival { get; set; } = new();
}
