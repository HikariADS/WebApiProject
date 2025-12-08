using WebApiProject.Domain.Entities;

namespace WebApiProject.Application.IRepositories
{
    public interface IProductTypeRepository
    {
        // Lấy tất cả loại sản phẩm
        Task<IEnumerable<ProductType>> GetAllAsync();

        // Lấy loại sản phẩm theo ID
        Task<ProductType?> GetByIdAsync(int id);

        // Tạo loại sản phẩm mới
        Task AddAsync(ProductType entity);

        // Cập nhật loại sản phẩm
        Task UpdateAsync(ProductType entity);

        // Xóa loại sản phẩm
        Task DeleteAsync(int id);
    }
}
