using System.ComponentModel.DataAnnotations;
using Application.Commands;
using Application.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc; 

namespace ProductRating.Controllers;

[ApiController]
[Route("api/product/amazon")]
public class AmazonProductController : ControllerBase
{
    private readonly IMediator _mediator;

    public AmazonProductController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Returns Amazon product list by search term
    /// </summary>
    /// <param name="searchTerm">Search term</param>
    /// <param name="page">Page, default input -> 1</param>
    /// <param name="cancellationToken">Cancellation Token</param>
    /// <returns></returns>
    [HttpGet("search/{searchTerm}")]
    public async Task<PagedList<Application.Models.ProductRating>> Search(
        [Required] string searchTerm,
        [FromQuery] int? page,
        CancellationToken cancellationToken)
    {
        var result = await _mediator
            .Send(new ProductsBySearchTerm.Command(searchTerm,page), 
                cancellationToken);
        return result;
    }

    /// <summary>
    /// Provides Amazon product Rating, Price ant other information by ASIN
    /// </summary>
    /// <param name="asins">List of ASINs to search for</param>
    /// <param name="cancellationToken">Cancellation Token</param>
    /// <returns></returns>
    [HttpGet("{asins}")]
    public async Task<Dictionary<string, ProductByAsin>> Asin(
        [Required] string asins, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(
            new GetProductByAsin.Command(asins), 
            cancellationToken);
        return result.Asins;
    }
}