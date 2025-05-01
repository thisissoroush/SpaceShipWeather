using Microsoft.AspNetCore.Mvc;
using SpaceShipWeather.Api.Services;

namespace SpaceShipWeather.Api;

[ApiController]
[Route("api/[controller]")]
public class WeatherController : ControllerBase
{
    private readonly IWeatherService _weatherService;

    public WeatherController(IWeatherService weatherService)
    {
        _weatherService = weatherService;
    }

    [HttpGet]
    public async Task<string> Get(CancellationToken ct) =>
        await _weatherService.GetLatestWeatherAsync(ct);
}
