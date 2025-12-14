using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WebApiProject.Domain.Constant;

namespace WebApiProject.Domain.Entities
{
    [Table(TableNames.News)]
    public class News
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(200)]
        public string Title { get; set; } = string.Empty;

        [Required]
        [StringLength(2000)]
        public string Content { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string Category { get; set; } = "Thông báo";

        [Required]
        [StringLength(450)]
        public string AuthorId { get; set; } = string.Empty;

        [StringLength(100)]
        public string AuthorName { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        // Navigation property
        [ForeignKey("AuthorId")]
        public virtual User? Author { get; set; }
    }
}

