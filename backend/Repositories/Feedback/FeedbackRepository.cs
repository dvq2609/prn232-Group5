using backend.Models;
using Microsoft.EntityFrameworkCore;

namespace backend.Repositories.Feedback
{
    public class FeedbackRepository : IFeedbackRepository
    {
        private readonly CloneEbayDbContext _context;

        public FeedbackRepository(CloneEbayDbContext context)
        {
            _context = context;
        }

        public async Task<Models.Feedback> CreateFeedbackAsync(Models.Feedback feedback, DetailFeedback detail)
        {
            _context.Feedbacks.Add(feedback);
            await _context.SaveChangesAsync();

            detail.FeedbackId = feedback.Id;
            _context.DetailFeedbacks.Add(detail);
            await _context.SaveChangesAsync();

            // Reload with relations
            await _context.Entry(feedback).Reference(f => f.Seller).LoadAsync();
            await _context.Entry(feedback).Reference(f => f.Orders).LoadAsync();
            await _context.Entry(feedback).Collection(f => f.DetailFeedbacks).LoadAsync();

            return feedback;
        }

        public async Task<Models.Feedback> UpdateFeedbackAsync(Models.Feedback feedback, DetailFeedback detail)
        {
            _context.Feedbacks.Update(feedback);
            _context.DetailFeedbacks.Update(detail);
            await _context.SaveChangesAsync();

            await _context.Entry(feedback).Reference(f => f.Seller).LoadAsync();
            await _context.Entry(feedback).Reference(f => f.Orders).LoadAsync();
            await _context.Entry(feedback).Collection(f => f.DetailFeedbacks).LoadAsync();

            return feedback;
        }

        public async Task<Models.Feedback?> GetByOrderIdAsync(int orderId)
        {
            return await _context.Feedbacks
                .Include(f => f.DetailFeedbacks)
                .FirstOrDefaultAsync(f => f.OrdersId == orderId);
        }

        public async Task<List<Models.Feedback>> GetBySellerId(int sellerId)
        {
            return await _context.Feedbacks
                .Include(f => f.Seller)
                .Include(f => f.Orders)
                    .ThenInclude(o => o!.Buyer)
                .Include(f => f.Orders)
                    .ThenInclude(o => o!.OrderItems)
                        .ThenInclude(oi => oi.Product)
                .Include(f => f.DetailFeedbacks)
                .Where(f => f.SellerId == sellerId)
                .OrderByDescending(f => f.Id)
                .ToListAsync();
        }

        public async Task<List<Models.Feedback>> GetByBuyerOrdersAsync(int buyerId)
        {
            return await _context.Feedbacks
                .Include(f => f.Seller)
                .Include(f => f.Orders)
                    .ThenInclude(o => o!.Buyer)
                .Include(f => f.Orders)
                    .ThenInclude(o => o!.OrderItems)
                        .ThenInclude(oi => oi.Product)
                .Include(f => f.DetailFeedbacks)
                .Where(f => f.Orders != null && f.Orders.BuyerId == buyerId)
                .OrderByDescending(f => f.Id)
                .ToListAsync();
        }

        public async Task<int> GetSellerIdByOrderId(int orderId)
        {
            var sellerId = await _context.OrderItems
                .Where(oi => oi.OrderId == orderId && oi.Product != null)
                .Select(oi => oi.Product!.SellerId)
                .FirstOrDefaultAsync();

            return sellerId ?? 0;
        }

        public async Task<int> GetProductIdByOrderId(int orderId)
        {
            var productId = await _context.OrderItems
                .Where(oi => oi.OrderId == orderId && oi.ProductId != null)
                .Select(oi => oi.ProductId!.Value)
                .FirstOrDefaultAsync();

            return productId;
        }

        public async Task<bool> OrderBelongsToBuyer(int orderId, int buyerId)
        {
            return await _context.OrderTables
                .AnyAsync(o => o.Id == orderId && o.BuyerId == buyerId);
        }

        public async Task<bool> IsOrderDelivered(int orderId)
        {
            return await _context.OrderTables
                .AnyAsync(o => o.Id == orderId
                    && (o.Status == "Delivered" || o.Status == "delivered" || o.Status == "Completed"));
        }

        public async Task SetOrderCommented(int orderId)
        {
            var order = await _context.OrderTables.FindAsync(orderId);
            if (order != null)
            {
                order.IsCommented = true;
                await _context.SaveChangesAsync();
            }
        }

        public async Task SetOrderUncommented(int orderId)
        {
            var order = await _context.OrderTables.FindAsync(orderId);
            if (order != null)
            {
                order.IsCommented = false;
                await _context.SaveChangesAsync();
            }
        }

        public async Task DeleteFeedbackAsync(int orderId)
        {
            var feedback = await _context.Feedbacks
                .Include(f => f.DetailFeedbacks)
                .FirstOrDefaultAsync(f => f.OrdersId == orderId);
            if (feedback != null)
            {
                _context.DetailFeedbacks.RemoveRange(feedback.DetailFeedbacks);
                _context.Feedbacks.Remove(feedback);
                await _context.SaveChangesAsync();
            }
        }
    }
}
