namespace DND.Services;

/// <summary>
/// Singleton, app-lifetime holder of the current browser-storage failure state.
/// </summary>
public sealed class StorageStatusService : IStorageStatusService
{
    /// <inheritdoc />
    public string? FailureMessage { get; private set; }

    /// <inheritdoc />
    public event Action? StatusChanged;

    /// <inheritdoc />
    public void ReportFailure(string message, Exception exception)
    {
        Console.Error.WriteLine(exception);
        FailureMessage = message;
        StatusChanged?.Invoke();
    }

    /// <inheritdoc />
    public void ClearFailure()
    {
        FailureMessage = null;
        StatusChanged?.Invoke();
    }
}
