namespace Web.Models.Factory;

/// <summary>
/// Represents building requirements for a factory.
/// </summary>
public class BuildingRequirement
{
    /// <summary>
    /// Name of the building.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Amount of buildings required.
    /// </summary>
    public double Amount { get; set; }

    /// <summary>
    /// Power consumed by the buildings (optional).
    /// </summary>
    public double? PowerConsumed { get; set; }

    /// <summary>
    /// Power produced by the buildings (optional).
    /// </summary>
    public double? PowerProduced { get; set; }
}
