using backend.Models;
using Microsoft.EntityFrameworkCore;

namespace backend.Repositories.SellerReview
{
    public class SellerReviewRepository : ISellerReviewRepository
    {
        private readonly CloneEbayDbContext _context;

        public SellerReviewRepository(CloneEbayDbContext context)
        {
            _context = context;
        }

        public async Task<SellerToBuyerReview> CreateAsync(SellerToBuyerReview review)
        {
            _context.SellerToBuyerReviews.Add(review);
            await _context.SaveChangesAsync();
            return review;
        }

        public async Task<List<SellerToBuyerReview>> GetBySellerIdAsync(int sellerId)
        {
            return await _context.SellerToBuyerReviews
                .Where(r => r.SellerId == sellerId)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();
        }

        public async Task<List<SellerToBuyerReview>> GetByBuyerIdAsync(int buyerId)
        {
            return await _context.SellerToBuyerReviews
                .Where(r => r.BuyerId == buyerId)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();
        }

        public async Task<SellerToBuyerReview?> GetByOrderIdAsync(int orderId)
        {
            return await _context.SellerToBuyerReviews
                .FirstOrDefaultAsync(r => r.OrderId == orderId);
        }

        public async Task<SellerToBuyerReview?> GetByIdAsync(int id)
        {
            return await _context.SellerToBuyerReviews.FindAsync(id);
        }

        public async Task UpdateAsync(SellerToBuyerReview review)
        {
            _context.SellerToBuyerReviews.Update(review);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(SellerToBuyerReview review)
        {
            _context.SellerToBuyerReviews.Remove(review);
            await _context.SaveChangesAsync();
        }
    }
}
