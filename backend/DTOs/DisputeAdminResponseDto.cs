namespace backend.DTOs
{
    public enum DisputeAdminActionType
    {
        BuyerWin = 1,
        SellerWin = 2


    }

    public class DisputeAdminResponseDto
    {
        /// <summary>
        /// Hành động lựa chọn của Admin (1: Hoàn tiền cho Buyer, 2: Hoàn tiền cho Seller)
        /// </summary>
        public DisputeAdminActionType Action { get; set; }

        /// <summary>
        /// Lời biện minh hoặc tin nhắn gửi lại cho Seller
        /// </summary>
        public string? Message { get; set; }
    }
}
