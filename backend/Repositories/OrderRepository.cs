using backend.Models;
using Microsoft.EntityFrameworkCore;

namespace backend.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly CloneEbayDbContext _context;

        public OrderRepository(CloneEbayDbContext context)
        {
            _context = context;
        }

        public async Task<OrderTable> CreateOrderAsync(int buyerId, int productId, decimal unitPrice)
        {
            var order = new OrderTable
            {
                BuyerId = buyerId,
                OrderDate = DateTime.UtcNow,
                TotalPrice = unitPrice,
                Status = "delivered"
            };

            _context.OrderTables.Add(order);
            await _context.SaveChangesAsync();

            var orderItem = new OrderItem
            {
                OrderId = order.Id,
                ProductId = productId,
                Quantity = 1,
                UnitPrice = unitPrice
            };

            _context.OrderItems.Add(orderItem);
            await _context.SaveChangesAsync();

            return order;
        }
    }
}
