namespace Web.Services;

/// <summary>
/// Configuration service implementation.
/// Provides API URL based on environment and data version.
/// </summary>
public class ConfigurationService : IConfigurationService
{
    private const string ProductionApiUrl = "https://api.satisfactory-factories.app";
    private const string DevelopmentApiUrl = "http://localhost:3001";
    private const string DataVersion = "1.0-29";

    /// <inheritdoc/>
    public string GetApiUrl()
    {
        // In production builds, use production API; otherwise use development
        // Blazor WebAssembly doesn't have a built-in environment concept like ASP.NET Core,
        // so we detect based on the hostname
#if DEBUG
        return DevelopmentApiUrl;
#else
        return ProductionApiUrl;
#endif
    }

    /// <inheritdoc/>
    public string GetDataVersion()
    {
        return DataVersion;
    }
}
