using Web.Models.Factory;

namespace Web.Services;

/// <summary>
/// Interface for application state management service.
/// Replaces Pinia app-store.ts from Vue implementation.
/// </summary>
public interface IAppStateService
{
    /// <summary>
    /// Event raised when factories are modified.
    /// </summary>
    event Action? OnChange;

    /// <summary>
    /// Gets the list of all factories.
    /// </summary>
    List<Factory> GetFactories();

    /// <summary>
    /// Sets the entire factory list.
    /// </summary>
    /// <param name="factories">The new factory list.</param>
    void SetFactories(List<Factory> factories);

    /// <summary>
    /// Adds a new factory to the list.
    /// </summary>
    /// <param name="factory">The factory to add.</param>
    void AddFactory(Factory factory);

    /// <summary>
    /// Clears all factories.
    /// </summary>
    void ClearFactories();

    /// <summary>
    /// Loads factories from local storage.
    /// </summary>
    /// <returns>True if factories were loaded successfully.</returns>
    Task<bool> LoadFactoriesAsync();

    /// <summary>
    /// Saves factories to local storage.
    /// </summary>
    Task SaveFactoriesAsync();

    /// <summary>
    /// Gets whether help text should be shown.
    /// </summary>
    bool GetHelpTextShown();

    /// <summary>
    /// Sets whether help text should be shown.
    /// </summary>
    /// <param name="shown">True to show help text.</param>
    void SetHelpTextShown(bool shown);

    /// <summary>
    /// Gets all factory tabs.
    /// </summary>
    List<FactoryTab> GetFactoryTabs();

    /// <summary>
    /// Adds a new factory tab.
    /// </summary>
    /// <param name="tab">The factory tab to add.</param>
    void AddFactoryTab(FactoryTab tab);

    /// <summary>
    /// Gets the current factory tab index.
    /// </summary>
    int GetCurrentFactoryTabIndex();

    /// <summary>
    /// Sets the current factory tab index.
    /// </summary>
    /// <param name="index">The index to set.</param>
    void SetCurrentFactoryTabIndex(int index);
}
