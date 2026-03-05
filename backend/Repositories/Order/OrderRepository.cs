using backend.Models;
using Microsoft.EntityFrameworkCore;

namespace backend.Repositories.Order
{
    public class OrderRepository : IOrderRepository
    {
        private readonly CloneEbayDbContext _context;

        public OrderRepository(CloneEbayDbContext context)
        {
            _context = context;
        }

        public async Task<OrderTable> CreateAsync(OrderTable order)
        {
            _context.OrderTables.Add(order);
            await _context.SaveChangesAsync();

            // Load lại kèm Buyer + OrderItems + Product
            await _context.Entry(order).Reference(o => o.Buyer).LoadAsync();
            await _context.Entry(order).Collection(o => o.OrderItems).LoadAsync();
            foreach (var item in order.OrderItems)
            {
                await _context.Entry(item).Reference(i => i.Product).LoadAsync();
            }

            return order;
        }

        public async Task<OrderTable?> GetByIdAsync(int id)
        {
            return await _context.OrderTables
                .Include(o => o.Buyer)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                .FirstOrDefaultAsync(o => o.Id == id);
        }

        public async Task<List<OrderTable>> GetByBuyerIdAsync(int buyerId)
        {
            return await _context.OrderTables
                .Include(o => o.Buyer)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                .Where(o => o.BuyerId == buyerId)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();
        }

        public async Task<OrderTable> UpdateAsync(OrderTable order)
        {
            _context.OrderTables.Update(order);
            await _context.SaveChangesAsync();
            return order;
        }

        public async Task<Product?> GetProductByIdAsync(int productId)
        {
            return await _context.Products.FindAsync(productId);
        }
    }
}
