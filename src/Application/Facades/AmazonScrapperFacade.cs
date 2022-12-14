using Application.Models;
using Application.Services;
using System.Collections.Concurrent;

namespace Application.Facades;

public interface IAmazonScrapperFacade
{
    Task<PagedList<ProductRating>> GetProductsBySearchTerm(string searchTerm, int? page);

    Task<Dictionary<string, ProductByAsin>> GetProductByAsin(string[] asins);
}
public class AmazonScrapperFacade : IAmazonScrapperFacade
{
    private readonly ICacheService _getProductByAsinCache;
    private readonly IAmazonScrapper _amazonScrapper;

    public AmazonScrapperFacade(
        ICacheService getProductByAsinCache, 
        IAmazonScrapper amazonScrapper)
    {
        _getProductByAsinCache = getProductByAsinCache;
        _amazonScrapper = amazonScrapper;
    }

    public Task<PagedList<ProductRating>> GetProductsBySearchTerm(string searchTerm, int? page)
    {
        return _amazonScrapper.GetProductsBySearchTerm(searchTerm, page);
    }

    public async Task<Dictionary<string, ProductByAsin>> GetProductByAsin(string[] asins)
    {
        var result = new ConcurrentDictionary<string, ProductByAsin>();

        var tasks = asins.Select(async asin =>
        {
            var product = _getProductByAsinCache.GetProductByAsin(asin);
            if (product != null)
            {
                result.TryAdd(asin, product);
            }
            else
            {
                product = await _amazonScrapper.GetProductByAsin(asin);
                if (product != null)
                {
                    _getProductByAsinCache.AddProductByAsin(asin, product);
                    result.TryAdd(asin, product);
                }
            }
        });
        await Task.WhenAll(tasks);

        return result.ToDictionary(kvp => kvp.Key,
            kvp => kvp.Value);
    }
}