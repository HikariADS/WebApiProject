using WebApiProject.Application.DTOs.News;
using WebApiProject.Application.IRepositories;
using WebApiProject.Application.IServices;
using WebApiProject.Domain.Entities;

namespace WebApiProject.Application.Services
{
    public class NewsService : INewsService
    {
        private readonly INewsRepository _repository;

        public NewsService(INewsRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<NewsDto>> GetAllAsync()
        {
            var news = await _repository.GetAllAsync();
            return news.Select(n => new NewsDto
            {
                Id = n.Id,
                Title = n.Title,
                Content = n.Content,
                Category = n.Category,
                AuthorId = n.AuthorId,
                AuthorName = n.AuthorName,
                CreatedAt = n.CreatedAt,
                UpdatedAt = n.UpdatedAt
            });
        }

        public async Task<NewsDto?> GetByIdAsync(int id)
        {
            var news = await _repository.GetByIdAsync(id);
            if (news == null) return null;

            return new NewsDto
            {
                Id = news.Id,
                Title = news.Title,
                Content = news.Content,
                Category = news.Category,
                AuthorId = news.AuthorId,
                AuthorName = news.AuthorName,
                CreatedAt = news.CreatedAt,
                UpdatedAt = news.UpdatedAt
            };
        }

        public async Task<NewsDto> CreateAsync(NewsCreateDto dto, string authorId, string authorName)
        {
            var news = new News
            {
                Title = dto.Title,
                Content = dto.Content,
                Category = dto.Category,
                AuthorId = authorId,
                AuthorName = authorName,
                CreatedAt = DateTime.UtcNow
            };

            var created = await _repository.AddAsync(news);
            return new NewsDto
            {
                Id = created.Id,
                Title = created.Title,
                Content = created.Content,
                Category = created.Category,
                AuthorId = created.AuthorId,
                AuthorName = created.AuthorName,
                CreatedAt = created.CreatedAt,
                UpdatedAt = created.UpdatedAt
            };
        }

        public async Task<bool> UpdateAsync(NewsUpdateDto dto, string authorId)
        {
            var news = await _repository.GetByIdAsync(dto.Id);
            if (news == null) return false;

            // Chỉ cho phép tác giả hoặc Admin cập nhật
            if (news.AuthorId != authorId)
            {
                // Có thể thêm logic kiểm tra role Admin ở đây nếu cần
                return false;
            }

            news.Title = dto.Title;
            news.Content = dto.Content;
            news.Category = dto.Category;
            news.UpdatedAt = DateTime.UtcNow;

            return await _repository.UpdateAsync(news);
        }

        public async Task<bool> DeleteAsync(int id, string authorId)
        {
            var news = await _repository.GetByIdAsync(id);
            if (news == null) return false;

            // Chỉ cho phép tác giả hoặc Admin xóa
            if (news.AuthorId != authorId)
            {
                // Có thể thêm logic kiểm tra role Admin ở đây nếu cần
                return false;
            }

            return await _repository.DeleteAsync(id);
        }

        // Method cho Admin để xóa/sửa bất kỳ tin tức nào
        public async Task<bool> UpdateByAdminAsync(NewsUpdateDto dto)
        {
            var news = await _repository.GetByIdAsync(dto.Id);
            if (news == null) return false;

            news.Title = dto.Title;
            news.Content = dto.Content;
            news.Category = dto.Category;
            news.UpdatedAt = DateTime.UtcNow;

            return await _repository.UpdateAsync(news);
        }

        public async Task<bool> DeleteByAdminAsync(int id)
        {
            var news = await _repository.GetByIdAsync(id);
            if (news == null) return false;

            return await _repository.DeleteAsync(id);
        }
    }
}

