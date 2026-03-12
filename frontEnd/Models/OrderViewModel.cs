namespace frontEnd.Models
{
    public class OrderViewModel
    {
        public int OrderId { get; set; }
        public string? ProductTitle { get; set; }
        public string? ProductImage { get; set; }
        public decimal? ProductPrice { get; set; }
        public int? ProductId { get; set; }
        public int? SellerId { get; set; }
        public string? SellerName { get; set; }
        public int? BuyerId { get; set; }
        public string? BuyerName { get; set; }
        public DateTime? OrderDate { get; set; }
        public string? Status { get; set; }
        public decimal? TotalPrice { get; set; }
        public bool? IsCommented { get; set; }
    }
}
