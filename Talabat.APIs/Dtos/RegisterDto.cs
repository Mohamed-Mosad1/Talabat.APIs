using System.ComponentModel.DataAnnotations;

namespace Talabat.APIs.Dtos
{
    public class RegisterDto
    {
        [Required]
        public string DisplayName { get; set; } = null!;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = null!;

        [Required]
        [Phone]
        public string PhoneNumber { get; set; } = null!;

        [Required]
        [RegularExpression("(?=^.{8,}$)((?=.*\\d)|(?=.*[\\W_]))(?![.\\n])(?=.*[A-Z])(?=.*[a-z]).*$", ErrorMessage = "Password must be eight characters including one uppercase letter, one special character and alphanumeric characters")]
        public string Password { get; set; } = null!;
    }
}
