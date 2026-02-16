namespace Parser.Models;

public record ParserIngredient
{
    public required string Part { get; init; }
    public double? Amount { get; init; }
    public required double PerMin { get; init; }
}

public record ParserProduct
{
    public required string Part { get; init; }
    public required double Amount { get; init; }
    public required double PerMin { get; init; }
    public bool? IsByProduct { get; init; }
}

public record ParserRecipe
{
    public required string Id { get; init; }
    public required string DisplayName { get; init; }
    public required List<ParserIngredient> Ingredients { get; init; }
    public required List<ParserProduct> Products { get; init; }
    public required ParserBuilding Building { get; init; }
    public required bool IsAlternate { get; init; }
    public required bool IsFicsmas { get; init; }
}

public record ParserBuilding
{
    public required string Name { get; init; }
    public required double Power { get; init; }
    public double? MinPower { get; init; }
    public double? MaxPower { get; init; }
}
