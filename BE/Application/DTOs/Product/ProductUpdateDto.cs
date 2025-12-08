using System.ComponentModel.DataAnnotations;

namespace WebApiProject.Application.DTOs.Product
{
    public class ProductUpdateDto : ProductCreateDto
    {
        [Required]
        public int Id { get; set; }
    }
}