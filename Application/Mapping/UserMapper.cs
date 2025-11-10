using WebApiProject.Application.DTOs;
using WebApiProject.Application.DTOs.User;
using WebApiProject.Domain.Entities;

namespace WebApiProject.Application.Mapping
{
    public static class UserMapper
    {
        public static UserDto ToDto(this User Entity)
        {
            return new UserDto
            {
                Id = Entity.Id,
                Name = Entity.Name,
                Email = Entity.Email,
                UserName = Entity.UserName,
                CreateDate = DateTime.Now,
                Role = Entity.Role,
            };
        }
        public static User ToEntity(this UserCreateDto dto)
        {
            return new User
            {
                UserName = dto.UserName,
                Email = dto.Email,
                Role = dto.Role,
                CreatedDate = dto.CreatedDate,
            };
        }
        public static void UpdateEntity(this User entity, UserDto dto)
        {
            entity.Name = dto.Name;
            entity.Email = dto.Email;
            entity.Role = dto.Role;
            entity.UserName = dto.UserName;
        }
    }
}