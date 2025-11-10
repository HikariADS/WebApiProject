using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace WebApiProject.Application.DTOs
{
    public class ProductCreateDto
    {
        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;
        [Required]
        [StringLength(225)]
        public string Description { get; set; } = string.Empty;
        [Required]
        [StringLength(100)]
        public int ProductTypeId { get; set; }
    }
}