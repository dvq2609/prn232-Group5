namespace backend.DTOs.Feedback
{
    public class CreateFeedbackDto
    {
        public int OrderId { get; set; }
        public string? Comment { get; set; }
        public int Rating { get; set; } // 1-5

        // Detail feedback (1-5 each)
        public int DeliveryOnTime { get; set; }
        public int ExactSame { get; set; }
        public int Communication { get; set; }
    }
}
