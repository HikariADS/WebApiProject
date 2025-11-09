using Microsoft.EntityFrameworkCore;
using WebApiProject.Domain.Entities;
using WebApiProject.Infrastructure.Persistence;

namespace WebApiProject.Infrastructure.Repositories
{
    public class StorageRepository
    {
        private readonly AppDbContext _context;

        public StorageRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Storage>> GetAllAsync()
        {
            return await _context.Storages.AsNoTracking().ToListAsync();
        }

        public async Task<Storage?> GetByIdAsync(int id)
        {
            return await _context.Storages.FindAsync(id);
        }

        public async Task AddAsync(Storage storage)
        {
            await _context.Storages.AddAsync(storage);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Storage storage)
        {
            _context.Storages.Update(storage);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var storage = await _context.Storages.FindAsync(id);
            if (storage != null)
            {
                _context.Storages.Remove(storage);
                await _context.SaveChangesAsync();
            }
        }
    }
}
