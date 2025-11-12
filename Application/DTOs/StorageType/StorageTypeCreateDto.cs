using System.ComponentModel.DataAnnotations;

namespace WebApiProject.Application.DTOs.StorageType
{
    public class StorageTypeCreateDto
    {
        [Required, StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required, StringLength(100)]
        public string ManagerName { get; set; } = string.Empty;

        [Required, StringLength(255)]
        public string StorageLocation { get; set; } = string.Empty;
    }
}
