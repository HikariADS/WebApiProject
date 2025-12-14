using WebApiProject.Domain.Entities;

namespace WebApiProject.Application.IRepositories
{
    public interface INewsRepository
    {
        Task<IEnumerable<News>> GetAllAsync();
        Task<News?> GetByIdAsync(int id);
        Task<News> AddAsync(News entity);
        Task<bool> UpdateAsync(News entity);
        Task<bool> DeleteAsync(int id);
    }
}

