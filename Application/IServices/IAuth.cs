using WebApiProject.Application.DTOs.Auth;

namespace WebApiProject.Application.IServices
{
    public interface IAuthService
    {
        Task<(bool Success, IEnumerable<string> Errors)> RegisterAsync(RegisterDto dto);
        Task<AuthResponseDto?> LoginAsync(LoginDto dto);
        Task<(bool Success, IEnumerable<string> Errors)> ChangeRoleAsync(ChangeRoleDto dto);

    }
}
