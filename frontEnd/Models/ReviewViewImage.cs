namespace frontEnd.Models
{
    public class ReviewViewImage
    {
        public int Rating { get; set; }
        public string? Comment { get; set; }
        public string? ReviewerUsername { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
