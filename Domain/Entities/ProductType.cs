using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WebApiProject.Domain.Enums;
using WebApiProject.Domain.Constant;

namespace WebApiProject.Domain.Entities
{
    [Table(TableNames.ProductType)]
    public class ProductType
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [StringLength(255)]
        public string Name { get; set; } = string.Empty;
        [StringLength(255)]
        public string Description { get; set; } = string.Empty;
        [StringLength(225)]
        public string ProductTypeName { get; set; } = string.Empty;
        
    }
}