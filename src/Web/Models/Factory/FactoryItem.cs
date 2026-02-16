namespace Web.Models.Factory;

/// <summary>
/// Represents a factory product or item.
/// </summary>
public class FactoryItem
{
    /// <summary>
    /// ID of the item.
    /// </summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// Recipe used to produce this item.
    /// </summary>
    public string Recipe { get; set; } = string.Empty;

    /// <summary>
    /// Amount to produce.
    /// </summary>
    public double Amount { get; set; }

    /// <summary>
    /// Display order for UI.
    /// </summary>
    public int DisplayOrder { get; set; }

    /// <summary>
    /// Requirements for producing this item.
    /// Key is the part ID, value contains the amount required.
    /// </summary>
    public Dictionary<string, RequirementAmount> Requirements { get; set; } = new Dictionary<string, RequirementAmount>();

    /// <summary>
    /// Building requirements for this item.
    /// </summary>
    public BuildingRequirement BuildingRequirements { get; set; } = new BuildingRequirement();

    /// <summary>
    /// Byproducts generated when producing this item (optional).
    /// </summary>
    public List<ByProductItem>? ByProducts { get; set; }
}

/// <summary>
/// Represents the amount required for a requirement.
/// </summary>
public class RequirementAmount
{
    /// <summary>
    /// Amount required.
    /// </summary>
    public double Amount { get; set; }
}
