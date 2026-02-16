namespace Web.Services;

/// <summary>
/// Service for displaying toast notifications to the user.
/// </summary>
public interface IToastService
{
    /// <summary>
    /// Event raised when a toast message should be shown.
    /// </summary>
    event Action<string, ToastType>? OnShowToast;

    /// <summary>
    /// Shows a toast message.
    /// </summary>
    /// <param name="message">The message to display.</param>
    /// <param name="type">The type of toast (info, success, error).</param>
    void ShowToast(string message, ToastType type = ToastType.Info);
}

/// <summary>
/// Type of toast notification.
/// </summary>
public enum ToastType
{
    Info,
    Success,
    Error
}
