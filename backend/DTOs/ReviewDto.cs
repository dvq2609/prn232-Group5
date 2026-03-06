namespace backend.DTOs
{
    public class ReviewDto
    {
        public int Id { get; set; }
        public int? ProductId { get; set; }
        public int? ReviewerId { get; set; }
        public int Rating { get; set; }
        public string? Comment { get; set; }
        public string? Productname { get; set; }
        public string? ReviewerUsername { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}