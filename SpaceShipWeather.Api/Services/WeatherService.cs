using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using SpaceShipWeather.Api.Database;
using SpaceShipWeather.Api.Models;
using SpaceShipWeather.Api.Services.OutScope;

namespace SpaceShipWeather.Api.Services;

public sealed class WeatherService : IWeatherService
{
    private readonly IWeatherClient _weatherClient;
    private readonly SpaceShipDbContext _dbContext;
    private readonly IMemoryCache _cache;
    private readonly ILogger<WeatherService> _logger;
    private readonly string _currentLatitude = "53.52";
    private readonly string _currentLongitude = "13.41";
    private readonly string _frequency = "temperature_2m";
    private readonly string _cacheKey = "weather";
    private readonly int _cacheDuration = 15;

    public WeatherService(
        SpaceShipDbContext dbContext,
        IWeatherClient weatherClient,
        IMemoryCache cache,
        ILogger<WeatherService> logger
    )
    {
        _dbContext = dbContext;
        _weatherClient = weatherClient;
        _cache = cache;
        _logger = logger;
    }

    public async ValueTask<string> GetLatestWeatherAsync(CancellationToken ct)
    {
        if (!_cache.TryGetValue(_cacheKey, out string weather))
        {
            weather = await _weatherClient.GetWeatherAsync(
                ct,
                _currentLatitude,
                _currentLongitude,
                _frequency
            );

            if (weather is not null)
            {
                await UpdateLatestData(weather);
                _cache.Set(_cacheKey, weather, TimeSpan.FromMinutes(_cacheDuration));
            }
            else
                weather = await GetLatestDataFromDB(ct);
        }

        return weather;
    }

    private async ValueTask<string> GetLatestDataFromDB(CancellationToken ct)
    {
        try
        {
            var dbData = await _dbContext.Weathers.AsNoTracking().FirstOrDefaultAsync(ct);
            return dbData?.Data;
        }
        catch (Exception e)
        {
            _logger.LogError("Retrieving data from Databae failed.");
            return null;
        }
    }

    private async Task UpdateLatestData(string data)
    {
        try
        {
            await _dbContext.Weathers.ExecuteDeleteAsync();
            await _dbContext.Weathers.AddAsync(new Weather(DateTime.UtcNow, data));
            await _dbContext.SaveChangesAsync();
        }
        catch (Exception e)
        {
            _logger.LogError("Failed to set latest data on Database.");
        }
    }
}
