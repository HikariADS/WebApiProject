using WebApiProject.Application.DTOs.Product;
using WebApiProject.Domain.Entities;
using WebApiProject.Application.DTOs.Paging;

namespace WebApiProject.Application.IServices
{
    public interface IProductService
    {
        
        Task<ProductDto?> GetByIdAsync(int id);
        Task<ProductDto> CreateAsync(ProductCreateDto dto);
        Task<bool> UpdateAsync(ProductUpdateDto dto);
        Task<bool> DeleteAsync(int id);
        Task<PageResult<ProductDto>> GetAllAsync(PageRequest request);

    }
}
