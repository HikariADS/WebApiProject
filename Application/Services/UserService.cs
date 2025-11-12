using WebApiProject.Application.DTOs.User;
using WebApiProject.Application.IServices;
using WebApiProject.Domain.Entities;

namespace WebApiProject.Application.Services
{
    public class UserService : IUserService
    {
        public Task<(bool Success, IEnumerable<string> Errors)> UpdateUserAsync(string id, UserUpdateDto dto)
        {
            throw new NotImplementedException("Chức năng cập nhật user sẽ được triển khai khi có Identity.");
        }

        public Task<UserResponseDto?> GetUserByIdAsync(string id)
        {
            throw new NotImplementedException("Chức năng này sẽ sử dụng dữ liệu từ Identity hoặc Repository riêng.");
        }

        public Task<List<UserResponseDto>> GetAllUsersAsync()
        {
            throw new NotImplementedException("Chức năng này sẽ được triển khai khi tích hợp Identity.");
        }

        public Task<(bool Success, IEnumerable<string> Errors)> CreateUserAsync(UserCreateDto dto)
        {
            throw new NotImplementedException("Chức năng tạo user cần tích hợp hệ thống Auth.");
        }
    }
}
