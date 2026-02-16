namespace Parser.Models;

public record ParserPart
{
    public required string Name { get; init; }
    public required int StackSize { get; init; }
    public required bool IsFluid { get; init; }
    public required bool IsFicsmas { get; init; }
    public required int EnergyGeneratedInMJ { get; init; }
}

public record ParserItemDataInterface
{
    public required Dictionary<string, ParserPart> Parts { get; init; }
    public required Dictionary<string, ParserRawResource> RawResources { get; init; }
}

public record ParserRawResource
{
    public required string Name { get; init; }
    public required long Limit { get; init; }
}
