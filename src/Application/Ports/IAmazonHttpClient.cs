namespace Application.Ports;

public interface IAmazonHttpClient
{
    Task<string> SearchProducts(string searchTerm);
    Task<string> GetHtmlFromUrl(string urlEnd);
}