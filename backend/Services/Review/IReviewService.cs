using backend.DTOs;
using backend.DTOs.Review;
using ReviewModel = backend.Models.Review;

namespace backend.Services.Review
{
    public interface IReviewService
    {
        /// <summary>
        /// Tạo review mới — validate order completed + chưa review trước đó
        /// </summary>
        Task<ReviewDto> CreateReviewAsync(CreateReviewDto dto, int reviewerId);

        /// <summary>
        /// Lấy tất cả review của 1 product
        /// </summary>
        Task<List<ReviewDto>> GetReviewsByProductIdAsync(int productId);

        /// <summary>
        /// Lấy tất cả review mà 1 user đã viết
        /// </summary>
        Task<List<ReviewDto>> GetReviewsByReviewerIdAsync(int reviewerId);

        /// <summary>
        /// Lấy 1 review theo Id
        /// </summary>
        Task<ReviewDto?> GetReviewByIdAsync(int id);

        /// <summary>
        /// Cập nhật review (chỉ chủ review mới được sửa)
        /// </summary>
        Task<ReviewDto?> UpdateReviewAsync(int reviewId, CreateReviewDto dto, int reviewerId);

        /// <summary>
        /// Xóa review (chủ review hoặc admin)
        /// </summary>
        Task<bool> DeleteReviewAsync(int reviewId, int reviewerId, string role);

        /// <summary>
        /// Lấy thống kê rating của 1 product (average, count, breakdown)
        /// </summary>
        Task<ReviewSummaryDto> GetReviewSummaryAsync(int productId);
    }
}