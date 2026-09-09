namespace DND.Models;

/// <summary>
/// The total and expended spell slots for a single spell level.
/// </summary>
public sealed class SpellSlotLevel
{
    public int Level { get; set; }

    public int Total { get; set; }

    public int Expended { get; set; }
}
