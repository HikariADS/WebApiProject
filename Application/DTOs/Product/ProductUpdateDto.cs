using System.ComponentModel.DataAnnotations;

namespace WebApiProject.Application.DTOs
{
    public class ProductUpdateDto
    {
        [Required]
        public int Id { get; set; }
        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;
        [Required]
        [StringLength(225)]
        public string Description { get; set; } = string.Empty;
        [Required]
        int ProductTypeId { get; set; }

    }
}