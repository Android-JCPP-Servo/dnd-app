namespace DND.Models;

/// <summary>
/// The single-file envelope produced by character export and consumed by character import.
/// Carries the complete character sheet alongside its portrait (if any) in one JSON document,
/// since <see cref="CharacterSheet"/> deliberately stores only a portrait reference and never
/// the portrait bytes themselves (see <see cref="CharacterSheet.PortraitReference"/>).
/// </summary>
public sealed class CharacterExportDocument
{
    /// <summary>
    /// The format marker every export produced by this application carries. Used by import to
    /// reject files that are valid JSON but not a recognized character export.
    /// </summary>
    public const string CurrentFormat = "dnd-character-sheet";

    public string Format { get; set; } = CurrentFormat;

    public int SchemaVersion { get; set; } = 1;

    public CharacterSheet? Character { get; set; }

    /// <summary>
    /// The portrait's content type (e.g. "image/jpeg"), or null when the character has no portrait.
    /// </summary>
    public string? PortraitContentType { get; set; }

    /// <summary>
    /// The portrait's bytes, base64-encoded so the export file remains plain, readable JSON.
    /// Null when the character has no portrait.
    /// </summary>
    public string? PortraitBase64 { get; set; }
}
