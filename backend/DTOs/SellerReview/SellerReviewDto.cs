namespace backend.DTOs.SellerReview
{
    public class SellerReviewDto
    {
        public int Id { get; set; }
        public int SellerId { get; set; }
        public string? SellerName { get; set; }
        public int BuyerId { get; set; }
        public string? BuyerName { get; set; }
        public string? Comment { get; set; }
        public DateTime CreatedAt { get; set; }
        public int OrderId { get; set; }
        public string? ProductName { get; set; }
        public string? ProductImage { get; set; }
    }

    public class UpdateSellerReviewDto
    {
        public string Comment { get; set; } = null!;
    }
}
