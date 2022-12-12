using Application.Models;
using System;
using Microsoft.Extensions.Caching.Memory;

namespace Application.Services;

public interface ICacheService
{
    void AddProductByAsin(string asin, ProductByAsin entry);
    ProductByAsin? GetProductByAsin(string asin);
}
public class CacheService : ICacheService
{
    private readonly IMemoryCache _memoryCache;
    private readonly MemoryCacheEntryOptions _cacheOptions;

    public CacheService(IMemoryCache memoryCache)
    {
        _memoryCache = memoryCache;
        _cacheOptions = new MemoryCacheEntryOptions()
        {
            Size = 1,
            SlidingExpiration = TimeSpan.FromMinutes(10),
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30),
        };
    }

    public void AddProductByAsin(string asin, ProductByAsin entry)
    {
        _memoryCache.Set(
            asin,
            entry,
            _cacheOptions);
    }

    public ProductByAsin? GetProductByAsin(string asin)
    {
        _memoryCache.TryGetValue(asin, out var entry);
        return (ProductByAsin?)entry;
    }
}