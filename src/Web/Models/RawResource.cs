using System.Text.Json.Serialization;

namespace Web.Models;

public class RawResource
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("limit")]
    public long Limit { get; set; }
}
