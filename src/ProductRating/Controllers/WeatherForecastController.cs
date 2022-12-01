using Application.Entities;
using Application.Ports;
using Application.Services;
using Infrastructure.Database;
using Microsoft.AspNetCore.Mvc;

namespace ProductRating.Controllers;

[ApiController]
[Route("api/[controller]")]
public class WeatherForecastController : ControllerBase
{
    private static readonly string[] Summaries =
    {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

    private readonly ILogger<WeatherForecastController> _logger;
    private readonly IProductRepository _repo;
    private readonly IAmazonScrapper _amazonScrapper;

    public WeatherForecastController(
        ILogger<WeatherForecastController> logger, 
        IProductRepository repo, 
        IAmazonScrapper amazonScrapper)
    {
        _logger = logger;
        _repo = repo;
        _amazonScrapper = amazonScrapper;
    }

    [HttpGet("GetWeatherForecast")]
    public async Task Get()
    {
        await _repo.Insert(new Product(){ Id = Guid.NewGuid().ToString()});
    }

    [HttpGet("test")]
    public async Task Test()
    {
        await _amazonScrapper.GetHotDeals();
    }
}