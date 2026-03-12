using backend.DTOs.Feedback;

namespace backend.Services.Feedback
{
    public interface IFeedbackService
    {
        Task<FeedbackDto> LeaveFeedbackAsync(CreateFeedbackDto dto, int buyerId);
        Task<FeedbackDto> UpdateFeedbackAsync(CreateFeedbackDto dto, int buyerId);
        Task DeleteFeedbackAsync(int orderId, int buyerId);
        Task<List<FeedbackDto>> GetFeedbacksBySellerIdAsync(int sellerId);
        Task<List<FeedbackDto>> GetFeedbacksByBuyerIdAsync(int buyerId);
        Task<SellerFeedbackProfileDto> GetSellerProfileAsync(int sellerId);
    }
}
