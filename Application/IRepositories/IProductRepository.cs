using WebApiProject.Domain.Entities;

namespace WebApiProject.Application.IRepositories
{
    public interface IProductRepository
    {
        // Lấy tất cả sản phẩm
        Task<IEnumerable<Product>> GetAllAsync();

        // Lấy sản phẩm theo ID
        Task<Product?> GetByIdAsync(int id);

        // Tạo sản phẩm mới
        Task AddAsync(Product entity);

        // Cập nhật sản phẩm
        Task UpdateAsync(Product entity);

        // Xóa sản phẩm
        Task DeleteAsync(int id);
    }
}
