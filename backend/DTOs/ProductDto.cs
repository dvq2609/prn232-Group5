namespace backend.DTOs
{
    public class ProductDto
    {
        public int Id { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public decimal? Price { get; set; }
        public string? Image { get; set; }
        public int? CategoryId { get; set; }
        public int? SellerId { get; set; }
        public string? CategoryName { get; set; }
        public string? SellerName { get; set; }
        public List<ReviewDto> Reviews { get; set; } = new List<ReviewDto>();
        public double AverageRating { get; set; }
        public int ReviewCount { get; set; }

        public Dictionary<int, int> RatingCounts { get; set; } = new Dictionary<int, int>();
    }
}