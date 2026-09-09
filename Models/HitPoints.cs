namespace DND.Models;

/// <summary>
/// Hit point and hit dice tracking as recorded on the character sheet.
/// </summary>
public sealed class HitPoints
{
    public int Maximum { get; set; }

    public int Current { get; set; }

    public int Temporary { get; set; }

    public string HitDiceType { get; set; } = string.Empty;

    public int HitDiceRemaining { get; set; }
}
