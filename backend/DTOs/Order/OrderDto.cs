namespace backend.DTOs.Order
{
    /// <summary>
    /// Input: Client gửi lên khi quick-buy
    /// </summary>
    public class QuickBuyDto
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; } = 1;
    }

    /// <summary>
    /// Output: Trả về cho client sau khi tạo/xem order
    /// </summary>
    public class OrderDto
    {
        public int Id { get; set; }
        public int? BuyerId { get; set; }
        public string? BuyerName { get; set; }
        public DateTime? OrderDate { get; set; }
        public decimal? TotalPrice { get; set; }
        public string? Status { get; set; }
        public List<OrderItemDto> OrderItems { get; set; } = new();
    }

    /// <summary>
    /// Output: Thông tin từng item trong order
    /// </summary>
    public class OrderItemDto
    {
        public int Id { get; set; }
        public int? ProductId { get; set; }
        public string? ProductName { get; set; }
        public int? Quantity { get; set; }
        public decimal? UnitPrice { get; set; }
    }
}