using System.ComponentModel.DataAnnotations;

namespace WebApiProject.Application.DTOs.ProductType
{
    public class ProductTypeUpdateDto : ProductTypeCreateDto
    {
        [Required]
        public int Id { get; set; }
    }
}