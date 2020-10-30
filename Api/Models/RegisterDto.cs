using System.ComponentModel.DataAnnotations;

namespace Api.Models
{
    public class RegisterDto
    {
        [Required]
        [StringLength(20, MinimumLength = 6)]
        public string Username { get; set; }
        [Required]
        [StringLength(20, MinimumLength = 6)]
        public string Password { get; set; }
        [Required]
        [DataType(DataType.EmailAddress)]
        [EmailAddress]
        public string Email { get; set; }
    }
}