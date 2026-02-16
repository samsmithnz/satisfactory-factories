namespace Web.Models.Factory;

/// <summary>
/// Represents an input to a factory from another factory.
/// </summary>
public class FactoryInput
{
    /// <summary>
    /// ID of the factory supplying this input (nullable).
    /// </summary>
    public int? FactoryId { get; set; }

    /// <summary>
    /// Part being output (nullable).
    /// </summary>
    public string? OutputPart { get; set; }

    /// <summary>
    /// Amount being supplied.
    /// </summary>
    public double Amount { get; set; }
}
