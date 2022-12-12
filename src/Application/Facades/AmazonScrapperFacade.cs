using Application.Models;
using Application.Services;

namespace Application.Facades;

public interface IAmazonScrapperFacade
{
    Task<PagedList<ProductRating>> GetProductsBySearchTerm(string searchTerm, int? page);

    Task<Dictionary<string, ProductByAsin>> GetProductByAsin(string asins);
}
public class AmazonScrapperFacade : IAmazonScrapperFacade
{
    private readonly ICacheService _cacheService;
    private readonly IAmazonScrapper _amazonScrapper;

    public AmazonScrapperFacade(
        ICacheService cacheService, 
        IAmazonScrapper amazonScrapper)
    {
        _cacheService = cacheService;
        _amazonScrapper = amazonScrapper;
    }

    public Task<PagedList<ProductRating>> GetProductsBySearchTerm(string searchTerm, int? page)
    {
        return _amazonScrapper.GetProductsBySearchTerm(searchTerm, page);
    }

    public async Task<Dictionary<string, ProductByAsin>> GetProductByAsin(string asins)
    {
        var asinArray = asins.Split(',');
        var result = new Dictionary<string, ProductByAsin>();
        //Parallel.ForEach(asinArray, async asin =>
        //{
        //    var product = _cacheService.GetProductByAsin(asin);
        //    if (product != null)
        //    {
        //        result.Add(asin, product);
        //    }
        //    else
        //    {
        //        product = await _amazonScrapper.GetProductByAsin(asin);
        //        if (product != null)
        //        {
        //            _cacheService.AddProductByAsin(asin, product);
        //            result.Add(asin, product);
        //        }
        //    }
        //});

        foreach (var asin in asinArray)
        {
            var product = _cacheService.GetProductByAsin(asin);
            if (product != null)
            {
                result.Add(asin, product);
                continue;
            }

            product = await _amazonScrapper.GetProductByAsin(asin);
            if (product != null)
            {
                _cacheService.AddProductByAsin(asin, product);
                result.Add(asin, product);
            }
        }
        return result;
    }
}