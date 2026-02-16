namespace Web.Models.Factory;

/// <summary>
/// Represents power information for a factory.
/// </summary>
public class FactoryPower
{
    /// <summary>
    /// Power consumed by the factory.
    /// </summary>
    public double Consumed { get; set; }

    /// <summary>
    /// Power produced by the factory.
    /// </summary>
    public double Produced { get; set; }

    /// <summary>
    /// Difference between produced and consumed (produced - consumed).
    /// </summary>
    public double Difference { get; set; }
}
