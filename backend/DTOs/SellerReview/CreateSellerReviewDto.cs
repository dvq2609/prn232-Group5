namespace backend.DTOs.SellerReview
{
    public class CreateSellerReviewDto
    {
        public int OrderId { get; set; }
        public int BuyerId { get; set; }
        public string? Comment { get; set; }
    }
}
