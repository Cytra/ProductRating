using Infrastructure.Scrapers;
using Xunit;
using System.Net;
using FluentAssertions;

namespace UnitTests.IntegrationTests;

public class AmazonHttpClientTests
{
    [Fact]
    public async Task SearchProduct()
    {
        var httpClient = GetHttpClient();
        var sut = new AmazonHttpClient(httpClient);
        var products = await sut.SearchProducts("Dell Laptop", 1);
        products.Should().NotBeNull();
    }

    [Fact]
    public async Task GetProductByAsin()
    {
        var httpClient = GetHttpClient();
        var sut = new AmazonHttpClient(httpClient);
        var products = await sut.GetProductByAsin("B08S71Y7M7");
        products.Should().NotBeNull();
    }

    private HttpClient GetHttpClient()
    {
        var httpClient = new HttpClient(new HttpClientHandler
        {
            AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
        });
        return httpClient;

    }
}