using Application.Ports;
using OpenQA.Selenium.Remote;

namespace Infrastructure.Scrapers;

public class AmazonHttpClient : IAmazonHttpClient
{
    private const string AmazonUrl = "https://www.amazon.com";
    private readonly ISeleniumDriverFactory _driverFactory;
    private RemoteWebDriver _driver;
    public AmazonHttpClient(ISeleniumDriverFactory driverFactory)
    {
        _driverFactory = driverFactory;
    }

    public string SearchProducts(string searchTerm)
    {
        var formSearchTerm = searchTerm.Replace(' ', '+');
        var url = $"{AmazonUrl}/s?k={formSearchTerm}";

        _driver = _driverFactory.GetDriver();
        _driver.Navigate().GoToUrl(url);
        var result = _driver.PageSource;
        //_driver.Dispose();
        return result;
    }

    public string GetHtmlFromUrl(string urlEnd)
    {
        var url = $"{AmazonUrl}{urlEnd}";

        //var driver = _driverFactory.GetDriver();
        _driver.Navigate().GoToUrl(url);
        var result = _driver.PageSource;
        //driver.Dispose();
        return result;
    }
}