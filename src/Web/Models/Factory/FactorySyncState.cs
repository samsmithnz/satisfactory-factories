namespace Web.Models.Factory;

/// <summary>
/// Represents sync state for a factory product.
/// </summary>
public class FactorySyncState
{
    /// <summary>
    /// Amount in sync state.
    /// </summary>
    public double Amount { get; set; }

    /// <summary>
    /// Recipe in sync state.
    /// </summary>
    public string Recipe { get; set; } = string.Empty;
}

/// <summary>
/// Represents sync state for factory power generation.
/// </summary>
public class FactoryPowerSyncState
{
    /// <summary>
    /// Building amount in sync state.
    /// </summary>
    public double BuildingAmount { get; set; }

    /// <summary>
    /// Power amount in sync state.
    /// </summary>
    public double PowerAmount { get; set; }

    /// <summary>
    /// Recipe and fuel used in sync state.
    /// </summary>
    public string Recipe { get; set; } = string.Empty;

    /// <summary>
    /// Ingredient amount in sync state.
    /// </summary>
    public double IngredientAmount { get; set; }
}
