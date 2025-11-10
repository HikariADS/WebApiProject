using System.ComponentModel.DataAnnotations;

namespace WebApiProject.Application.DTOs.ProductType
{
    public class ProductTypeCreateDto
    {
        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;
        [Required]
        [StringLength(225)]
        public string Description { get; set; } = string.Empty;
        
    }
}