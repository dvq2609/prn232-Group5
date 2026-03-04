using System.ComponentModel.DataAnnotations;

namespace backend.DTOs
{
    public class DisputeDto
    {
        public int DisputeId { get; set; }
        public int? OrderId { get; set; }
        public string? Status { get; set; }
        public string? UserDispute { get; set; }
        public string? ProductTitle { get; set; }
        public string? Description { get; set; }
        public string? SellerName { get; set; }
    }
    public class DisputeCreateDto
    {
        [Required]
        public int OrderId { get; set; }
        [Required]
        [MinLength(20)]
        public string? Description { get; set; }
    }
}
