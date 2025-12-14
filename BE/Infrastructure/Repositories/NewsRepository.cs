using Microsoft.EntityFrameworkCore;
using WebApiProject.Application.IRepositories;
using WebApiProject.Domain.Entities;
using WebApiProject.Infrastructure.Persistence;

namespace WebApiProject.Infrastructure.Repositories
{
    public class NewsRepository : INewsRepository
    {
        private readonly AppDbContext _context;

        public NewsRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<News>> GetAllAsync()
        {
            return await _context.News
                .Include(n => n.Author)
                .OrderByDescending(n => n.CreatedAt)
                .ToListAsync();
        }

        public async Task<News?> GetByIdAsync(int id)
        {
            return await _context.News
                .Include(n => n.Author)
                .FirstOrDefaultAsync(n => n.Id == id);
        }

        public async Task<News> AddAsync(News entity)
        {
            await _context.News.AddAsync(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task<bool> UpdateAsync(News entity)
        {
            _context.News.Update(entity);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var news = await _context.News.FindAsync(id);
            if (news == null) return false;
            
            _context.News.Remove(news);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}

