using System.ComponentModel.DataAnnotations;

namespace frontEnd.Models
{
    public class LoginViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = null!;

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; } = null!;

    }
    public class LoginResponseViewModel
    {
        public string? Token { get; set; }
        public string? Email { get; set; }
        public string? Role { get; set; }
        public int AccountId { get; set; }
    }
}