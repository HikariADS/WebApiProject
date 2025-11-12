using WebApiProject.Application.DTOs.Auth;

namespace WebApiProject.Application.IServices
{
    public interface IAuthService
    {
        Task<AuthResponseDto?> RegisterAsync(RegisterDto dto);
        Task<AuthResponseDto?> LoginAsync(LoginDto dto);
    }
}
