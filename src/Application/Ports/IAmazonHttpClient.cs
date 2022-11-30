namespace Application.Ports;

public interface IAmazonHttpClient
{
    Task<string> GetHotDeals();

    Task<string> GetOneDeal();
}