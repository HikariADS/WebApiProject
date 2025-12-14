using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApiProject.Domain.Entities
{
    [Table("PasswordResetCodes")]
    public class PasswordResetCode
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        [EmailAddress]
        [StringLength(256)]
        public string Email { get; set; } = string.Empty;

        [Required]
        [StringLength(10)]
        public string Code { get; set; } = string.Empty; // 6-8 digit code

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime ExpiresAt { get; set; } = DateTime.UtcNow.AddMinutes(15); // Code hết hạn sau 15 phút

        public bool IsUsed { get; set; } = false;
    }
}

