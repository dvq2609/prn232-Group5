using backend.Models;
using backend.Repositories;

namespace backend.Services
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;

        public OrderService(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        public async Task<OrderTable> BuyNowAsync(int buyerId, int productId, decimal unitPrice)
        {
            return await _orderRepository.CreateOrderAsync(buyerId, productId, unitPrice);
        }
    }
}
