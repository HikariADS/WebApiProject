using System.ComponentModel.DataAnnotations;


namespace WebApiProject.Application.DTOs
{
    public class StorageCreateDto
    {
        [Required]
        [StringLength(225)]
        public string Description { get; set; } = string.Empty;
        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;
        [Required]
        [StringLength(100)]
        public string ProductType { get; set; } = string.Empty;
    }
}