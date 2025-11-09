using Microsoft.EntityFrameworkCore;
using WebApiProject.Domain.Entities;
using WebApiProject.Infrastructure.Persistence;

namespace WebApiProject.Infrastructure.Repositories
{
    public class ProductTypeRepository
    {
        private readonly AppDbContext _context;

        public ProductTypeRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ProductType>> GetAllAsync()
        {
            return await _context.ProductTypes.AsNoTracking().ToListAsync();
        }

        public async Task<ProductType?> GetByIdAsync(int id)
        {
            return await _context.ProductTypes.FindAsync(id);
        }

        public async Task AddAsync(ProductType entity)
        {
            await _context.ProductTypes.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(ProductType entity)
        {
            _context.ProductTypes.Update(entity);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await _context.ProductTypes.FindAsync(id);
            if (entity != null)
            {
                _context.ProductTypes.Remove(entity);
                await _context.SaveChangesAsync();
            }
        }
    }
}
