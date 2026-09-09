namespace DND.Models;

/// <summary>
/// Physical description details as recorded on the character sheet.
/// </summary>
public sealed class PhysicalTraits
{
    public string Age { get; set; } = string.Empty;

    public string Height { get; set; } = string.Empty;

    public string Weight { get; set; } = string.Empty;

    public string Eyes { get; set; } = string.Empty;

    public string Skin { get; set; } = string.Empty;

    public string Hair { get; set; } = string.Empty;
}
