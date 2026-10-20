namespace Sheffield.Shared.Grains;

[GenerateSerializer]
public sealed record class WeatherForecast
{
    [Id(0)]
    public required DateOnly Date { get; init; }

    [Id(1)]
    public required int TemperatureC { get; init; }

    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);

    [Id(2)]
    public string? Summary { get; init; } = default;
}
