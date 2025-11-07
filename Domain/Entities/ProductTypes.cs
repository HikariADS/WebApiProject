using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WebApiProject.Enums;
using WebApiProject.Constant;

namespace WebApiProject.Models
{
    [Table("TableNames.ProductTypes")]
    public class ProductTypes
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [StringLength(255)]
        public string Name { get; set; } = string.Empty;
        [StringLength(255)]
        public string Description { get; set;} = string.Empty;
    }
}