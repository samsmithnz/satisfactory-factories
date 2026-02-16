using System.Text.Json.Serialization;

namespace Web.Models;

public class GameDataItems
{
    [JsonPropertyName("parts")]
    public Dictionary<string, Part> Parts { get; set; } = new Dictionary<string, Part>();

    [JsonPropertyName("rawResources")]
    public Dictionary<string, RawResource> RawResources { get; set; } = new Dictionary<string, RawResource>();
}
