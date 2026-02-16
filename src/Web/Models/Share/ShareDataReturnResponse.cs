using Web.Models.Factory;

namespace Web.Models.Share;

/// <summary>
/// Response model for share data retrieval.
/// Mirrors TypeScript ShareDataReturnResponse interface.
/// Supports both legacy (Factory[]) and new (FactoryTab) formats.
/// </summary>
public class ShareDataReturnResponse
{
    /// <summary>
    /// The shared data. Can be either a list of factories (legacy) or a FactoryTab.
    /// </summary>
    public object? Data { get; set; }
}
