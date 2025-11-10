using Microsoft.EntityFrameworkCore;
using WebApiProject.Application.DTOs;
using WebApiProject.Domain.Entities;
using WebApiProject.Infrastructure.Persistence;

namespace WebApiProject.Application.Services
{
    public class ProductService
    {
        private readonly AppDbContext _context;

        public ProductService(AppDbContext context)
        {
            _context = context;
        }

        // Lấy tất cả sản phẩm
        public async Task<IEnumerable<ProductDto>> GetAllAsync()
        {
            return await _context.Products
                .Select(p => new ProductDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    Description = p.Description,
                    ProductTypeId = p.ProductTypeId
                })
                .ToListAsync();
        }

        // Lấy sản phẩm theo ID
        public async Task<ProductDto?> GetByIdAsync(int id)
        {
            var p = await _context.Products.FindAsync(id);
            if (p == null) return null;

            return new ProductDto
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                ProductTypeId = p.ProductTypeId
            };
        }

        // Tạo mới sản phẩm
        public async Task<ProductDto> CreateAsync(ProductCreateDto dto)
        {
            var entity = new Product
            {
                Name = dto.Name,
                Description = dto.Description,
                ProductTypeId = dto.ProductTypeId
            };

            _context.Products.Add(entity);
            await _context.SaveChangesAsync();

            return new ProductDto
            {
                Id = entity.Id,
                Name = entity.Name,
                Description = entity.Description,
                ProductTypeId = entity.ProductTypeId
            };
        }

        // Cập nhật sản phẩm
        public async Task<bool> UpdateAsync(ProductUpdateDto dto)
        {
            var entity = await _context.Products.FindAsync(dto.Id);
            if (entity == null) return false;

            entity.Name = dto.Name;
            entity.Description = dto.Description;
            entity.ProductTypeId = dto.ProductTypeId;

            _context.Products.Update(entity);
            await _context.SaveChangesAsync();
            return true;
        }

        // Xóa sản phẩm
        public async Task<bool> DeleteAsync(int id)
        {
            var entity = await _context.Products.FindAsync(id);
            if (entity == null) return false;

            _context.Products.Remove(entity);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
