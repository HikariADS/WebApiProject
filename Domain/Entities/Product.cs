using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WebApiProject.Domain.Constant;
using WebApiProject.Domain.Enums;

namespace WebApiProject.Domain.Entities
{
    [Table(TableNames.Product)]
    public class Product
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [StringLength(50)]
        public string Name { get; set; } = string.Empty;
        [Required]
        [StringLength(225)]
        public string Description { get; set; } = string.Empty;
        [Required]
        public int ProductTypeId { get; set; }
         public ProductType? ProductType { get; set; }
    }
}