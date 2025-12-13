using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using WebApiProject.Application.DTOs.Paging;
using WebApiProject.Application.DTOs.Product;
using WebApiProject.Application.IRepositories;
using WebApiProject.Application.IServices;
using WebApiProject.Application.Mapping;
using WebApiProject.Domain.Entities;

namespace WebApiProject.Application.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _repository;
        private readonly IGenericPagingService<Product, ProductDto> _paging;
        public ProductService(
            IGenericPagingService<Product, ProductDto> paging,
            IProductRepository repository)
        {
            _repository = repository;
            _paging = paging;
        }

        public async Task<IEnumerable<ProductDto>> GetAllAsync()
        {
            var entities = await _repository.GetAllAsync(new ProductQueryRequest());
            return entities.Items;
        }

        public async Task<ProductDto?> GetByIdAsync(int id)
        {
            var entity = await _repository.GetByIdAsync(id);
            return entity?.ToDto();
        }

        public async Task<ProductDto> CreateAsync(ProductCreateDto dto)
        {
            var entity = dto.ToEntity();
            await _repository.AddAsync(entity);
            return entity.ToDto();
        }

        public async Task<bool> UpdateAsync(ProductUpdateDto dto)
        {
            var entity = await _repository.GetByIdAsync(dto.Id);
            if (entity == null) return false;

            entity.UpdateEntity(dto);
            await _repository.UpdateAsync(entity);
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var existing = await _repository.GetByIdAsync(id);
            if (existing == null) return false;

            await _repository.DeleteAsync(id);
            return true;
        }
        public async Task<PageResult<ProductDto>> GetAllAsync(PageRequest request)
        {
            IQueryable<Product> query = _repository.GetQueryable();
            
            // Apply search filter
            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                var searchTerm = request.Search.Trim();
                query = query.Where(p => p.Name.Contains(searchTerm) || p.Description.Contains(searchTerm));
            }

            // Apply sorting
            if (!string.IsNullOrWhiteSpace(request.SortBy))
            {
                if (request.SortOrder == "desc")
                {
                    query = request.SortBy.ToLower() switch
                    {
                        "name" => query.OrderByDescending(p => p.Name),
                        "description" => query.OrderByDescending(p => p.Description),
                        "price" => query.OrderByDescending(p => p.Price),
                        _ => query.OrderByDescending(p => p.Id)
                    };
                }
                else
                {
                    query = request.SortBy.ToLower() switch
                    {
                        "name" => query.OrderBy(p => p.Name),
                        "description" => query.OrderBy(p => p.Description),
                        "price" => query.OrderBy(p => p.Price),
                        _ => query.OrderBy(p => p.Id)
                    };
                }
            }
            else
            {
                query = query.OrderBy(p => p.Id);
            }

            // Get total count
            var totalItems = await query.CountAsync();

            // Apply paging and include ProductType
            var skip = (request.PageNumber - 1) * request.PageSize;
            var entities = await query
                .Include(p => p.ProductType)
                .Skip(skip)
                .Take(request.PageSize)
                .ToListAsync();

            // Map to DTOs with ProductTypeName
            var items = entities.Select(p => new ProductDto
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                Price = p.Price,
                ProductTypeId = p.ProductTypeId,
                ProductTypeName = p.ProductType?.Name ?? string.Empty
            }).ToList();

            return new PageResult<ProductDto>
            {
                Items = items,
                TotalItems = totalItems,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize
            };
        }
    }
}
