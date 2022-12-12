using System.ComponentModel.DataAnnotations;
using Application.Facades;
using Application.Services;
using Application.Models;
using Microsoft.AspNetCore.Mvc;

namespace ProductRating.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductController : ControllerBase
{
    private readonly IAmazonScrapperFacade _amazonScrapperFacade;

    public ProductController(IAmazonScrapperFacade amazonScrapperFacade)
    {
        _amazonScrapperFacade = amazonScrapperFacade;
    }

    [HttpGet("{searchTerm}/search")]
    public async Task<PagedList<Application.Models.ProductRating>> Search(
        [Required] string searchTerm,
        [FromQuery] int? page)
    {
        var result = await _amazonScrapperFacade
            .GetProductsBySearchTerm(searchTerm, page);
        return result;
    }

    [HttpGet("{asins}")]
    public async Task<Dictionary<string, ProductByAsin>> Asin(
        [Required] string asins)
    {
        var result = await _amazonScrapperFacade.GetProductByAsin(asins);
        return result;
    }
}