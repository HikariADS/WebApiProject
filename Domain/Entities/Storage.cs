using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WebApiProject.Models;
using WebApiProject.Enums;

namespace WebApiProject.Models
{
    [Table("TableNames.Storage")]
    public class Storage
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public int ProductId { get; set; }
        [Required]
        public int Quantity { get; set; }
        [Required]
        public int UserId { get; set; }
        [Required]
        public int StorageTypesId { get; set; }
        public DateTimeOffset ImportDate { get; set; } = DateTimeOffset.Now;
        public DateTimeOffset ExportDate { get; set; } = DateTimeOffset.Now;
    }
}