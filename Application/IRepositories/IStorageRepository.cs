using WebApiProject.Domain.Entities;

namespace WebApiProject.Application.IRepositories
{
    public interface IStorageRepository
    {
        // Lấy tất cả bản ghi hàng tồn kho
        Task<IEnumerable<Storage>> GetAllAsync();

        // Lấy thông tin tồn kho theo ID
        Task<Storage?> GetByIdAsync(int id);

        // Tạo mới bản ghi tồn kho
        Task AddAsync(Storage entity);

        // Cập nhật bản ghi tồn kho
        Task UpdateAsync(Storage entity);

        // Xóa bản ghi tồn kho
        Task DeleteAsync(int id);
    }
}
