using Application.Ports;
using System.Text;
using System.Web;

namespace Infrastructure.Scrapers;

public class AmazonHttpClient : IAmazonHttpClient
{
    private readonly HttpClient _httpClient;
    private const string AmazonUrl = "https://www.amazon.com";

    public AmazonHttpClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<string> SearchProducts(string searchTerm, int page)
    {
        var url = $"https://www.amazon.com/s?k={HttpUtility.UrlEncode(searchTerm)}&page={page}";
        return await MakeRequest(url);
    }

    public async Task<string> GetProductByAsin(string asin)
    {
        var url = $"{AmazonUrl}/dp/{asin}";
        return await MakeRequest(url);
    }

    private async Task<string> MakeRequest(string url)
    {
        var response = await _httpClient.GetAsync(url);
        response.EnsureSuccessStatusCode();
        var resultBytes = await response.Content.ReadAsByteArrayAsync();
        var result  = Encoding.UTF8.GetString(resultBytes);
        return result;
    }
}