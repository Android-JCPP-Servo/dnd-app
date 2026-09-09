namespace DND.Models;

/// <summary>
/// Personality details as recorded on the character sheet.
/// </summary>
public sealed class PersonalityProfile
{
    public string PersonalityTraits { get; set; } = string.Empty;

    public string Ideals { get; set; } = string.Empty;

    public string Bonds { get; set; } = string.Empty;

    public string Flaws { get; set; } = string.Empty;
}
