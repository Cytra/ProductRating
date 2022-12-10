using System.ComponentModel.DataAnnotations;
using Application.Entities;
using Application.Ports;
using Application.Services;
using Application.Models;
using Microsoft.AspNetCore.Mvc;

namespace ProductRating.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductController : ControllerBase
{
    private readonly IAmazonScrapper _amazonScrapper;

    public ProductController(IAmazonScrapper amazonScrapper)
    {
        _amazonScrapper = amazonScrapper;
    }

    [HttpGet("{searchTerm}/search")]
    public async Task<PagedList<Application.Models.ProductRating>> Search(
        [Required] string searchTerm,
        [FromQuery] int? page)
    {
        var result = await _amazonScrapper
            .GetProductsBySearchTerm(searchTerm, page);
        return result;
    }

    [HttpGet("{asin}")]
    public async Task<IActionResult> Asin([Required] string asin)
    {
        await _amazonScrapper.GetProductByAsin(asin);
        return Ok();
    }
}