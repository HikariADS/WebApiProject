using WebApiProject.Domain.Entities;
using WebApiProject.Application.DTOs.Product;

namespace WebApiProject.Application.Mapping
{
    public static class ProductMapper
    {
        public static ProductDto ToDto(this Product entity)
        {
            return new ProductDto
            {
                Id = entity.Id,
                Name = entity.Name,
                Description = entity.Description,
                ProductTypeId = entity.ProductTypeId,
                ProductTypeName = entity.ProductType != null ? entity.ProductType.Name : null
            };
        }

        public static Product ToEntity(this ProductCreateDto dto)
        {
            return new Product
            {
                Name = dto.Name,
                Description = dto.Description,
                ProductTypeId = dto.ProductTypeId
            };
        }

        public static void UpdateEntity(this Product entity, ProductUpdateDto dto)
        {
            entity.Name = dto.Name;
            entity.Description = dto.Description;
            entity.ProductTypeId = dto.ProductTypeId;
        }
    }
}
