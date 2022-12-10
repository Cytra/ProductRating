namespace Application.Ports;

public interface IAmazonHttpClient
{
    Task<string> SearchProducts(string searchTerm, int page);
    Task<string> GetProductByAsin(string asin);
}