namespace Web.Models.Factory;

/// <summary>
/// Represents a power producer in a factory.
/// </summary>
public class FactoryPowerProducer
{
    /// <summary>
    /// Building type used for power generation.
    /// </summary>
    public string Building { get; set; } = string.Empty;

    /// <summary>
    /// Amount of buildings requested by the user.
    /// </summary>
    public double BuildingAmount { get; set; }

    /// <summary>
    /// Amount of buildings actually needed to produce the power requested by the user.
    /// </summary>
    public double BuildingCount { get; set; }

    /// <summary>
    /// Ingredients/fuel for power generation.
    /// </summary>
    public List<PowerItem> Ingredients { get; set; } = new List<PowerItem>();

    /// <summary>
    /// Enables the user to specify the quantity of fuel to use.
    /// </summary>
    public double IngredientAmount { get; set; }

    /// <summary>
    /// Byproduct produced (e.g., uranium waste), added back to factory.parts.
    /// </summary>
    public PowerByproduct? Byproduct { get; set; }

    /// <summary>
    /// Amount of energy user is requesting to be generated.
    /// </summary>
    public double PowerAmount { get; set; }

    /// <summary>
    /// Amount of energy actually produced calculated from requested ingredientAmount and powerAmount.
    /// </summary>
    public double PowerProduced { get; set; }

    /// <summary>
    /// Recipe for power generation.
    /// </summary>
    public string Recipe { get; set; } = string.Empty;

    /// <summary>
    /// Display order for UI.
    /// </summary>
    public int DisplayOrder { get; set; }

    /// <summary>
    /// Denotes what was just updated so we can recalculate the power generation
    /// based off ingredientAmount or powerAmount (nullable).
    /// </summary>
    public string? Updated { get; set; }
}

/// <summary>
/// Represents a power item ingredient.
/// </summary>
public class PowerItem
{
    /// <summary>
    /// Part ID.
    /// </summary>
    public string Part { get; set; } = string.Empty;

    /// <summary>
    /// Amount per minute.
    /// </summary>
    public double PerMin { get; set; }
}

/// <summary>
/// Represents a power generation byproduct.
/// </summary>
public class PowerByproduct
{
    /// <summary>
    /// Part ID of the byproduct.
    /// </summary>
    public string Part { get; set; } = string.Empty;

    /// <summary>
    /// Amount of the byproduct.
    /// </summary>
    public double Amount { get; set; }
}
