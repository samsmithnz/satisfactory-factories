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
    private bool _helpTextShown = false;
    private const string LocalStorageKey = "factories";
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
        // Save to localStorage
        Task.Run(async () =>
        {
            try
            {
                await _jsRuntime.InvokeVoidAsync("localStorage.setItem", HelpTextKey, shown.ToString().ToLower());
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error saving help text setting: {ex.Message}");
            }
        });
    }

    private void NotifyStateChanged()
    {
        OnChange?.Invoke();
    }
}
