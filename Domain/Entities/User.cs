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
        public string UserName { get; set; }
        [Required]
        public string Role { get; set; }
        [EmailAddress]
        public string Email { get; set; }
        [Key]
        public int Id { get; set; }
        [Required]
        [StringLength(36)]
        public string Name { get; set; }
        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

    }
}