namespace Sheffield.Shared.Grains;

[GenerateSerializer]
public sealed record class CartSnapshot
{
    [Id(0)]
    public required string Id { get; init; }

    [Id(1)]
    public required int ProductCount { get; init; }
}
