using Application.Entities;
using Application.Ports;
using Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace ProductRating.Controllers;

[ApiController]
[Route("api/[controller]")]
public class WeatherForecastController : ControllerBase
{
    private readonly IProductRepository _repo;
    private readonly IAmazonScrapper _amazonScrapper;

    public WeatherForecastController(
        IProductRepository repo, 
        IAmazonScrapper amazonScrapper)
    {
        _repo = repo;
        _amazonScrapper = amazonScrapper;
    }

    [HttpGet("InsertToDbTest")]
    public async Task Get()
    {
        await _repo.Insert(new Product(){ Id = Guid.NewGuid().ToString()});
    }

    [HttpGet("test")]
    public IActionResult Test()
    {
        _amazonScrapper.GetProductsBySearchTerm("dell laptop");
        return Ok();
    }
}