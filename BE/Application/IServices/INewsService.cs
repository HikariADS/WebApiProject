using WebApiProject.Application.DTOs.News;

namespace WebApiProject.Application.IServices
{
    public interface INewsService
    {
        Task<IEnumerable<NewsDto>> GetAllAsync();
        Task<NewsDto?> GetByIdAsync(int id);
        Task<NewsDto> CreateAsync(NewsCreateDto dto, string authorId, string authorName);
        Task<bool> UpdateAsync(NewsUpdateDto dto, string authorId);
        Task<bool> DeleteAsync(int id, string authorId);
        Task<bool> UpdateByAdminAsync(NewsUpdateDto dto);
        Task<bool> DeleteByAdminAsync(int id);
    }
}

