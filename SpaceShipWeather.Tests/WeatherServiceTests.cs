using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using SpaceShipWeather.Api.Database;
using SpaceShipWeather.Api.Models;
using SpaceShipWeather.Api.Services;
using SpaceShipWeather.Api.Services.OutScope;

namespace SpaceShipWeather.Tests;

public class WeatherServiceTests
{
    private WeatherService CreateService(
        IWeatherClient _weatherClient = null,
        SpaceShipDbContext _dbContext = null,
        IMemoryCache _memoryCache = null
    )
    {
        _weatherClient ??= Mock.Of<IWeatherClient>();
        _dbContext ??= new SpaceShipDbContext(
            new DbContextOptionsBuilder<SpaceShipDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options
        );

        _memoryCache ??= new MemoryCache(new MemoryCacheOptions());

        return new WeatherService(
            _dbContext,
            _weatherClient,
            _memoryCache,
            NullLogger<WeatherService>.Instance
        );
    }

    [Fact]
    public async Task Should_Return_Weather_From_Client_If_Available()
    {
        // Arrange
        var expectedWeather = "HttpClientResult";
        var mockClient = new Mock<IWeatherClient>();
        mockClient
            .Setup(x =>
                x.GetWeatherAsync(
                    It.IsAny<CancellationToken>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>()
                )
            )
            .ReturnsAsync(expectedWeather);

        var service = CreateService(mockClient.Object);

        // Act
        var result = await service.GetLatestWeatherAsync(CancellationToken.None);

        // Assert
        Assert.Equal(expectedWeather, result);
    }

    [Fact]
    public async Task Should_Return_Weather_From_Db_If_Client_Returns_Null()
    {
        // Arrange
        var dbOptions = new DbContextOptionsBuilder<SpaceShipDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        var dbReturnedData = "DbReturnedData";
        var dbContext = new SpaceShipDbContext(dbOptions);
        var dbWeather = new Weather(DateTime.UtcNow, dbReturnedData);
        await dbContext.Weathers.AddAsync(dbWeather);
        await dbContext.SaveChangesAsync();

        var mockClient = new Mock<IWeatherClient>();
        mockClient
            .Setup(x =>
                x.GetWeatherAsync(
                    It.IsAny<CancellationToken>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>()
                )
            )
            .ReturnsAsync((string)null);

        var service = CreateService(mockClient.Object, dbContext);

        // Act
        var result = await service.GetLatestWeatherAsync(CancellationToken.None);

        // Assert
        Assert.Equal(dbReturnedData, result);
    }

    [Fact]
    public async Task Should_Return_Null_If_Client_And_Db_Both_Return_Null()
    {
        // Arrange
        var dbOptions = new DbContextOptionsBuilder<SpaceShipDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        var dbContext = new SpaceShipDbContext(dbOptions);

        var mockClient = new Mock<IWeatherClient>();
        mockClient
            .Setup(x =>
                x.GetWeatherAsync(
                    It.IsAny<CancellationToken>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>()
                )
            )
            .ReturnsAsync((string)null);

        var service = CreateService(mockClient.Object, dbContext);

        // Act
        var result = await service.GetLatestWeatherAsync(CancellationToken.None);

        // Assert
        Assert.Null(result);
    }
}
