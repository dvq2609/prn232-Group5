using backend.Models;

namespace backend.Services
{
    public interface IProductService
    {
        Task<List<Product>> GetAllAsync();
        Task<Product> GetByIdAsync(int id);
        Task<List<Product>> GetByBuyerIdAsync(int buyerId);

    }
}