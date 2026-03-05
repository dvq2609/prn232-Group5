namespace backend.DTOs.Review
{
    /// <summary>
    /// Thống kê rating tổng hợp của 1 product
    /// </summary>
    public class ReviewSummaryDto
    {
        public int ProductId { get; set; }
        public double AverageRating { get; set; }
        public int TotalReviews { get; set; }

        /// <summary>
        /// Breakdown: key = số sao (1-5), value = số lượng review
        /// VD: { 5: 10, 4: 5, 3: 2, 2: 1, 1: 0 }
        /// </summary>
        public Dictionary<int, int> RatingBreakdown { get; set; } = new Dictionary<int, int>();
    }
}
