using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WebApiProject.Enums;
using WebApiProject.Models;

namespace WebApiProject.Models
{
    [Table(TableNames.StorageType)]
    public class StorageType
    {
        [Key]
        public int StorageTypeId { get; set; }
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