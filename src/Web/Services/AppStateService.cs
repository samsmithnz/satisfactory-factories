using System.Text.Json;
using Microsoft.JSInterop;
using Web.Models.Factory;

namespace Web.Services;

/// <summary>
/// Application state management service.
/// Replaces Pinia app-store.ts from Vue implementation.
/// </summary>
public class AppStateService : IAppStateService
{
    private readonly IJSRuntime _jsRuntime;
    private List<Factory> _factories = new List<Factory>();
    private List<FactoryTab> _factoryTabs = new List<FactoryTab>();
    private int _currentFactoryTabIndex = 0;
    private bool _helpTextShown = false;
    private const string LocalStorageKey = "factories";
    private const string FactoryTabsKey = "factoryTabs";
    private const string CurrentTabIndexKey = "currentFactoryTabIndex";
    private const string HelpTextKey = "helpText";

    /// <inheritdoc/>
    public event Action? OnChange;

    public AppStateService(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
    }

    /// <inheritdoc/>
    public List<Factory> GetFactories()
    {
        return _factories;
    }

    /// <inheritdoc/>
    public void SetFactories(List<Factory> factories)
    {
        _factories = factories;
        NotifyStateChanged();
    }

    /// <inheritdoc/>
    public void AddFactory(Factory factory)
    {
        _factories.Add(factory);
        NotifyStateChanged();
    }

    /// <inheritdoc/>
    public void ClearFactories()
    {
        _factories.Clear();
        NotifyStateChanged();
    }

    /// <inheritdoc/>
    public async Task<bool> LoadFactoriesAsync()
    {
        try
        {
            string? json = await _jsRuntime.InvokeAsync<string?>("localStorage.getItem", LocalStorageKey);
            if (!string.IsNullOrEmpty(json))
            {
                List<Factory>? loaded = JsonSerializer.Deserialize<List<Factory>>(json);
                if (loaded != null)
                {
                    _factories = loaded;
                    NotifyStateChanged();
                    return true;
                }
            }
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Error loading factories from local storage: {ex.Message}");
        }

        return false;
    }

    /// <inheritdoc/>
    public async Task SaveFactoriesAsync()
    {
        try
        {
            string json = JsonSerializer.Serialize(_factories);
            await _jsRuntime.InvokeVoidAsync("localStorage.setItem", LocalStorageKey, json);
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Error saving factories to local storage: {ex.Message}");
        }
    }

    /// <inheritdoc/>
    public bool GetHelpTextShown()
    {
        return _helpTextShown;
    }

    /// <inheritdoc/>
    public void SetHelpTextShown(bool shown)
    {
        _helpTextShown = shown;
        NotifyStateChanged();
        // Save to localStorage - fire and forget
        _ = SaveHelpTextAsync(shown);
    }

    private async Task SaveHelpTextAsync(bool shown)
    {
        try
        {
            await _jsRuntime.InvokeVoidAsync("localStorage.setItem", HelpTextKey, shown.ToString().ToLower());
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Error saving help text setting: {ex.Message}");
        }
    }

    /// <inheritdoc/>
    public List<FactoryTab> GetFactoryTabs()
    {
        return _factoryTabs;
    }

    /// <inheritdoc/>
    public void AddFactoryTab(FactoryTab tab)
    {
        _factoryTabs.Add(tab);
        _currentFactoryTabIndex = _factoryTabs.Count - 1;
        NotifyStateChanged();
        _ = SaveFactoryTabsAsync();
    }

    /// <inheritdoc/>
    public int GetCurrentFactoryTabIndex()
    {
        return _currentFactoryTabIndex;
    }

    /// <inheritdoc/>
    public void SetCurrentFactoryTabIndex(int index)
    {
        if (index >= 0 && index < _factoryTabs.Count)
        {
            _currentFactoryTabIndex = index;
            NotifyStateChanged();
            _ = SaveCurrentTabIndexAsync(index);
        }
    }

    private async Task SaveCurrentTabIndexAsync(int index)
    {
        try
        {
            await _jsRuntime.InvokeVoidAsync("localStorage.setItem", CurrentTabIndexKey, index.ToString());
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Error saving current tab index: {ex.Message}");
        }
    }

    private async Task SaveFactoryTabsAsync()
    {
        try
        {
            string json = JsonSerializer.Serialize(_factoryTabs);
            await _jsRuntime.InvokeVoidAsync("localStorage.setItem", FactoryTabsKey, json);
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Error saving factory tabs: {ex.Message}");
        }
    }

    private void NotifyStateChanged()
    {
        OnChange?.Invoke();
    }
}
