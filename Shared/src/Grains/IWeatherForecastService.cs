namespace Sheffield.Shared.Grains;

public interface IWeatherForecastService : IGrainWithStringKey
{
    Task<IEnumerable<WeatherForecast>> GetWeatherForecastAsync();
}
