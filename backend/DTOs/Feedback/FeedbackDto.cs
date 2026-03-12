namespace backend.DTOs.Feedback
{
    public class FeedbackDto
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public int SellerId { get; set; }
        public string? SellerName { get; set; }
        public string? BuyerName { get; set; }
        public string? Comment { get; set; }
        public decimal? AverageRating { get; set; }
        public int? ProductId { get; set; }
        public string? ProductTitle { get; set; }
        public string? ProductImage { get; set; }
        public DateTime? OrderDate { get; set; }

        // Detail feedback
        public int? DeliveryOnTime { get; set; }
        public int? ExactSame { get; set; }
        public int? Communication { get; set; }
    }
}
