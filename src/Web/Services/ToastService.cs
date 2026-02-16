namespace Web.Services;

/// <summary>
/// Implementation of toast notification service.
/// </summary>
public class ToastService : IToastService
{
    /// <inheritdoc/>
    public event Action<string, ToastType>? OnShowToast;

    /// <inheritdoc/>
    public void ShowToast(string message, ToastType type = ToastType.Info)
    {
        OnShowToast?.Invoke(message, type);
    }
}
