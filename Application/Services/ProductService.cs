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
            var query = _repository.GetQueryable();
            return await _paging.PagingAsync(
                query,
                request,
                p => p.Name,
                p => p.Description
            );

        }
    }
}
