using backend.DTOs;
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
        public async Task<List<OrderDto>> GetOrdersByBuyerIdAsync(int buyerId)
        {
            return await _context.OrderTables
                .Where(o => o.BuyerId == buyerId
                         && (o.Status == "Delivered" || o.Status == "delivered"))
                .Where(o => !_context.Disputes.Any(d => d.OrderId == o.Id))
                .Select(o => new OrderDto
                {
                    OrderId = o.Id,
                    OrderDate = o.OrderDate,
                    Status = o.Status,
                    ProductTitle = o.OrderItems
                        .Where(oi => oi.Product != null)
                        .Select(oi => oi.Product!.Title)
                        .FirstOrDefault(),
                    SellerName = o.OrderItems
                        .Where(oi => oi.Product != null && oi.Product.Seller != null)
                        .Select(oi => oi.Product!.Seller!.Username)
                        .FirstOrDefault()
                })
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();
        }

    }
}
