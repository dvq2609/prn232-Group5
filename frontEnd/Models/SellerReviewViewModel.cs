using System.ComponentModel.DataAnnotations;

namespace frontEnd.Models
{
    public class CreateSellerReviewViewModel
    {
        public int OrderId { get; set; }
        public int BuyerId { get; set; }
        public string? BuyerName { get; set; }
        public string? ProductTitle { get; set; }
        public string? ProductImage { get; set; }

        [Required(ErrorMessage = "Please enter a comment")]
        public string? Comment { get; set; }
    }

    public class SellerReviewViewModel
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
}
