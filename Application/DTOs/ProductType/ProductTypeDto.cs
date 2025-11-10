using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace WebApiProject.Application.DTOs
{
    public class ProductypeDto
    {
        public int ProdcutTyopId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }
}