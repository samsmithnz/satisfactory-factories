using System.Text.Json.Serialization;

namespace Web.Models;

public class RecipeItem
{
    [JsonPropertyName("part")]
    public string Part { get; set; } = string.Empty;

    [JsonPropertyName("amount")]
    public double Amount { get; set; }

    [JsonPropertyName("perMin")]
    public double PerMin { get; set; }

    [JsonPropertyName("isByProduct")]
    public bool? IsByProduct { get; set; }
}
