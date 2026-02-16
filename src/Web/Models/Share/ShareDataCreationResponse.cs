namespace Web.Models.Share;

/// <summary>
/// Response model for share data creation.
/// Mirrors TypeScript ShareDataCreationResponse interface.
/// </summary>
public class ShareDataCreationResponse
{
    /// <summary>
    /// Status of the share creation.
    /// </summary>
    public string Status { get; set; } = string.Empty;

    /// <summary>
    /// The generated share ID.
    /// </summary>
    public string ShareId { get; set; } = string.Empty;
}
