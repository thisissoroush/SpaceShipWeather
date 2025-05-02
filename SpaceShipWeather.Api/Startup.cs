using Microsoft.EntityFrameworkCore;
using SpaceShipWeather.Api.Database;
using SpaceShipWeather.Api.Middlewares;
using SpaceShipWeather.Api.Services;
using SpaceShipWeather.Api.Services.OutScope;

namespace SpaceShipWeather.Api;

public class Startup
{
    public IConfiguration Configuration { get; }

    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddHttpClient<IWeatherClient, WeatherClient>(client =>
        {
            client.BaseAddress = new Uri("https://api.open-meteo.com/");
            client.DefaultRequestHeaders.Add("User-Agent", "SpaceShipWeatherApiClient");
            client.Timeout = TimeSpan.FromSeconds(5);
        });

        services.AddControllers();

        services.AddDbContext<SpaceShipDbContext>(options =>
            options.UseSqlite(Configuration.GetConnectionString("Default"))
        );

        services.AddScoped<IWeatherService, WeatherService>();

        services.AddMemoryCache();
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        using (var serviceScope = app.ApplicationServices.CreateScope())
        {
            var dbContext = serviceScope.ServiceProvider.GetRequiredService<SpaceShipDbContext>();
            dbContext.Database.Migrate();
        }

        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }

        app.UseMiddleware<GlobalExceptionMiddleware>();

        app.UseRouting();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
        });
    }
}
