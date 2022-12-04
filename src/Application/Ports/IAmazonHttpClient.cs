namespace Application.Ports;

public interface IAmazonHttpClient
{
    string SearchProducts(string searchTerm);
    string GetHtmlFromUrl(string urlEnd);
}