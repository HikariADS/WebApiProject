using WebApiProject.Application.DTOs.Product;

namespace WebApiProject.Application.IServices
{
    public interface IProductService
    {
        Task<IEnumerable<ProductDto>> GetAllAsync();
        Task<ProductDto?> GetByIdAsync(int id);
        Task<ProductDto> CreateAsync(ProductCreateDto dto);
        Task<bool> UpdateAsync(ProductUpdateDto dto);
        Task<bool> DeleteAsync(int id);
    }
}
