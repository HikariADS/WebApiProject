using WebApiProject.Application.DTOs.StorageType;

namespace WebApiProject.Application.IServices
{
    public interface IStorageTypeService
    {
        Task<IEnumerable<StorageTypeDto>> GetAllAsync();
        Task<StorageTypeDto?> GetByIdAsync(int id);
        Task<StorageTypeDto> CreateAsync(StorageTypeCreateDto dto);
        Task<bool> UpdateAsync(StorageTypeUpdateDto dto);
        Task<bool> DeleteAsync(int id);
    }
}
