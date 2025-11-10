using WebApiProject.Application.DTOs.Product;
using WebApiProject.Application.IRepositories;
using WebApiProject.Application.Mapping;
using WebApiProject.Domain.Entities;

namespace WebApiProject.Application.Services
{
    public class ProductService
    {
        private readonly IProductRepository _repository;

        public ProductService(IProductRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<ProductDto>> GetAllAsync()
        {
            var products = await _repository.GetAllAsync();
            return products.Select(p => p.ToDto());
        }

        public async Task<ProductDto?> GetByIdAsync(int id)
        {
            var product = await _repository.GetByIdAsync(id);
            return product?.ToDto();
        }

        public async Task<ProductDto> CreateAsync(ProductCreateDto dto)
        {
            var entity = dto.ToEntity();
            await _repository.AddAsync(entity);
            return entity.ToDto();
        }

        public async Task<bool> UpdateAsync(ProductUpdateDto dto)
        {
            var product = await _repository.GetByIdAsync(dto.Id);
            if (product == null) return false;

            product.UpdateEntity(dto);
            await _repository.UpdateAsync(product);
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
