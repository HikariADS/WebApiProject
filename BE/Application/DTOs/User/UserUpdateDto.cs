using System.ComponentModel.DataAnnotations;

namespace WebApiProject.Application.DTOs.User
{
    public class UserUpdateDto
    {
        [Required]
        public string Id { get; set; } = string.Empty;
        
        [Required]
        [StringLength(50)]
        public string Name { get; set; } = string.Empty;
        
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
        
        [Phone]
        public string? PhoneNumber { get; set; }
        
        public string? UnitId { get; set; }
    }
}