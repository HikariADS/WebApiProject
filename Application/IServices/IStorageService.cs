using WebApiProject.Application.DTOs.Storage;

namespace WebApiProject.Application.IServices
{
    public interface IStorageService
    {
        Task<IEnumerable<StorageDto>> GetAllAsync();
        Task<StorageDto?> GetByIdAsync(int id);
        Task<StorageDto> CreateAsync(StorageCreateDto dto);
        Task<bool> UpdateAsync(StorageUpdateDto dto);
        Task<bool> DeleteAsync(int id);
    }
}
