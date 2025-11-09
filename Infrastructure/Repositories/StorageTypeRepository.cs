using Microsoft.EntityFrameworkCore;
using WebApiProject.Domain.Entities;
using WebApiProject.Infrastructure.Persistence;

namespace WebApiProject.Infrastructure.Repositories
{
    public class StorageTypeRepository
    {
        private readonly AppDbContext _context;

        public StorageTypeRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<StorageType>> GetAllAsync()
        {
            return await _context.StorageTypes.AsNoTracking().ToListAsync();
        }

        public async Task<StorageType?> GetByIdAsync(int id)
        {
            return await _context.StorageTypes.FindAsync(id);
        }

        public async Task AddAsync(StorageType entity)
        {
            await _context.StorageTypes.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(StorageType entity)
        {
            _context.StorageTypes.Update(entity);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await _context.StorageTypes.FindAsync(id);
            if (entity != null)
            {
                _context.StorageTypes.Remove(entity);
                await _context.SaveChangesAsync();
            }
        }
    }
}
