using WebApiProject.Application.DTOs.Storage;
using WebApiProject.Application.IRepositories;
using WebApiProject.Application.IServices;
using WebApiProject.Application.Mapping;
using WebApiProject.Domain.Entities;

namespace WebApiProject.Application.Services
{
    public class StorageService : IStorageService
    {
        private readonly IStorageRepository _repository;

        public StorageService(IStorageRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<StorageDto>> GetAllAsync()
        {
            var entities = await _repository.GetAllAsync();
            return entities.Select(e => e.ToDto());
        }

        public async Task<StorageDto?> GetByIdAsync(int id)
        {
            var entity = await _repository.GetByIdAsync(id);
            return entity?.ToDto();
        }

        public async Task<StorageDto> CreateAsync(StorageCreateDto dto)
        {
            var entity = dto.ToEntity();
            await _repository.AddAsync(entity);
            return entity.ToDto();
        }

        public async Task<bool> UpdateAsync(StorageUpdateDto dto)
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
