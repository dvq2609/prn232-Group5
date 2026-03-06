using ReviewModel = backend.Models.Review;

namespace backend.Repositories.Review
{
    public interface IReviewRepository
    {
        Task<List<ReviewModel>> GetByProductIdAsync(int productId);
        Task<List<ReviewModel>> GetByReviewerIdAsync(int reviewerId);
        Task<ReviewModel?> GetByIdAsync(int id);
        Task<ReviewModel?> GetByReviewerAndProductAsync(int reviewerId, int productId);
        Task<bool> HasCompletedOrderAsync(int reviewerId, int productId);
        Task<ReviewModel> CreateAsync(ReviewModel review);
        Task<ReviewModel> UpdateAsync(ReviewModel review);
        Task<bool> DeleteAsync(int id);
    }
}