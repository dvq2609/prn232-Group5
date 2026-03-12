using backend.Models;

namespace backend.Repositories.SellerReview
{
    public interface ISellerReviewRepository
    {
        Task<SellerToBuyerReview> CreateAsync(SellerToBuyerReview review);
        Task<List<SellerToBuyerReview>> GetBySellerIdAsync(int sellerId);
        Task<List<SellerToBuyerReview>> GetByBuyerIdAsync(int buyerId);
        Task<SellerToBuyerReview?> GetByOrderIdAsync(int orderId);
        Task<SellerToBuyerReview?> GetByIdAsync(int id);
        Task UpdateAsync(SellerToBuyerReview review);
        Task DeleteAsync(SellerToBuyerReview review);
    }
}
