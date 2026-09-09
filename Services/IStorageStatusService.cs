namespace DND.Services;

/// <summary>
/// Holds the current browser-storage failure state and notifies subscribers when it changes,
/// so a failure can surface as a visible, non-blocking warning rather than being lost silently.
/// </summary>
public interface IStorageStatusService
{
    /// <summary>
    /// The current user-facing failure message, or null when storage is healthy.
    /// </summary>
    string? FailureMessage { get; }

    /// <summary>
    /// Raised whenever <see cref="FailureMessage"/> changes.
    /// </summary>
    event Action? StatusChanged;

    /// <summary>
    /// Records a storage failure. <paramref name="message"/> is user-facing; <paramref name="exception"/>
    /// is captured for diagnostics only and must never appear in <see cref="FailureMessage"/>.
    /// </summary>
    void ReportFailure(string message, Exception exception);

    /// <summary>
    /// Clears the current failure state.
    /// </summary>
    void ClearFailure();
}
