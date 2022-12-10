using System.Web;
using Application.Ports;
using OpenQA.Selenium.Remote;

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

    public Task<string> SearchProducts(string searchTerm, int page)
    {
        var formSearchTerm = HttpUtility.UrlEncode(searchTerm);
        var url = $"{AmazonUrl}/s?k={formSearchTerm}&page={page}";
        _driver = _driverFactory.GetDriver();
        _driver.Navigate().GoToUrl(url);
        var result = _driver.PageSource;
        return Task.FromResult(result);
    }

    public Task<string> GetProductByAsin(string asin)
    {
        throw new NotImplementedException();
    }
}