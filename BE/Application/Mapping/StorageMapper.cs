using WebApiProject.Application.DTOs.Storage;
using WebApiProject.Domain.Entities;

namespace WebApiProject.Application.Mapping
{
    public static class StorageMapper
    {
        public static StorageDto ToDto(this Storage enitity)
        {
            return new StorageDto
            {
                Id = enitity.Id,
                ProductId = enitity.ProductId,
                StorageTypeId = enitity.StorageTypeId,
                ExportDate = enitity.ExportDate,
                ImportDate = enitity.ImportDate,
                Quantity = enitity.Quantity,
            };
        }
        public static Storage ToEntity(this StorageCreateDto dto)
        {
            return new Storage
            {
                ProductId = dto.ProductId,
                StorageTypeId = dto.StorageTypeId,
                Quantity = dto.Quantity,
                ImportDate = dto.ImportDate,
            };
        }
        public static void UpdateEntity(this Storage entity, StorageUpdateDto dto)
        {
            entity.Quantity = dto.Quantity;
            entity.ImportDate = dto.ImportDate;
            entity.StorageTypeId = dto.StorageTypeId;
            entity.ExportDate = dto.ExportDate;
        }
    }
}