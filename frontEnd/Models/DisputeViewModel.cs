namespace frontEnd.Models
{
    public class DisputeViewModel
    {
        public int DisputeId { get; set; }
        public int? OrderId { get; set; }
        public string? Status { get; set; }
        public string? UserDispute { get; set; }
        public string? ProductTitle { get; set; }
        public string? Description { get; set; }
        public string? SellerName { get; set; }
    }
}