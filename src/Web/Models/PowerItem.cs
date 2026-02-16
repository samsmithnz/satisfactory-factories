using System.Text.Json.Serialization;

namespace Web.Models;

public class PowerItem
{
    [JsonPropertyName("part")]
    public string Part { get; set; } = string.Empty;

    [JsonPropertyName("perMin")]
    public double PerMin { get; set; }

    [JsonPropertyName("amount")]
    public double? Amount { get; set; }

    [JsonPropertyName("mwPerItem")]
    public double? MwPerItem { get; set; }

    [JsonPropertyName("supplementalRatio")]
    public double? SupplementalRatio { get; set; }
}
