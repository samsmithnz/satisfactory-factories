namespace Web.Models.Factory;

/// <summary>
/// Represents metrics for a part in a factory.
/// </summary>
public class PartMetrics
{
    /// <summary>
    /// Total amount required by all products on the line.
    /// </summary>
    public double AmountRequired { get; set; }

    /// <summary>
    /// Total amount required by production.
    /// </summary>
    public double AmountRequiredProduction { get; set; }

    /// <summary>
    /// Total amount required by all exports.
    /// </summary>
    public double AmountRequiredExports { get; set; }

    /// <summary>
    /// Total amount required for power generation.
    /// </summary>
    public double AmountRequiredPower { get; set; }

    /// <summary>
    /// Total amount of surplus used for display purposes.
    /// </summary>
    public double AmountSupplied { get; set; }

    /// <summary>
    /// Amount supplied by the inputs.
    /// </summary>
    public double AmountSuppliedViaInput { get; set; }

    /// <summary>
    /// Amount supplied by the raw resources assumed to be handled by the user.
    /// </summary>
    public double AmountSuppliedViaRaw { get; set; }

    /// <summary>
    /// Amount supplied by internal products.
    /// </summary>
    public double AmountSuppliedViaProduction { get; set; }

    /// <summary>
    /// Amount remaining after all inputs and internal products are accounted for.
    /// Can be a negative number, which is used for surplus calculations.
    /// </summary>
    public double AmountRemaining { get; set; }

    /// <summary>
    /// Whether the part is a raw resource or not.
    /// If so, it will always be marked as satisfied.
    /// </summary>
    public bool IsRaw { get; set; }

    /// <summary>
    /// Use of use flag for templating.
    /// </summary>
    public bool Satisfied { get; set; }

    /// <summary>
    /// Whether the product should be a candidate for imports.
    /// </summary>
    public bool Exportable { get; set; }
}
