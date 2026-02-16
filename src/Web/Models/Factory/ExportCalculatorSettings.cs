namespace Web.Models.Factory;

/// <summary>
/// Represents export calculator settings for a factory.
/// </summary>
public class ExportCalculatorFactorySettings
{
    /// <summary>
    /// Train time in minutes.
    /// </summary>
    public double TrainTime { get; set; }

    /// <summary>
    /// Drone time in minutes.
    /// </summary>
    public double DroneTime { get; set; }

    /// <summary>
    /// Truck time in minutes.
    /// </summary>
    public double TruckTime { get; set; }

    /// <summary>
    /// Tractor time in minutes.
    /// </summary>
    public double TractorTime { get; set; }
}

/// <summary>
/// Represents export calculator settings.
/// </summary>
public class ExportCalculatorSettings
{
    /// <summary>
    /// Selected option (nullable).
    /// </summary>
    public string? Selected { get; set; }

    /// <summary>
    /// Factory-specific settings.
    /// Key is the factory ID as string.
    /// </summary>
    public Dictionary<string, ExportCalculatorFactorySettings> FactorySettings { get; set; } = new Dictionary<string, ExportCalculatorFactorySettings>();
}
