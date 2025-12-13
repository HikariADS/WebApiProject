using WebApiProject.Application.DTOs.Auth;

namespace WebApiProject.Application.IServices
{
    public interface IAuthService
    {
        Task<(bool Success, IEnumerable<string> Errors)> RegisterAsync(RegisterDto dto);
        Task<(AuthResponseDto? Response, string? ErrorMessage)> LoginAsync(LoginDto dto);
        Task<(bool Success, IEnumerable<string> Errors)> ChangeRoleAsync(ChangeRoleDto dto);
        Task<(bool Success, string Message)> VerifyEmailAsync(string token);
    }
}
