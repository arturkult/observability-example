using Microsoft.EntityFrameworkCore;

namespace DotnetObservabilityExample;

public class ApplicationDbContext:DbContext
{
    public DbSet<WeatherForecast> WeatherForecasts { get; set; }

    public ApplicationDbContext(DbContextOptions options) : base(options)
    {
    }
    
    
}