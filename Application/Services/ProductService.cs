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

        public ProductService(IProductRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<ProductDto>> GetAllAsync()
        {
            var entities = await _repository.GetAllAsync();
            return entities.Select(e => e.ToDto());
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
    }
}
