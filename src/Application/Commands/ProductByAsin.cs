using Application.Facades;
using Application.Models;
using Application.Models.Enums;
using MediatR;

namespace Application.Commands;

public static class GetProductByAsin
{
    public record Command(string Asins) : IRequest<Response>
    { }

    public class Response : ErrorResponse
    {
        public Dictionary<string, ProductByAsin> Asins { get; set; }
    }

    public class Handler : IRequestHandler<Command, Response>
    {
        private readonly IAmazonScrapperFacade _amazonScrapperFacade;
        public Handler(IAmazonScrapperFacade amazonScrapperFacade)
        {
            _amazonScrapperFacade = amazonScrapperFacade;
        }
        public async Task<Response> Handle(Command request, CancellationToken cancellationToken)
        {
            var asinArray = request.Asins.Split(',');

            if (asinArray.Length > 10)
            {
                return new Response()
                {
                    Errors = new List<Error>()
                    {
                        new ()
                        {
                            ErrorCode = ErrorCodes.BadRequest,
                            ErrorMessage = "Asin list limited to 10 items"
                        }
                    }
                };
            }

            var result = await _amazonScrapperFacade
                .GetProductByAsin(asinArray);

            return new Response()
            {
                Asins = result
            };
        }
    }
}