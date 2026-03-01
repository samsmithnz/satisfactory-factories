namespace Web.Services;

/// <summary>
/// Service for managing the Templates modal dialog visibility.
/// Renders the modal at the app root level (MainLayout) to avoid CSS stacking
/// context issues that occur when the modal is deeply nested in page content.
/// </summary>
public class TemplateModalService
{
    private bool _isOpen;
    private Func<Task>? _onTemplateLoaded;

    /// <summary>
    /// Gets whether the templates modal is currently open.
    /// </summary>
    public bool IsOpen => _isOpen;

    /// <summary>
    /// Event raised when modal state changes.
    /// </summary>
    public event Action? OnChange;

    /// <summary>
    /// Opens the templates modal.
    /// </summary>
    /// <param name="onTemplateLoaded">Optional callback to invoke after a template is loaded successfully.</param>
    public void Open(Func<Task>? onTemplateLoaded = null)
    {
        Console.WriteLine($"TemplateModalService: Open() called. IsOpen was {_isOpen}, now setting to true");
        _onTemplateLoaded = onTemplateLoaded;
        _isOpen = true;
        NotifyStateChanged();
    }

    /// <summary>
    /// Closes the templates modal.
    /// </summary>
    public void Close()
    {
        Console.WriteLine($"TemplateModalService: Close() called. IsOpen was {_isOpen}, now setting to false");
        _isOpen = false;
        _onTemplateLoaded = null;
        NotifyStateChanged();
    }

    /// <summary>
    /// Invokes the template-loaded callback if one was provided when opening.
    /// </summary>
    public async Task InvokeTemplateLoadedAsync()
    {
        Console.WriteLine($"TemplateModalService: InvokeTemplateLoadedAsync() — callback is {(_onTemplateLoaded != null ? "set" : "null")}");
        if (_onTemplateLoaded != null)
        {
            await _onTemplateLoaded();
        }
    }

    private void NotifyStateChanged() => OnChange?.Invoke();
}
