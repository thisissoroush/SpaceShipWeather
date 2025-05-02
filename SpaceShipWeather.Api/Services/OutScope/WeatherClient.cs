namespace SpaceShipWeather.Api.Services.OutScope;

public sealed class WeatherClient : IWeatherClient
{
    private readonly HttpClient _httpClient;

    private readonly ILogger<WeatherClient> _logger;

    public WeatherClient(HttpClient httpClient, ILogger<WeatherClient> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<string> GetWeatherAsync(
        CancellationToken ct,
        string latitude,
        string longitude,
        string frequency
    )
    {
        try
        {
            var url = $"v1/forecast?latitude={latitude}&longitude={longitude}&hourly={frequency}";
            var response = await _httpClient.GetAsync(url, ct);
            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            return await response.Content.ReadAsStringAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError("Failed to get data from api!");
            return null;
        }
    }
}
