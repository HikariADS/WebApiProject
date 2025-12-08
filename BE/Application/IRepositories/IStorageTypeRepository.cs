using WebApiProject.Domain.Entities;

namespace WebApiProject.Application.IRepositories
{
    public interface IStorageTypeRepository
    {
        // Lấy danh sách tất cả loại kho
        Task<IEnumerable<StorageType>> GetAllAsync();

        // Lấy thông tin loại kho theo ID
        Task<StorageType?> GetByIdAsync(int id);

        // Tạo mới loại kho
        Task AddAsync(StorageType entity);

        // Cập nhật loại kho
        Task UpdateAsync(StorageType entity);

        // Xóa loại kho
        Task DeleteAsync(int id);
    }
}
