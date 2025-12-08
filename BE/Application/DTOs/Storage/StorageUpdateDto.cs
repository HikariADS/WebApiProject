using System.ComponentModel.DataAnnotations;

namespace WebApiProject.Application.DTOs.Storage
{
    public class StorageUpdateDto : StorageCreateDto
    {
        [Required]
        public DateTimeOffset ExprotDate { get; set; } = DateTimeOffset.UtcNow;
        public string? BelongToUnitId { get; set; }
        public string? ManagerId { get; set; }
    }
}