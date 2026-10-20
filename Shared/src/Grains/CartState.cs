namespace Sheffield.Shared.Grains;

[GenerateSerializer]
public sealed class CartState
{
    [Id(0)]
    public int ProductCount { get; set; }
}
