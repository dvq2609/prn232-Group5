using System.ComponentModel.DataAnnotations;

namespace frontEnd.Models
{
    public class CreateFeedbackViewModel
    {
        public int OrderId { get; set; }
        public string? ProductTitle { get; set; }
        public string? ProductImage { get; set; }
        public string? SellerName { get; set; }

        [Required(ErrorMessage = "Please select a rating")]
        [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5")]
        public int Rating { get; set; }

        [Range(1, 5)]
        public int DeliveryOnTime { get; set; } = 3;

        [Range(1, 5)]
        public int ExactSame { get; set; } = 3;

        [Range(1, 5)]
        public int Communication { get; set; } = 3;

        public string? Comment { get; set; }
    }

    public class FeedbackViewModel
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
        public int? DeliveryOnTime { get; set; }
        public int? ExactSame { get; set; }
        public int? Communication { get; set; }
    }

    public class SellerFeedbackProfileViewModel
    {
        public int SellerId { get; set; }
        public string? SellerName { get; set; }
        public double PositiveFeedbackPercent { get; set; }
        public int TotalFeedbacks { get; set; }
        public double AverageRating { get; set; }
        public double AvgDeliveryOnTime { get; set; }
        public double AvgExactSame { get; set; }
        public double AvgCommunication { get; set; }
        public List<FeedbackViewModel> RecentFeedbacks { get; set; } = new();
    }
}
