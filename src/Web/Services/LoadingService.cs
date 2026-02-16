namespace Web.Services;

/// <summary>
/// Service for managing loading overlay state.
/// Replaces the event bus pattern from Vue SaveLoader component.
/// </summary>
public class LoadingService
{
    private bool _isLoading;
    private string _title = "Loading...";
    private int _totalSteps = 1;
    private int _currentStep;
    private string _message = string.Empty;

    /// <summary>
    /// Event raised when loading state changes.
    /// </summary>
    public event Action? OnChange;

    /// <summary>
    /// Gets whether loading is currently in progress.
    /// </summary>
    public bool IsLoading => _isLoading;

    /// <summary>
    /// Gets the loading title.
    /// </summary>
    public string Title => _title;

    /// <summary>
    /// Gets the total number of steps.
    /// </summary>
    public int TotalSteps => _totalSteps;

    /// <summary>
    /// Gets the current step number.
    /// </summary>
    public int CurrentStep => _currentStep;

    /// <summary>
    /// Gets the current loading message.
    /// </summary>
    public string Message => _message;

    /// <summary>
    /// Initializes a new loading operation.
    /// </summary>
    /// <param name="title">Title to display.</param>
    /// <param name="steps">Total number of steps.</param>
    public void Initialize(string title, int steps)
    {
        _isLoading = true;
        _title = title;
        _totalSteps = steps;
        _currentStep = 0;
        _message = string.Empty;
        NotifyStateChanged();
    }

    /// <summary>
    /// Updates the loading progress to the next step.
    /// </summary>
    /// <param name="message">Message to display for this step.</param>
    /// <param name="isFinalStep">Whether this is the final step.</param>
    public void IncrementStep(string message, bool isFinalStep = false)
    {
        _currentStep++;
        _message = message;
        
        if (_currentStep >= _totalSteps || isFinalStep)
        {
            // Complete after a short delay to show the final message
            _ = Task.Run(async () =>
            {
                try
                {
                    await Task.Delay(100);
                    Complete();
                }
                catch (Exception ex)
                {
                    Console.Error.WriteLine($"Error during loading completion: {ex.Message}");
                    Complete(); // Ensure we complete even if there's an error
                }
            });
        }
        
        NotifyStateChanged();
    }

    /// <summary>
    /// Sets a specific step number.
    /// </summary>
    /// <param name="step">Step number to set.</param>
    /// <param name="message">Message to display.</param>
    public void SetStep(int step, string message)
    {
        _currentStep = step;
        _message = message;
        NotifyStateChanged();
    }

    /// <summary>
    /// Completes the loading operation.
    /// </summary>
    public void Complete()
    {
        _isLoading = false;
        _currentStep = 0;
        _message = string.Empty;
        NotifyStateChanged();
    }

    private void NotifyStateChanged()
    {
        OnChange?.Invoke();
    }
}
