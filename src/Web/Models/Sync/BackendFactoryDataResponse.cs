namespace Web.Models.Sync;

/// <summary>
/// Represents the response from the backend API when loading factory data.
/// Matches the TypeScript interface BackendFactoryDataResponse.
/// </summary>
public class BackendFactoryDataResponse
{
    /// <summary>
    /// The username of the user who owns this data.
    /// </summary>
    public string User { get; set; } = string.Empty;

    /// <summary>
    /// The factory data array.
    /// </summary>
    public List<Factory.Factory> Data { get; set; } = new List<Factory.Factory>();

    /// <summary>
    /// The timestamp when this data was last saved on the server.
    /// </summary>
    public DateTime LastSaved { get; set; }
}
