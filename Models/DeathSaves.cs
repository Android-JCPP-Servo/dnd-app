namespace DND.Models;

/// <summary>
/// Death save tracking as recorded on the character sheet.
/// </summary>
public sealed class DeathSaves
{
    public int Successes { get; set; }

    public int Failures { get; set; }
}
