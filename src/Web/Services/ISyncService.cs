namespace Web.Services;

/// <summary>
/// Service interface for backend data synchronization.
/// Replaces Vue sync-store.ts from original implementation.
/// This is an optional feature for syncing factory data to a backend server.
/// </summary>
public interface ISyncService
{
    /// <summary>
    /// Event fired when the state changes (e.g., sync completed).
    /// </summary>
    event Action? OnChange;

    /// <summary>
    /// Gets whether data save is pending.
    /// </summary>
    bool DataSavePending { get; }

    /// <summary>
    /// Gets the last saved timestamp.
    /// </summary>
    DateTime? DataLastSaved { get; }

    /// <summary>
    /// Gets whether syncing is stopped.
    /// </summary>
    bool StopSyncing { get; }

    /// <summary>
    /// Gets whether the service is currently syncing.
    /// </summary>
    bool IsSyncing { get; }

    /// <summary>
    /// Sets up the automatic sync tick (runs every 10 seconds).
    /// </summary>
    void SetupTick();

    /// <summary>
    /// Stops the automatic sync tick.
    /// </summary>
    void StopSync();

    /// <summary>
    /// Marks that a change has been detected and data should be synced.
    /// </summary>
    void DetectedChange();

    /// <summary>
    /// Loads data from the server.
    /// </summary>
    /// <param name="forceLoad">Whether to force load the data without OOS checking.</param>
    /// <returns>"oos" if out of sync, true if loaded successfully, null otherwise.</returns>
    Task<string?> HandleDataLoadAsync(bool forceLoad = false);

    /// <summary>
    /// Syncs data to the server.
    /// </summary>
    /// <param name="force">Whether to force sync regardless of pending state.</param>
    /// <returns>True if sync succeeded.</returns>
    Task<bool> HandleSyncAsync(bool force = false);

    /// <summary>
    /// Gets the formatted last saved display string.
    /// </summary>
    string GetLastSavedDisplay();
}
