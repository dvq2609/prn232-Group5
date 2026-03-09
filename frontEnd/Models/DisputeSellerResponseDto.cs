namespace frontEnd.Models
{
    public enum DisputeActionType
    {
        AcceptReturn = 1,
        OfferPartial = 2,
        Decline = 3
    }

    public class DisputeSellerResponseDto
    {
        public DisputeActionType Action { get; set; }
        public decimal? AmountOffer { get; set; }
        public string? Message { get; set; }
    }
}
