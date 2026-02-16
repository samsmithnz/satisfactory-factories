namespace Web.Services;

/// <summary>
/// Configuration service for application settings.
/// Provides API URL and other configuration values.
/// </summary>
public interface IConfigurationService
{
    /// <summary>
    /// Gets the API base URL.
    /// </summary>
    string GetApiUrl();

    /// <summary>
    /// Gets the data version.
    /// </summary>
    string GetDataVersion();
}
