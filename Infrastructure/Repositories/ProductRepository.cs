using Microsoft.EntityFrameworkCore;
using WebApiProject.Domain.Entities;
using WebApiProject.Application.IRepositories;
using WebApiProject.Application.DTOs.Paging;
using WebApiProject.Application.DTOs.Product;
using WebApiProject.Infrastructure.Persistence;
using System.Data.Common;

namespace WebApiProject.Infrastructure.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly AppDbContext _context;

        public ProductRepository(AppDbContext context)
        {
            _context = context;
        }
        public async Task<PageResult<ProductDto>> GetAllAsync(ProductQueryRequest request)
        {
            var query = _context.Products.AsQueryable();
            if (!string.IsNullOrEmpty(request.Keyword))
            {
                query = query.Where(p => p.Name.Contains(request.Keyword));
            }
            var totalItems = await query.CountAsync();
            var items = await query
                .Skip((request.PageNumber -1) * request.PageSize)
                .Take(request.PageSize)
                .Select(p => new ProductDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    Price = p.Price
                })
                .ToListAsync();
            return new PageResult<ProductDto>{
                Items = items, 
                TotalItems = totalItems, 
                PageNumber = request.PageNumber, 
                PageSize = request.PageSize
            };
        }
        public async Task<Product?> GetByIdAsync(int id)
        {
            return await _context.Products.FindAsync(id);
        }

        public async Task AddAsync(Product entity)
        {
            await _context.Products.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Product entity)
        {
            _context.Products.Update(entity);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product != null)
            {
                _context.Products.Remove(product);
                await _context.SaveChangesAsync();
            }
        }
        public IQueryable<Product> GetQueryable()
        {
            return _context.Products.AsQueryable();
        }

    }
}
