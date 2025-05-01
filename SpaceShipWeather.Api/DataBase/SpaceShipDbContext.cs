using Microsoft.EntityFrameworkCore;
using SpaceShipWeather.Api.Models;

namespace SpaceShipWeather.Api.Database;

public class SpaceShipDbContext : DbContext
{
    public SpaceShipDbContext(DbContextOptions<SpaceShipDbContext> options)
        : base(options) { }

    public DbSet<Weather> Weathers { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Weather>(entity =>
        {
            entity.HasKey(p => p.Id);
            entity.Property(p => p.Data).HasMaxLength(4000);
        });
    }
}
