using System.Text.Json.Serialization;

namespace Web.Models;

public class RecipeBuilding
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("power")]
    public double Power { get; set; }
}
