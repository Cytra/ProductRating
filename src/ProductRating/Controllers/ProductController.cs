using System.ComponentModel.DataAnnotations;
using Application.Commands;
using Application.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ProductRating.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductController : ControllerBase
{
    private readonly IMediator _mediator;

    public ProductController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("{searchTerm}/search")]
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