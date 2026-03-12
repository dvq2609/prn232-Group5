using backend.DTOs.Feedback;
using backend.Models;

namespace backend.Repositories.Feedback
{
    public interface IFeedbackRepository
    {
        Task<Models.Feedback> CreateFeedbackAsync(Models.Feedback feedback, DetailFeedback detail);
        Task<Models.Feedback> UpdateFeedbackAsync(Models.Feedback feedback, DetailFeedback detail);
        Task<Models.Feedback?> GetByOrderIdAsync(int orderId);
        Task<List<Models.Feedback>> GetBySellerId(int sellerId);
        Task<List<Models.Feedback>> GetByBuyerOrdersAsync(int buyerId);
        Task<int> GetSellerIdByOrderId(int orderId);
        Task<int> GetProductIdByOrderId(int orderId);
        Task<bool> OrderBelongsToBuyer(int orderId, int buyerId);
        Task<bool> IsOrderDelivered(int orderId);
        Task SetOrderCommented(int orderId);
        Task SetOrderUncommented(int orderId);
        Task DeleteFeedbackAsync(int orderId);
    }
}
