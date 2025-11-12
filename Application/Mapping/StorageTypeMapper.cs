using WebApiProject.Application.DTOs.StorageType;
using WebApiProject.Domain.Entities;

namespace WebApiProject.Application.Mapping
{
    public static class StorageTypeMapper
    {
        public static StorageTypeDto ToDto(this StorageType entity)
        {
            return new StorageTypeDto
            {
                Id = entity.Id,
                Name = entity.Name,
                ManagerName = entity.ManagerName,
                StorageLocation = entity.StorageLocation
            };
        }

        public static StorageType ToEntity(this StorageTypeCreateDto dto)
        {
            return new StorageType
            {
                Name = dto.Name,
                ManagerName = dto.ManagerName,
                StorageLocation = dto.StorageLocation
            };
        }

        public static void UpdateEntity(this StorageType entity, StorageTypeUpdateDto dto)
        {
            entity.Name = dto.Name;
            entity.ManagerName = dto.ManagerName;
            entity.StorageLocation = dto.StorageLocation;
        }
    }
}
