namespace SpaceShipWeather.Api.Services.OutScope;

public interface IWeatherClient
{
    Task<string> GetWeatherAsync(CancellationToken ct, string lat, string lon, string freq);
}
