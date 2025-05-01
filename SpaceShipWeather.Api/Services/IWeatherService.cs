using SpaceShipWeather.Api.Models;

namespace SpaceShipWeather.Api.Services;

public interface IWeatherService
{
    ValueTask<string> GetLatestWeatherAsync(CancellationToken ct);
}
