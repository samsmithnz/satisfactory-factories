namespace Parser.Models;

public record ParserPowerItem
{
    public required string Part { get; init; }
    public required double PerMin { get; init; }
    public double? MwPerItem { get; init; }
    public double? SupplementalRatio { get; init; }
}

public record ParserPowerRecipe
{
    public required string Id { get; init; }
    public required string DisplayName { get; init; }
    public required List<ParserPowerItem> Ingredients { get; init; }
    public ParserPowerItem? Byproduct { get; init; }
    public required ParserPowerRecipeBuilding Building { get; init; }
}

public record ParserPowerRecipeBuilding
{
    public required string Name { get; init; }
    public required double Power { get; init; }
}

public record ParserFuel
{
    public required string PrimaryFuel { get; init; }
    public required string SupplementalResource { get; init; }
    public required string ByProduct { get; init; }
    public required double ByProductAmount { get; init; }
    public required double ByProductAmountPerMin { get; init; }
    public required double BurnDurationInS { get; init; }
}
