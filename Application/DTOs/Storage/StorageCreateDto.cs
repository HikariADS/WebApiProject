using System.ComponentModel.DataAnnotations;

namespace WebApiProject.Application.DTOs.Storage
{
    public class StorageCreateDto : StorageDto
    {
        [Required]
        [StringLength(225)]
        public string Description { get; set; } = string.Empty;
        [Required]
        [StringLength(100)]
        public string ProductType { get; set; } = string.Empty;
    }
}