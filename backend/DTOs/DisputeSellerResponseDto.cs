namespace backend.DTOs
{
    public enum DisputeActionType
    {
        AcceptReturn = 1,
        OfferPartial = 2,
        Decline = 3
    }

    public class DisputeSellerResponseDto
    {
        /// <summary>
        /// Hành động lựa chọn của Seller (1: Chấp nhận, 2: Đền bù 1 phần, 3: Từ chối)
        /// </summary>
        public DisputeActionType Action { get; set; }

        /// <summary>
        /// Số tiền đền bù (chỉ áp dụng khi Action = OfferPartial)
        /// </summary>
        public decimal? AmountOffer { get; set; }

        /// <summary>
        /// Lời biện minh hoặc tin nhắn gửi lại cho Buyer
        /// </summary>
        public string? Message { get; set; }
    }
}
