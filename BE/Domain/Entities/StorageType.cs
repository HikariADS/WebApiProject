using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WebApiProject.Domain.Enums;
using WebApiProject.Domain.Constant;

namespace WebApiProject.Domain.Entities
{
    [Table(TableNames.StorageType)]
    public class StorageType
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;
        [Required]
        [StringLength(100)]
        public string ManagerName { get; set; } = string.Empty;
        [Required]
        [StringLength(225)]
        public string StorageLocation { get; set; } = string.Empty;
    }
}