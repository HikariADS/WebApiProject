using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WebApiProject.Domain.Constant;
using WebApiProject.Domain.Enums;

namespace WebApiProject.Domain.Entities
{
    [Table(TableNames.Storage)]
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
        public int StorageTypeId { get; set; }
        public DateTimeOffset ImportDate { get; set; } = DateTimeOffset.Now;
        public DateTimeOffset ExportDate { get; set; } = DateTimeOffset.Now;
    }
}