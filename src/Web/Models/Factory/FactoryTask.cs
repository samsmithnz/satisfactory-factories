namespace Web.Models.Factory;

/// <summary>
/// Represents a task for a factory.
/// </summary>
public class FactoryTask
{
    /// <summary>
    /// Title of the task.
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Whether the task is completed.
    /// </summary>
    public bool Completed { get; set; }
}
