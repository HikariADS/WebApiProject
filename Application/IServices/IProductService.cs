using WebApiProject.Application.DTOs.Product;
using WebApiProject.Domain.Entities;

namespace WebApiProject.Application.IServices
{
    public interface IProductService
    {
        
        Task<ProductDto?> GetByIdAsync(int id);
        Task<ProductDto> CreateAsync(ProductCreateDto dto);
        Task<bool> UpdateAsync(ProductUpdateDto dto);
        Task<bool> DeleteAsync(int id);
    }
}
