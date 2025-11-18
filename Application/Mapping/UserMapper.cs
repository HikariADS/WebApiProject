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
                Name = Entity.FullName,
                CreateDate = DateTime.Now
            };
        }
        public static User ToEntity(this UserCreateDto dto)
        {
            return new User
            {
                UserName = dto.UserName,
                Email = dto.Email,
                CreatedDate = DateTimeOffset.UtcNow
            };
        }
        public static void UpdateEntity(this User entity, UserDto dto)
        {
            entity.FullName = dto.Name;
            entity.Email = dto.Email;
            entity.UserName = dto.UserName;
        }
    }
}