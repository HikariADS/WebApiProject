using WebApiProject.Application.DTOs.ProductType;

namespace WebApiProject.Application.IServices
{
    public interface IProductTypeService
    {
        Task<IEnumerable<ProductTypeDto>> GetAllAsync();
        Task<ProductTypeDto?> GetByIdAsync(int id);
        Task<ProductTypeDto> CreateAsync(ProductTypeCreateDto dto);
        Task<bool> UpdateAsync(ProductTypeUpdateDto dto);
        Task<bool> DeleteAsync(int id);
    }
}
