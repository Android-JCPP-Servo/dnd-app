namespace DND.Models;

/// <summary>
/// The coin purse as recorded on the character sheet.
/// </summary>
public sealed class CoinPurse
{
    public int Copper { get; set; }

    public int Silver { get; set; }

    public int Electrum { get; set; }

    public int Gold { get; set; }

    public int Platinum { get; set; }
}
