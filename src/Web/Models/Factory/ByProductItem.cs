namespace Web.Models.Factory;

/// <summary>
/// Represents a byproduct item.
/// </summary>
public class ByProductItem
{
    /// <summary>
    /// ID of the part.
    /// </summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// Amount of the byproduct.
    /// </summary>
    public double Amount { get; set; }

    /// <summary>
    /// Product ID this is a byproduct of.
    /// </summary>
    public string ByProductOf { get; set; } = string.Empty;
}
