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
        private readonly IPasswordResetCodeRepository _passwordResetCodeRepository;

        public AuthService(UserManager<User> userManager, IConfiguration config, IPendingRegistrationRepository pendingRegistrationRepository, IEmailService emailService, IPasswordResetCodeRepository passwordResetCodeRepository)
        {
            _userManager = userManager;
            _config = config;
            _pendingRegistrationRepository = pendingRegistrationRepository;
            _emailService = emailService;
            _passwordResetCodeRepository = passwordResetCodeRepository;
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
                ExpiresAt = DateTime.UtcNow.AddMinutes(5), // Token hết hạn sau 5 phút
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

        public async Task<(AuthResponseDto? Response, string? ErrorMessage)> LoginAsync(LoginDto dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.EmailorUserName);
            if (user == null)
            {
                user = await _userManager.FindByNameAsync(dto.EmailorUserName);
            }
            
            // Kiểm tra tài khoản có tồn tại không
            if (user == null)
            {
                return (null, "Tài khoản không tồn tại. Vui lòng kiểm tra lại email hoặc tên đăng nhập.");
            }

            // Kiểm tra mật khẩu
            if (!await _userManager.CheckPasswordAsync(user, dto.Password))
            {
                return (null, "Mật khẩu không đúng. Vui lòng thử lại.");
            }

            // Kiểm tra email đã được verify chưa
            if (!user.EmailConfirmed)
            {
                return (null, "Email chưa được xác thực. Vui lòng kiểm tra email và xác thực tài khoản trước khi đăng nhập.");
            }

            var response = await GenerateJwtTokenAsync(user);
            return (response, null);
        }

        public async Task<(bool Success, string Message)> VerifyEmailAsync(string token)
        {
            try
            {
                // Lấy pending registration (chỉ lấy những cái chưa verify)
                var pendingRegistration = await _pendingRegistrationRepository.GetByTokenAsync(token);

                if (pendingRegistration == null)
                {
                    // Nếu không tìm thấy token chưa verify, có thể token đã được sử dụng
                    // Kiểm tra xem có token này (kể cả đã verify) để lấy email
                    var anyPendingRegistration = await _pendingRegistrationRepository.GetByTokenIgnoreVerifiedAsync(token);
                    
                    if (anyPendingRegistration != null)
                    {
                        // Token đã tồn tại (có thể đã verify), kiểm tra xem user đã được tạo chưa
                        var verifiedUser = await _userManager.FindByEmailAsync(anyPendingRegistration.Email);
                        if (verifiedUser != null && verifiedUser.EmailConfirmed)
                        {
                            // User đã được tạo và email đã được verify, trả về thành công
                            return (true, "Email đã được xác thực thành công trước đó. Bạn có thể đăng nhập ngay bây giờ.");
                        }
                    }
                    
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

                // QUAN TRỌNG: Đánh dấu IsVerified = true NGAY LẬP TỨC để token không thể dùng lại
                // Ngay cả khi có request thứ 2 cùng lúc, nó sẽ không tìm thấy token này nữa
                // (vì GetByTokenAsync filter !p.IsVerified)
                pendingRegistration.IsVerified = true;
                await _pendingRegistrationRepository.SaveChangesAsync();

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
                    // Nếu tạo user thất bại, đặt lại IsVerified = false để có thể thử lại
                    pendingRegistration.IsVerified = false;
                    await _pendingRegistrationRepository.SaveChangesAsync();
                    return (false, string.Join(", ", result.Errors.Select(e => e.Description)));
                }

                await _userManager.AddToRoleAsync(user, "User");

                // Xóa pending registration sau khi đã tạo user thành công
                // Token đã được đánh dấu IsVerified = true, nên không thể dùng lại nữa
                await _pendingRegistrationRepository.RemoveAsync(pendingRegistration);
                await _pendingRegistrationRepository.SaveChangesAsync();

                return (true, "Email đã được xác thực thành công. Bạn có thể đăng nhập ngay bây giờ.");
            }
            catch (Exception ex)
            {
                return (false, $"Lỗi khi xác thực email: {ex.Message}");
            }
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
        public async Task<(bool Success, string Message)> ForgotPasswordAsync(ForgotPasswordDto dto)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(dto.Email);
                if (user == null)
                {
                    // Không tiết lộ email có tồn tại hay không vì lý do bảo mật
                    return (true, "Nếu email tồn tại, mã đặt lại mật khẩu đã được gửi đến email của bạn.");
                }

                // Xóa các mã cũ chưa sử dụng cho email này
                var existingCodes = await _passwordResetCodeRepository.GetByEmailAsync(dto.Email);
                if (existingCodes != null)
                {
                    existingCodes.IsUsed = true; // Đánh dấu là đã sử dụng
                    await _passwordResetCodeRepository.SaveChangesAsync();
                }

                // Tạo mã reset 6 chữ số
                var random = new Random();
                var resetCode = random.Next(100000, 999999).ToString();

                // Lưu mã reset
                var passwordResetCode = new PasswordResetCode
                {
                    Email = dto.Email,
                    Code = resetCode,
                    CreatedAt = DateTime.UtcNow,
                    ExpiresAt = DateTime.UtcNow.AddMinutes(15),
                    IsUsed = false
                };

                await _passwordResetCodeRepository.AddAsync(passwordResetCode);
                await _passwordResetCodeRepository.SaveChangesAsync();

                // Gửi email
                var emailSent = await _emailService.SendPasswordResetEmailAsync(
                    dto.Email,
                    user.FullName ?? user.UserName ?? "Người dùng",
                    resetCode
                );

                if (!emailSent)
                {
                    await _passwordResetCodeRepository.RemoveAsync(passwordResetCode);
                    await _passwordResetCodeRepository.SaveChangesAsync();
                    return (false, "Không thể gửi email. Vui lòng thử lại sau.");
                }

                return (true, "Nếu email tồn tại, mã đặt lại mật khẩu đã được gửi đến email của bạn.");
            }
            catch (Exception ex)
            {
                return (false, $"Lỗi khi xử lý yêu cầu: {ex.Message}");
            }
        }

        public async Task<(bool Success, string Message)> ResetPasswordAsync(ResetPasswordDto dto)
        {
            try
            {
                // Tìm mã reset hợp lệ
                var resetCode = await _passwordResetCodeRepository.GetByEmailAndCodeAsync(dto.Email, dto.Code);
                if (resetCode == null)
                {
                    return (false, "Mã xác thực không hợp lệ hoặc đã hết hạn.");
                }

                // Kiểm tra mã đã hết hạn chưa
                if (resetCode.ExpiresAt < DateTime.UtcNow)
                {
                    return (false, "Mã xác thực đã hết hạn. Vui lòng yêu cầu mã mới.");
                }

                // Kiểm tra mã đã được sử dụng chưa
                if (resetCode.IsUsed)
                {
                    return (false, "Mã xác thực đã được sử dụng. Vui lòng yêu cầu mã mới.");
                }

                // Tìm user
                var user = await _userManager.FindByEmailAsync(dto.Email);
                if (user == null)
                {
                    return (false, "Email không tồn tại trong hệ thống.");
                }

                // Reset password
                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                var result = await _userManager.ResetPasswordAsync(user, token, dto.NewPassword);

                if (!result.Succeeded)
                {
                    return (false, string.Join(", ", result.Errors.Select(e => e.Description)));
                }

                // Đánh dấu mã đã được sử dụng
                resetCode.IsUsed = true;
                await _passwordResetCodeRepository.SaveChangesAsync();

                return (true, "Đặt lại mật khẩu thành công. Bạn có thể đăng nhập với mật khẩu mới.");
            }
            catch (Exception ex)
            {
                return (false, $"Lỗi khi đặt lại mật khẩu: {ex.Message}");
            }
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
