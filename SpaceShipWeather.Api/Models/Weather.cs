namespace SpaceShipWeather.Api.Models;

public class Weather
{
    public Weather() { }

    public Weather(DateTime createDateUtc, string data)
    {
        Id = long.Parse(createDateUtc.ToString("yyyyMMddHHmmssfff"));
        Data = data;
    }

    public long Id { get; private set; }
    public string Data { get; private set; }
}
