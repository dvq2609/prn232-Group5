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
        public DateTime? SubmittedDate { get; set; }
        public DateTime? SolvedDate { get; set; }
        public string? Comment { get; set; }
        public string? SellerResponse { get; set; }
        public string? BuyerResponse { get; set; }
        public List<IFormFile>? Images { get; set; }
    }
    public class DisputeResponse
    {
        public int DisputeId { get; set; }
        public int? OrderId { get; set; }
        public string? Status { get; set; }
        public string? UserDispute { get; set; }
        public string? ProductTitle { get; set; }
        public string? Description { get; set; }
        public string? SellerName { get; set; }
        public string? Resolution { get; set; }
        public string? RaisedBy { get; set; }
        public DateTime? SubmittedDate { get; set; }
        public DateTime? SolvedDate { get; set; }
        public string? Comment { get; set; }
        public string? SellerResponse { get; set; }
        public string? BuyerResponse { get; set; }
        public List<ImageViewModel>? Images { get; set; }

    }
}