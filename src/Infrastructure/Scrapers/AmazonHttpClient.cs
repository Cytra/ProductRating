using Application.Options;
using Application.Ports;
using Microsoft.Extensions.Options;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Remote;


namespace Infrastructure.Scrapers;

public class AmazonHttpClient : IAmazonHttpClient
{
    //https://www.amazon.com/gp/goldbox?ref_=nav_cs_gb

    private readonly AppOptions _appOptions;
    private readonly RemoteWebDriver _remoteWebDriver;
    public AmazonHttpClient(IOptions<AppOptions> appOptions, RemoteWebDriver remoteWebDriver)
    {
        _remoteWebDriver = remoteWebDriver;
        _appOptions = appOptions.Value;
    }

    public async Task<string> GetHotDeals()
    {
        //var response = await _client.GetAsync("/gp/goldbox?ref_=nav_cs_gb");
        //response.EnsureSuccessStatusCode();

        //var result = await response.Content.ReadAsStringAsync();
        //return result;
        return "";
    }

    public async Task<string> GetOneDeal()
    {

        //using var driver = GetChromeDriver();

        var url = $"https://www.amazon.com/s?k=mac&crid=2H5OIWBTQ2Q6Q&sprefix=ma%2Caps%2C161&ref=nb_sb_noss_2";

        _remoteWebDriver.Navigate().GoToUrl(url);
        return _remoteWebDriver.PageSource;
        ////https://www.amazon.com/dp/B097VJS3Y3?ref_=Oct_DLandingS_D_dee22a8f_NA&th=1
        //var response = await _client.GetAsync("/dp/B097VJS3Y3?ref_=Oct_DLandingS_D_dee22a8f_NA&th=1");
        //response.EnsureSuccessStatusCode();

        //var result = await response.Content.ReadAsStringAsync();
        //return result;
    }

    //private RemoteWebDriver GetChromeDriver()
    //{
    //    var chromeOptions = new ChromeOptions();
    //    return new RemoteWebDriver(new Uri(_appOptions.SeleniumUrl), chromeOptions);
    //}
}