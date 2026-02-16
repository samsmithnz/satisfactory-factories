namespace Web.Models.Factory;

/// <summary>
/// Represents a tab containing multiple factories.
/// Mirrors TypeScript FactoryTab interface.
/// </summary>
public class FactoryTab
{
    /// <summary>
    /// Unique identifier for the tab.
    /// </summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// Name of the tab.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Factories contained in this tab.
    /// </summary>
    public List<Factory> Factories { get; set; } = new List<Factory>();
}
