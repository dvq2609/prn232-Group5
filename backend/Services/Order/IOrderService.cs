using backend.DTOs.Order;

namespace backend.Services.Order
{
    public interface IOrderService
    {
        /// <summary>
        /// Quick-buy: Tạo order + auto chuyển status → "Delivered"
        /// Để user có thể review ngay
        /// </summary>
        Task<OrderDto> QuickBuyAsync(QuickBuyDto dto, int buyerId);

        /// <summary>
        /// Lấy order theo Id
        /// </summary>
        Task<OrderDto?> GetOrderByIdAsync(int id);

        /// <summary>
        /// Lấy tất cả orders của 1 buyer
        /// </summary>
        Task<List<OrderDto>> GetOrdersByBuyerIdAsync(int buyerId);
    }
}
