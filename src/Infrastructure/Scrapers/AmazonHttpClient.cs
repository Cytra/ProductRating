using Application.Ports;

namespace Infrastructure.Scrapers;

public class AmazonHttpClient : IAmazonHttpClient
{
    //https://www.amazon.com/gp/goldbox?ref_=nav_cs_gb

    private readonly HttpClient _client;
    public AmazonHttpClient(HttpClient client)
    {
        client.BaseAddress = new Uri("https://www.amazon.com");
        _client = client;
    }

    public async Task<string> GetHotDeals()
    {
        var response = await _client.GetAsync("/gp/goldbox?ref_=nav_cs_gb");
        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadAsStringAsync();
        return result;
    }

    public async Task<string> GetOneDeal()
    {
        //https://www.amazon.com/dp/B097VJS3Y3?ref_=Oct_DLandingS_D_dee22a8f_NA&th=1
        var response = await _client.GetAsync("/dp/B097VJS3Y3?ref_=Oct_DLandingS_D_dee22a8f_NA&th=1");
        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadAsStringAsync();
        return result;
    }
}