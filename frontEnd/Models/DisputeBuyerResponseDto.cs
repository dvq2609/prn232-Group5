namespace frontEnd.Models
{
    public enum DisputeBuyerActionType
    {
        Accept = 1,
        Decline = 2
    }

    public class DisputeBuyerResponseDto
    {
        /// <summary>
        /// Hành động lựa chọn của Buyer (1: Chấp nhận, 2: Từ chối)
        /// </summary>
        public DisputeBuyerActionType Action { get; set; }

        /// <summary>
        /// Lời biện minh hoặc tin nhắn gửi lại cho Seller
        /// </summary>
        public string? Message { get; set; }
    }
}
