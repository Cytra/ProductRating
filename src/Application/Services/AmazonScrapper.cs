using Application.Ports;

namespace Application.Services;

public interface IAmazonScrapper
{

}

public class AmazonScrapper : IAmazonScrapper
{
    private readonly IAmazonHttpClient _amazonHttpClient;
    public AmazonScrapper(IAmazonHttpClient amazonHttpClient)
    {
        _amazonHttpClient = amazonHttpClient;
    }

    public async Task GetHotDeals()
    {
        var oneDeal = await _amazonHttpClient.GetOneDeal();

    }
}