namespace backend.DTOs.Feedback
{
    public class SellerFeedbackProfileDto
    {
        public int SellerId { get; set; }
        public string? SellerName { get; set; }
        public double PositiveFeedbackPercent { get; set; }
        public int TotalFeedbacks { get; set; }
        public double AverageRating { get; set; }

        // Average detail scores
        public double AvgDeliveryOnTime { get; set; }
        public double AvgExactSame { get; set; }
        public double AvgCommunication { get; set; }

        public List<FeedbackDto> RecentFeedbacks { get; set; } = new();
    }
}
