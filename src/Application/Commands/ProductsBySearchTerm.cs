using Application.Facades;
using Application.Models;
using MediatR;

namespace Application.Commands;

public static class ProductsBySearchTerm
{
    public record Command(
        string SearchTerm,
        int? Page) : IRequest<PagedList<ProductRating>>
    {}

    public class Handler : IRequestHandler<Command, PagedList<ProductRating>>
    {
        private readonly IAmazonScrapperFacade _amazonScrapperFacade;
        public Handler(IAmazonScrapperFacade amazonScrapperFacade)
        {
            _amazonScrapperFacade = amazonScrapperFacade;
        }
        public Task<PagedList<ProductRating>> Handle(Command request, CancellationToken cancellationToken)
        {
            return _amazonScrapperFacade.GetProductsBySearchTerm(request.SearchTerm, request.Page);
        }
    }
}