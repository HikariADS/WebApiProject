using WebApiProject.Domain.Entities;
using WebApiProject.Application.DTOs.ProductType;

namespace WebApiProject.Application.Mapping
{
    public static class ProductTypeMapper
    {
        public static ProductTypeDto ToDto(this ProductType entity)
        {
            return new ProductTypeDto
            {
                Id = entity.Id,
                Name = entity.Name,
                Description = entity.Description,

            };
        }
        public static ProductType ToEntity(this ProductTypeCreateDto dto)
        {
            return new ProductType
            {
                Name = dto.Name,
                Description = dto.Description,
            };
        }
        public static void UpdateEntity(this ProductType entity, ProductTypeUpdateDto dto)
        {
            entity.Name = dto.Name;
            entity.Description = dto.Description;
            entity.Id = dto.Id;
        }
    }
}