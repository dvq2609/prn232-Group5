using System.ComponentModel.DataAnnotations;
using Sieve.Attributes;

namespace backend.DTOs
{
    public class DisputeDto
    {
        [Sieve(CanSort = true, CanFilter = true)]
        public int DisputeId { get; set; }
        public int? OrderId { get; set; }
        public string? Status { get; set; }
        public string? UserDispute { get; set; }
        public string? ProductTitle { get; set; }
        public string? Description { get; set; }
        public string? SellerName { get; set; }
        public string? RaisedBy { get; set; }
        public DateTime? SubmittedDate { get; set; }
        public DateTime? SolvedDate { get; set; }
        public string? Comment { get; set; }
        public string? Resolution { get; set; }
        public List<ImageDto>? Images { get; set; }
    }
    public class DisputeCreateDto
    {
        [Required]
        public int OrderId { get; set; }
        [Required]
        [MinLength(20)]
        public string? Description { get; set; }
        public List<IFormFile>? Images { get; set; }

    }
}
