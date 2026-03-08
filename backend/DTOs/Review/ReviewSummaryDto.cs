namespace backend.DTOs.Review
{

    public class ReviewSummaryDto
    {
        public int ProductId { get; set; }
        public double AverageRating { get; set; }
        public int TotalReviews { get; set; }


        public Dictionary<int, int> RatingBreakdown { get; set; } = new Dictionary<int, int>();
    }
}
