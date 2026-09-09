namespace DND.Models;

/// <summary>
/// The raw bytes and content type of a character's portrait image, as stored in the
/// browser's IndexedDB portraits object store. Never attached to <see cref="CharacterSheet"/>.
/// </summary>
public sealed class PortraitRecord
{
    public string ContentType { get; set; } = string.Empty;

    public byte[] Bytes { get; set; } = Array.Empty<byte>();
}
