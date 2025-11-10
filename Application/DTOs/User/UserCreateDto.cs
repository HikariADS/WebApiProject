using System.ComponentModel.DataAnnotations;
using WebApiProject.Domain.Enums;

namespace WebApiProject.Application.DTOs.User
{
    public class UserCreateDto
    {
        [Required]
        [StringLength(50)]
        public string Name { get; set; } = string.Empty;
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
        [Required]
        [StringLength(50)]
        public string UserName { get; set; } = string.Empty;
        [Required]
        public string Role { get; set; } = string.Empty;
        public DateTimeOffset CreatedDate { get; set; } = DateTimeOffset.Now;
    }   
}