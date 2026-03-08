using backend.DTOs;
using backend.DTOs.Review;
using ReviewModel = backend.Models.Review;

namespace backend.Services.Review
{
    public interface IReviewService
    {
        Task<ReviewDto> CreateReviewAsync(CreateReviewDto dto, int reviewerId);
        Task<List<ReviewDto>> GetReviewsByProductIdAsync(int productId);
        Task<List<ReviewDto>> GetReviewsByReviewerIdAsync(int reviewerId);
        Task<ReviewDto?> GetReviewByIdAsync(int id);
        Task<ReviewDto?> UpdateReviewAsync(int reviewId, CreateReviewDto dto, int reviewerId);
        Task<bool> DeleteReviewAsync(int reviewId, int reviewerId, string role);
        Task<ReviewSummaryDto> GetReviewSummaryAsync(int productId);
    }
}