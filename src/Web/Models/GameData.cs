using System.Text.Json.Serialization;

namespace Web.Models;

public class GameData
{
    [JsonPropertyName("buildings")]
    public Dictionary<string, double> Buildings { get; set; } = new Dictionary<string, double>();

    [JsonPropertyName("items")]
    public GameDataItems Items { get; set; } = new GameDataItems();

    [JsonPropertyName("recipes")]
    public List<Recipe> Recipes { get; set; } = new List<Recipe>();

    [JsonPropertyName("powerGenerationRecipes")]
    public List<PowerRecipe> PowerGenerationRecipes { get; set; } = new List<PowerRecipe>();
}
