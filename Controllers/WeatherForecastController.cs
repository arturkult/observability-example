using Microsoft.AspNetCore.Mvc;

namespace DotnetObservabilityExample.Controllers;

[ApiController]
[Route("[controller]")]
public class WeatherForecastController : ControllerBase
{
    private static readonly string[] Summaries = new[]
    {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

    private readonly ILogger<WeatherForecastController> _logger;
    private readonly IHttpClientFactory _factory;
    private readonly ApplicationDbContext _dbContext;

    public WeatherForecastController(ILogger<WeatherForecastController> logger, IHttpClientFactory factory, ApplicationDbContext dbContext)
    {
        _logger = logger;
        _factory = factory;
        _dbContext = dbContext;
    }

    [HttpGet(Name = "GetWeatherForecast")]
    public IEnumerable<WeatherForecast> Get()
    {
        _logger.LogInformation("Getting a forecast");

        _dbContext.WeatherForecasts.Add(new WeatherForecast
        {
            Date = DateOnly.FromDateTime(DateTime.Now.AddDays(1)),
            TemperatureC = Random.Shared.Next(-20, 55),
            Summary = Summaries[Random.Shared.Next(Summaries.Length)]
        });
        _dbContext.SaveChanges();
        
        return Enumerable.Range(1, 5).Select(index => new WeatherForecast
        {
            Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            TemperatureC = Random.Shared.Next(-20, 55),
            Summary = Summaries[Random.Shared.Next(Summaries.Length)]
        })
        .ToArray();
    }

    [HttpGet("joke",  Name = "GetJoke")]
    public async Task<IActionResult> GetJoke()
    {
        _logger.LogInformation("Getting joke");
        var client = _factory.CreateClient();
        var response = await client.GetAsync("https://official-joke-api.appspot.com/random_joke");

        return Ok(await response.Content.ReadAsStreamAsync());
    }
}
