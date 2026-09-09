using DND.Models;

namespace DND.Services;

/// <summary>
/// Owns a single debounce timer for the whole character sheet: every <see cref="NotifyChanged"/>
/// call cancels the in-flight delay and starts a fresh one, so only the last change in a burst
/// of edits is written to storage.
/// </summary>
public sealed class CharacterAutosaveService(ICharacterStore characterStore, IStorageStatusService storageStatus)
    : ICharacterAutosaveService, IAsyncDisposable
{
    private const int DebounceMilliseconds = 500;

    private CancellationTokenSource? _pendingCancellation;
    private CharacterSheet? _pendingCharacter;

    /// <inheritdoc />
    public void NotifyChanged(CharacterSheet character)
    {
        _pendingCancellation?.Cancel();

        var cancellation = new CancellationTokenSource();
        _pendingCancellation = cancellation;
        _pendingCharacter = character;

        _ = DebounceAndSaveAsync(character, cancellation.Token);
    }

    /// <inheritdoc />
    public async Task FlushAsync(CancellationToken cancellationToken = default)
    {
        _pendingCancellation?.Cancel();

        var character = _pendingCharacter;
        if (character is not null)
        {
            await characterStore.SaveAsync(character, cancellationToken);
        }
    }

    private async Task DebounceAndSaveAsync(CharacterSheet character, CancellationToken token)
    {
        try
        {
            await Task.Delay(DebounceMilliseconds, token);
            await characterStore.SaveAsync(character, token);
        }
        catch (OperationCanceledException)
        {
            // Superseded by a newer edit — expected, not a failure. Do not report it.
        }
        catch (Exception exception)
        {
            storageStatus.ReportFailure("Your changes could not be saved to this browser.", exception);
        }
    }

    /// <inheritdoc />
    public ValueTask DisposeAsync()
    {
        _pendingCancellation?.Cancel();
        _pendingCancellation?.Dispose();
        return ValueTask.CompletedTask;
    }
}
