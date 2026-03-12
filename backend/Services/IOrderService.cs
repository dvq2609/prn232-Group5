using backend.DTOs;
using backend.Models;

namespace backend.Services
{
    public interface IOrderService
    {
        Task<OrderTable> BuyNowAsync(int buyerId, int productId, decimal unitPrice);
        Task<List<OrderDto>> GetOrdersByBuyerIdAsync(int buyerId);
        Task<List<OrderDto>> GetOrdersBySellerIdAsync(int sellerId);
    }
}
