using Application.Entities;

namespace Application.Ports;

public interface IProductRepository
{
    Task<bool> Insert(Product product);
    Task<Product?> Get(Guid id);
    Task<bool> Update(Product product);
    Task<bool> Delete(Guid id);
}