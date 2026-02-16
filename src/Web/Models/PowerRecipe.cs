using System.Text.Json.Serialization;

namespace Web.Models;

public class PowerRecipe
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    [JsonPropertyName("displayName")]
    public string DisplayName { get; set; } = string.Empty;

    [JsonPropertyName("ingredients")]
    public List<PowerItem> Ingredients { get; set; } = new List<PowerItem>();

    [JsonPropertyName("byproduct")]
    public PowerItem? Byproduct { get; set; }

    [JsonPropertyName("building")]
    public RecipeBuilding Building { get; set; } = new RecipeBuilding();
}
