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

        // Lấy tất cả review của 1 product (kèm thông tin người review)
        public async Task<List<ReviewModel>> GetByProductIdAsync(int productId)
        {
            return await _context.Reviews
                .Include(r => r.Reviewer)
                .Include(r => r.Product)
                .Where(r => r.ProductId == productId)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();
        }

        // Lấy tất cả review mà 1 user đã viết
        public async Task<List<ReviewModel>> GetByReviewerIdAsync(int reviewerId)
        {
            return await _context.Reviews
                .Include(r => r.Product)
                .Include(r => r.Reviewer)
                .Where(r => r.ReviewerId == reviewerId)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();
        }

        // Lấy 1 review theo Id
        public async Task<ReviewModel?> GetByIdAsync(int id)
        {
            return await _context.Reviews
                .Include(r => r.Reviewer)
                .Include(r => r.Product)
                .FirstOrDefaultAsync(r => r.Id == id);
        }

        // Kiểm tra user đã review product này chưa (1 review/product/user)
        public async Task<ReviewModel?> GetByReviewerAndProductAsync(int reviewerId, int productId)
        {
            return await _context.Reviews
                .FirstOrDefaultAsync(r => r.ReviewerId == reviewerId && r.ProductId == productId);
        }

        // Kiểm tra user đã mua và nhận hàng chưa (order status = "Delivered"/"delivered"/"Completed")
        public async Task<bool> HasCompletedOrderAsync(int reviewerId, int productId)
        {
            return await _context.OrderTables
                .AnyAsync(o => o.BuyerId == reviewerId
                    && (o.Status == "Delivered" || o.Status == "delivered" || o.Status == "Completed")
                    && o.OrderItems.Any(oi => oi.ProductId == productId));
        }

        // Tạo review mới
        public async Task<ReviewModel> CreateAsync(ReviewModel review)
        {
            _context.Reviews.Add(review);
            await _context.SaveChangesAsync();

            // Load lại Reviewer + Product để AutoMapper có thể map ReviewerUsername + Productname
            await _context.Entry(review).Reference(r => r.Reviewer).LoadAsync();
            await _context.Entry(review).Reference(r => r.Product).LoadAsync();

            return review;
        }

        // Cập nhật review (chỉ sửa rating + comment)
        public async Task<ReviewModel> UpdateAsync(ReviewModel review)
        {
            _context.Reviews.Update(review);
            await _context.SaveChangesAsync();

            await _context.Entry(review).Reference(r => r.Reviewer).LoadAsync();
            await _context.Entry(review).Reference(r => r.Product).LoadAsync();

            return review;
        }

        // Xóa review
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