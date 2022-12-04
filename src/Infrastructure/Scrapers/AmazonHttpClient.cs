using Application.Ports;
using Microsoft.Extensions.Options;
using OpenQA.Selenium.Remote;
using System.Text.Json;

namespace Infrastructure.Scrapers;

public class SeleniumClient : IAmazonHttpClient
{
    private const string AmazonUrl = "https://www.amazon.com";
    private readonly ISeleniumDriverFactory _driverFactory;
    private RemoteWebDriver _driver;
    public SeleniumClient(ISeleniumDriverFactory driverFactory)
    {
        _driverFactory = driverFactory;
    }

    public Task<string> SearchProducts(string searchTerm)
    {
        var formSearchTerm = searchTerm.Replace(' ', '+');
        var url = $"{AmazonUrl}/s?k={formSearchTerm}";

        _driver = _driverFactory.GetDriver();
        _driver.Navigate().GoToUrl(url);
        var result = _driver.PageSource;
        //_driver.Dispose();
        return Task.FromResult(result);
    }

    public Task<string> GetHtmlFromUrl(string urlEnd)
    {
        var url = $"{AmazonUrl}{urlEnd}";

        //var driver = _driverFactory.GetDriver();
        _driver.Navigate().GoToUrl(url);
        var result = _driver.PageSource;
        //driver.Dispose();
        return Task.FromResult(result);
    }
}

public class AmazonHttpClient : IAmazonHttpClient
{
    private readonly HttpClient _httpClient;
    private const string AmazonUrl = "https://www.amazon.com";
    private readonly JsonSerializerOptions _jsonOptions = new(JsonSerializerDefaults.Web);

    public AmazonHttpClient(HttpClient httpClient)
    {

        //httpClient.BaseAddress = new Uri(AmazonUrl);
        httpClient.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/78.0.3904.108 Safari/537.36");
        httpClient.DefaultRequestHeaders.Add("Accept-Encoding", "gzip, deflate");
        httpClient.DefaultRequestHeaders.Add("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8");
        httpClient.DefaultRequestHeaders.Add("DNT", "1");
        httpClient.DefaultRequestHeaders.Add("Connection", "close");
        httpClient.DefaultRequestHeaders.Add("Upgrade-Insecure-Requests", "1");
        _httpClient = httpClient;
    }

    public async Task<string> SearchProducts(string searchTerm)
    {
        var formSearchTerm = searchTerm.Replace(' ', '+');
        var url = $"{AmazonUrl}/s?k={formSearchTerm}";
        var response = await _httpClient.GetAsync(url);
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadAsStringAsync();
        return result;
    }

    public Task<string> GetHtmlFromUrl(string urlEnd)
    {
        //var url = $"{AmazonUrl}{urlEnd}";
        //var request = new H
        throw new NotImplementedException();
    }
}