namespace WebApiProject.Application.DTOs.Storage
{
    public class StorageDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int ProductId{ get; set; }
        public int StorageTypeId { get; set; }
        public int Quantity { get; set; }
        public DateTimeOffset ImportDate { get; set; }
        public DateTimeOffset ExportDate { get; set; }
        
    }
}