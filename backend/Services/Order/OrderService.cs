using backend.DTOs.Order;
using backend.Models;
using backend.Repositories.Order;

namespace backend.Services.Order
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;

        public OrderService(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        /// <summary>
        /// Quick-buy flow:
        /// 1. Kiểm tra product tồn tại
        /// 2. Tạo OrderTable + OrderItem
        /// 3. Auto chuyển status → "Delivered" (skip các bước trung gian)
        /// → User có thể review ngay sau bước này
        /// </summary>
        public async Task<OrderDto> QuickBuyAsync(QuickBuyDto dto, int buyerId)
        {
            // 1. Kiểm tra product
            var product = await _orderRepository.GetProductByIdAsync(dto.ProductId);
            if (product == null)
                throw new KeyNotFoundException("Product not found.");

            if (dto.Quantity < 1)
                throw new ArgumentException("Quantity must be at least 1.");

            // 2. Tạo OrderTable
            var order = new OrderTable
            {
                BuyerId = buyerId,
                OrderDate = DateTime.UtcNow,
                TotalPrice = product.Price * dto.Quantity,
                Status = "Delivered", // ← Auto skip tới Delivered để có thể review ngay
                OrderItems = new List<OrderItem>
                {
                    new OrderItem
                    {
                        ProductId = dto.ProductId,
                        Quantity = dto.Quantity,
                        UnitPrice = product.Price
                    }
                }
            };

            // 3. Lưu vào DB
            var createdOrder = await _orderRepository.CreateAsync(order);

            // 4. Map sang DTO trả về
            return MapToDto(createdOrder);
        }

        public async Task<OrderDto?> GetOrderByIdAsync(int id)
        {
            var order = await _orderRepository.GetByIdAsync(id);
            if (order == null) return null;
            return MapToDto(order);
        }

        public async Task<List<OrderDto>> GetOrdersByBuyerIdAsync(int buyerId)
        {
            var orders = await _orderRepository.GetByBuyerIdAsync(buyerId);
            return orders.Select(MapToDto).ToList();
        }

        /// <summary>
        /// Map OrderTable → OrderDto (thủ công, không cần AutoMapper cho phần đơn giản này)
        /// </summary>
        private OrderDto MapToDto(OrderTable order)
        {
            return new OrderDto
            {
                Id = order.Id,
                BuyerId = order.BuyerId,
                BuyerName = order.Buyer?.Username,
                OrderDate = order.OrderDate,
                TotalPrice = order.TotalPrice,
                Status = order.Status,
                OrderItems = order.OrderItems.Select(oi => new OrderItemDto
                {
                    Id = oi.Id,
                    ProductId = oi.ProductId,
                    ProductName = oi.Product?.Title,
                    Quantity = oi.Quantity,
                    UnitPrice = oi.UnitPrice
                }).ToList()
            };
        }
    }
}
