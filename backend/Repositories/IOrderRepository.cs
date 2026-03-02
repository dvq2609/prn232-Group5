using backend.Models;

namespace backend.Repositories
{
    public interface IOrderRepository
    {
        Task<OrderTable> CreateOrderAsync(int buyerId, int productId, decimal unitPrice);
    }
}
