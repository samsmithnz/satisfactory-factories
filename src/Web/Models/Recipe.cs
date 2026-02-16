using System.Text.Json.Serialization;

namespace Web.Models;

public class Recipe
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    [JsonPropertyName("displayName")]
    public string DisplayName { get; set; } = string.Empty;

    [JsonPropertyName("ingredients")]
    public List<RecipeItem> Ingredients { get; set; } = new List<RecipeItem>();

    [JsonPropertyName("products")]
    public List<RecipeItem> Products { get; set; } = new List<RecipeItem>();

    [JsonPropertyName("byproduct")]
    public List<RecipeItem>? Byproduct { get; set; }

    [JsonPropertyName("building")]
    public RecipeBuilding Building { get; set; } = new RecipeBuilding();

    [JsonPropertyName("isAlternate")]
    public bool IsAlternate { get; set; }

    [JsonPropertyName("isFicsmas")]
    public bool IsFicsmas { get; set; }
}
