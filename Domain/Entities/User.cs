using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WebApiProject.Domain.Constant;
using WebApiProject.Domain.Enums;
using System.Reflection.Metadata.Ecma335;

namespace WebApiProject.Domain.Entities
{
    public class User
    {
        [Required]
        public string UserName { get; set; } = string.Empty;
        [Required]
        public string Role { get; set; } = string.Empty;
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
        [Key]
        public int Id { get; set; }
        [Required]
        [StringLength(36)]
        public string Name { get; set; } = string.Empty;
        public DateTimeOffset CreatedDate { get; set; } = DateTimeOffset.UtcNow;

    }
}