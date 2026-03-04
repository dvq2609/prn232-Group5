namespace frontEnd.Models
{
    public class OrderViewModel
    {
        public int OrderId { get; set; }
        public string? ProductTitle { get; set; }
        public string? SellerName { get; set; }
        public DateTime? OrderDate { get; set; }
        public string? Status { get; set; }
    }
}
