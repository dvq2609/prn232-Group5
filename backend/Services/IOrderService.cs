using backend.Models;

namespace backend.Services
{
    public interface IOrderService
    {
        Task<OrderTable> BuyNowAsync(int buyerId, int productId, decimal unitPrice);
    }
}
