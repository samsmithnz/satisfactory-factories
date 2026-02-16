using System.Text.Json.Serialization;

namespace Web.Models;

public class Part
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("stackSize")]
    public int StackSize { get; set; }

    [JsonPropertyName("isFluid")]
    public bool IsFluid { get; set; }

    [JsonPropertyName("isFicsmas")]
    public bool IsFicsmas { get; set; }

    [JsonPropertyName("energyGeneratedInMJ")]
    public double EnergyGeneratedInMJ { get; set; }
}
