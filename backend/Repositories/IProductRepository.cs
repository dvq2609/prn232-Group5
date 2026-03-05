using backend.Models;

namespace backend.Repositories
{
    public interface IProductRepository
    {
        Task<List<Product>> GetAllAsync();
        Task<Product> GetByIdAsync(int id);
        Task<List<Product>> GetByBuyerIdAsync(int buyerId);

    }
}