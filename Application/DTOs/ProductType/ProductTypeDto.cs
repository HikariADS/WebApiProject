using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
namespace WebApiProject.Application.DTOs.ProductType
{
    public class ProductTypeDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string ProductTypeName { get; set; } = string.Empty;
    }
}