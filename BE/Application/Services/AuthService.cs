using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using WebApiProject.Application.DTOs.Auth;
using WebApiProject.Application.DTOs.User;
using WebApiProject.Application.IServices;
using WebApiProject.Application.IRepositories;
using WebApiProject.Domain.Entities;


namespace WebApiProject.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<User> _userManager;
        private readonly IConfiguration _config;
        private readonly IPendingRegistrationRepository _pendingRegistrationRepository;
        private readonly IEmailService _emailService;

        public AuthService(UserManager<User> userManager, IConfiguration config, IPendingRegistrationRepository pendingRegistrationRepository, IEmailService emailService)
        {
            _userManager = userManager;
            _config = config;
            _pendingRegistrationRepository = pendingRegistrationRepository;
            _emailService = emailService;
        }

        public async Task<(bool Success, IEnumerable<string> Errors)> RegisterAsync(RegisterDto dto)
        {
            // Kiểm tra email đã tồn tại trong database (đã verify) hoặc đang pending
            var existingUser = await _userManager.FindByEmailAsync(dto.Email);
            if (existingUser != null)
            {
                return (false, new[] { "Email này đã được sử dụng" });
            }

            var existingPending = await _pendingRegistrationRepository.GetByEmailAsync(dto.Email);
            
            if (existingPending != null)
            {
                // Nếu đã có pending registration, xóa cái cũ
                await _pendingRegistrationRepository.RemoveAsync(existingPending);
            }

            // Tạo verification token
            var verificationToken = Convert.ToBase64String(RandomNumberGenerator.GetBytes(32))
                .Replace("+", "-")
                .Replace("/", "_")
                .Replace("=", "");

            // Hash password
            var passwordHasher = new PasswordHasher<User>();
            var tempUser = new User { UserName = dto.UserName };
            var passwordHash = passwordHasher.HashPassword(tempUser, dto.Password);

            // Lưu thông tin đăng ký tạm thời
            var pendingRegistration = new PendingRegistration
            {
                UserName = dto.UserName,
                Email = dto.Email,
                PasswordHash = passwordHash,
                Name = dto.Name,
                PhoneNumber = dto.PhoneNumber,
                VerificationToken = verificationToken,
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddHours(24),
                IsVerified = false
            };

            await _pendingRegistrationRepository.AddAsync(pendingRegistration);
            await _pendingRegistrationRepository.SaveChangesAsync();

            // Gửi email verify
            var emailSent = await _emailService.SendVerificationEmailAsync(
                dto.Email, 
                dto.Name, 
                verificationToken
            );

            if (!emailSent)
            {
                // Nếu không gửi được email, xóa pending registration
                await _pendingRegistrationRepository.RemoveAsync(pendingRegistration);
                await _pendingRegistrationRepository.SaveChangesAsync();
                return (false, new[] { "Không thể gửi email xác thực. Vui lòng thử lại sau." });
            }

            return (true, Array.Empty<string>());
        }

        public async Task<AuthResponseDto?> LoginAsync(LoginDto dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.EmailorUserName);
            if (user == null)
            {
                user = await _userManager.FindByNameAsync(dto.EmailorUserName);
            }
            if(user == null || !await _userManager.CheckPasswordAsync(user, dto.Password))
            return null;

            // Kiểm tra email đã được verify chưa
            if (!user.EmailConfirmed)
            {
                // Có thể throw exception hoặc return null với message
                // Tạm thời vẫn cho login, nhưng có thể thêm check sau
            }

            return await GenerateJwtTokenAsync(user);
        }

        public async Task<(bool Success, string Message)> VerifyEmailAsync(string token)
        {
            var pendingRegistration = await _pendingRegistrationRepository.GetByTokenAsync(token);

            if (pendingRegistration == null)
            {
                return (false, "Token không hợp lệ hoặc đã được sử dụng");
            }

            if (pendingRegistration.ExpiresAt < DateTime.UtcNow)
            {
                await _pendingRegistrationRepository.RemoveAsync(pendingRegistration);
                await _pendingRegistrationRepository.SaveChangesAsync();
                return (false, "Token đã hết hạn. Vui lòng đăng ký lại.");
            }

            // Kiểm tra email đã tồn tại chưa
            var existingUser = await _userManager.FindByEmailAsync(pendingRegistration.Email);
            if (existingUser != null)
            {
                await _pendingRegistrationRepository.RemoveAsync(pendingRegistration);
                await _pendingRegistrationRepository.SaveChangesAsync();
                return (false, "Email này đã được sử dụng");
            }

            // Tạo user mới - cần lưu password hash trực tiếp vì đã hash rồi
            var user = new User
            {
                UserName = pendingRegistration.UserName,
                Email = pendingRegistration.Email,
                FullName = pendingRegistration.Name,
                PhoneNumber = pendingRegistration.PhoneNumber,
                EmailConfirmed = true, // Đánh dấu email đã được verify
                PasswordHash = pendingRegistration.PasswordHash // Set password hash trực tiếp
            };

            // Tạo user mà không set password (vì đã set PasswordHash)
            var result = await _userManager.CreateAsync(user);
            if (!result.Succeeded)
            {
                return (false, string.Join(", ", result.Errors.Select(e => e.Description)));
            }

            await _userManager.AddToRoleAsync(user, "User");

            // Đánh dấu pending registration đã được verify và xóa
            pendingRegistration.IsVerified = true;
            await _pendingRegistrationRepository.RemoveAsync(pendingRegistration);
            await _pendingRegistrationRepository.SaveChangesAsync();

            return (true, "Email đã được xác thực thành công. Bạn có thể đăng nhập ngay bây giờ.");
        }
        public async Task<(bool Success, IEnumerable<string> Errors)> ChangeRoleAsync(ChangeRoleDto dto)
        {
            var user = await _userManager.FindByIdAsync(dto.UserId);
            if (user == null)
                return (false, new[] { "User not found" });

            var currentRoles = await _userManager.GetRolesAsync(user);
            await _userManager.RemoveFromRolesAsync(user, currentRoles);

            var result = await _userManager.AddToRoleAsync(user, dto.NewRole);
            if (!result.Succeeded)
                return (false, result.Errors.Select(e => e.Description));

            return (true, Array.Empty<string>());
        }
        private async Task<AuthResponseDto> GenerateJwtTokenAsync(User user)
        {
            var roles = await _userManager.GetRolesAsync(user);

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id),
                new Claim(JwtRegisteredClaimNames.UniqueName, user.UserName ?? ""),
                new Claim(JwtRegisteredClaimNames.Email, user.Email ?? ""),
                // Bổ sung claim UnitId
                new Claim("unitid", user.UnitId ?? "")
            };

            claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(Convert.ToDouble(_config["Jwt:ExpireMinutes"] ?? "60")),
                signingCredentials: creds
            );
        

            return new AuthResponseDto
            {
                UserId = user.Id,
                UserName = user.UserName ?? "",
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                Roles = roles.ToList()
            };
        }
    }
}
