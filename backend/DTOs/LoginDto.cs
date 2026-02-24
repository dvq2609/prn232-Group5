using System.ComponentModel.DataAnnotations;

namespace backend.DTOs
{

    public class LoginDto
    {
        [Required]
        [EmailAddress]
        public string? Email { get; set; }
        [Required]
        public string? Password { get; set; }
    }
    public class LoginResponseDto
    {
        public string? Token { get; set; }
        public string? Email { get; set; }
        public string? Role { get; set; }
        public int AccountId { get; set; }
    }
}