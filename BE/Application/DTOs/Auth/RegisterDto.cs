using System.ComponentModel.DataAnnotations;

namespace WebApiProject.Application.DTOs.Auth
{
    public class RegisterDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
        [Required]
        [StringLength(50, MinimumLength = 6)]
        public string Password { get; set; } = string.Empty;
        [Required]
        public string UserName { get; set; } = string.Empty;
        [Required]
        [StringLength(30)]
        public string Name { get; set; } = string.Empty;
        
        [Phone]
        public string? PhoneNumber { get; set; }
    }
}