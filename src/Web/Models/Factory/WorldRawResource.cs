namespace Web.Models.Factory;

/// <summary>
/// Represents a raw resource available in the world.
/// </summary>
public class WorldRawResource
{
    /// <summary>
    /// ID of the raw resource.
    /// </summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// Name of the raw resource.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Amount available.
    /// </summary>
    public double Amount { get; set; }
}
