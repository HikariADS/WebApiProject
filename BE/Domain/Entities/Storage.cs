using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WebApiProject.Domain.Constant;
using WebApiProject.Domain.Enums;

namespace WebApiProject.Domain.Entities
{
    [Table(TableNames.Storage)]
    public class Storage
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public int ProductId { get; set; }
        public Product? Product { get; set; }
        [Required]
        public int Quantity { get; set; }
        [Required]
        public string UserId { get; set; } = string.Empty;
        public User? User { get; set; }
        [Required]
        public int StorageTypeId { get; set; }
        public StorageType? StorageType { get; set; }
        public DateTimeOffset ImportDate { get; set; } = DateTimeOffset.Now;
        public DateTimeOffset? ExportDate { get; set; }
        public string? BelongToUnitId { get; set; }
        public string? ManagerId { get; set; } // ID của user manager
    }
}