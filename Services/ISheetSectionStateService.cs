namespace DND.Services;

/// <summary>
/// Persists, per device, whether each collapsible sheet section is expanded or collapsed,
/// so the client's fold/unfold choices survive a page reload.
/// </summary>
public interface ISheetSectionStateService
{
    /// <summary>
    /// Returns whether the section identified by <paramref name="sectionKey"/> is currently
    /// expanded. Falls back to <paramref name="defaultExpanded"/> when no state has been saved.
    /// </summary>
    Task<bool> IsExpandedAsync(string sectionKey, bool defaultExpanded = true, CancellationToken cancellationToken = default);

    /// <summary>
    /// Records whether the section identified by <paramref name="sectionKey"/> is expanded.
    /// </summary>
    Task SetExpandedAsync(string sectionKey, bool isExpanded, CancellationToken cancellationToken = default);
}
