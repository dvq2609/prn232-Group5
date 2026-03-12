using backend.DTOs.SellerReview;

namespace backend.Services.SellerReview
{
    public interface ISellerReviewService
    {
        Task<SellerReviewDto> LeaveSellerReviewAsync(CreateSellerReviewDto dto, int sellerId);
        Task<List<SellerReviewDto>> GetBySellerIdAsync(int sellerId);
        Task<List<SellerReviewDto>> GetByBuyerIdAsync(int buyerId);
        Task<SellerReviewDto> UpdateSellerReviewAsync(int reviewId, string comment, int sellerId);
        Task DeleteSellerReviewAsync(int reviewId, int sellerId);
    }
}
