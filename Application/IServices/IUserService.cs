using WebApiProject.Application.DTOs.User; 

namespace WebApiProject.Application.IServices
{
    public interface IUserService
    {        
        Task<(bool Success, IEnumerable<string> Errors)> UpdateUserAsync(string id, UserUpdateDto dto);
        
        Task<UserResponseDto?> GetUserByIdAsync(string id);
        Task<List<UserResponseDto>> GetAllUsersAsync();
        Task<(bool Success, IEnumerable<string> Errors)> CreateUserAsync(UserCreateDto dto);

    }
}