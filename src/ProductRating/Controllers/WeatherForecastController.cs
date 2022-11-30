using Application.Entities;
using Application.Ports;
using Infrastructure.Database;
using Microsoft.AspNetCore.Mvc;

namespace ProductRating.Controllers;

[ApiController]
[Route("[controller]")]
public class WeatherForecastController : ControllerBase
{
    private static readonly string[] Summaries =
    {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

    private readonly ILogger<WeatherForecastController> _logger;
    private readonly IProductRepository _repo;

    public WeatherForecastController(
        ILogger<WeatherForecastController> logger, 
        IProductRepository repo)
    {
        _logger = logger;
        _repo = repo;
    }

    [HttpGet(Name = "GetWeatherForecast")]
    public async Task Get()
    {
        await _repo.Insert(new Product(){ Id = Guid.NewGuid().ToString()});
    }
}