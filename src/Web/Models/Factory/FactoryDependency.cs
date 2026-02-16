namespace Web.Models.Factory;

/// <summary>
/// Represents a dependency request from one factory to another.
/// </summary>
public class FactoryDependencyRequest
{
    /// <summary>
    /// ID of the requesting factory.
    /// </summary>
    public int RequestingFactoryId { get; set; }

    /// <summary>
    /// Part being requested.
    /// </summary>
    public string Part { get; set; } = string.Empty;

    /// <summary>
    /// Amount being requested.
    /// </summary>
    public double Amount { get; set; }
}

/// <summary>
/// Represents metrics for a factory dependency.
/// </summary>
public class FactoryDependencyMetrics
{
    /// <summary>
    /// Part being tracked.
    /// </summary>
    public string Part { get; set; } = string.Empty;

    /// <summary>
    /// Amount requested.
    /// </summary>
    public double Request { get; set; }

    /// <summary>
    /// Amount supplied.
    /// </summary>
    public double Supply { get; set; }

    /// <summary>
    /// Whether the request is satisfied.
    /// </summary>
    public bool IsRequestSatisfied { get; set; }

    /// <summary>
    /// Difference between supply and request.
    /// </summary>
    public double Difference { get; set; }
}

/// <summary>
/// Represents factory dependencies.
/// </summary>
public class FactoryDependency
{
    /// <summary>
    /// Dependency requests grouped by part.
    /// Key is the part ID, value is list of requests.
    /// </summary>
    public Dictionary<string, List<FactoryDependencyRequest>> Requests { get; set; } = new Dictionary<string, List<FactoryDependencyRequest>>();

    /// <summary>
    /// Dependency metrics grouped by part.
    /// Key is the part ID, value is the metrics.
    /// </summary>
    public Dictionary<string, FactoryDependencyMetrics> Metrics { get; set; } = new Dictionary<string, FactoryDependencyMetrics>();
}
