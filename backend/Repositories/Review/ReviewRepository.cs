using backend.Models;
using Microsoft.EntityFrameworkCore;
using ReviewModel = backend.Models.Review;

namespace backend.Repositories.Review
{
    public class ReviewRepository : IReviewRepository
    {
        private readonly CloneEbayDbContext _context;

        public ReviewRepository(CloneEbayDbContext context)
        {
            _context = context;
        }

        public async Task<List<ReviewModel>> GetByProductIdAsync(int productId)
        {
            return await _context.Reviews
                .Include(r => r.Reviewer)
                .Include(r => r.Product)
                .Where(r => r.ProductId == productId)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();
        }

        public async Task<List<ReviewModel>> GetByReviewerIdAsync(int reviewerId)
        {
            return await _context.Reviews
                .Include(r => r.Product)
                .Include(r => r.Reviewer)
                .Where(r => r.ReviewerId == reviewerId)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();
        }

        public async Task<ReviewModel?> GetByIdAsync(int id)
        {
            return await _context.Reviews
                .Include(r => r.Reviewer)
                .Include(r => r.Product)
                .FirstOrDefaultAsync(r => r.Id == id);
        }

        public async Task<ReviewModel?> GetByReviewerAndProductAsync(int reviewerId, int productId)
        {
            return await _context.Reviews
                .FirstOrDefaultAsync(r => r.ReviewerId == reviewerId && r.ProductId == productId);
        }

        public async Task<bool> HasCompletedOrderAsync(int reviewerId, int productId)
        {
            return await _context.OrderTables
                .AnyAsync(o => o.BuyerId == reviewerId
                    && (o.Status == "Delivered" || o.Status == "delivered" || o.Status == "Completed")
                    && o.OrderItems.Any(oi => oi.ProductId == productId));
        }

        public async Task<ReviewModel> CreateAsync(ReviewModel review)
        {
            _context.Reviews.Add(review);
            await _context.SaveChangesAsync();

            await _context.Entry(review).Reference(r => r.Reviewer).LoadAsync();
            await _context.Entry(review).Reference(r => r.Product).LoadAsync();

            return review;
        }

        public async Task<ReviewModel> UpdateAsync(ReviewModel review)
        {
            _context.Reviews.Update(review);
            await _context.SaveChangesAsync();

            await _context.Entry(review).Reference(r => r.Reviewer).LoadAsync();
            await _context.Entry(review).Reference(r => r.Product).LoadAsync();

            return review;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var review = await _context.Reviews.FindAsync(id);
            if (review == null) return false;

            _context.Reviews.Remove(review);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}