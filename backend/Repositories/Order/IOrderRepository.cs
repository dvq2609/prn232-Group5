using backend.Models;

namespace backend.Repositories.Order
{
    public interface IOrderRepository
    {
        /// <summary>
        /// Tạo order mới (kèm OrderItem)
        /// </summary>
        Task<OrderTable> CreateAsync(OrderTable order);

        /// <summary>
        /// Lấy order theo Id (kèm OrderItems + Product + Buyer)
        /// </summary>
        Task<OrderTable?> GetByIdAsync(int id);

        /// <summary>
        /// Lấy tất cả orders của 1 buyer
        /// </summary>
        Task<List<OrderTable>> GetByBuyerIdAsync(int buyerId);

        /// <summary>
        /// Cập nhật status của order
        /// </summary>
        Task<OrderTable> UpdateAsync(OrderTable order);

        /// <summary>
        /// Lấy product theo id (để lấy giá)
        /// </summary>
        Task<Product?> GetProductByIdAsync(int productId);
    }
}
