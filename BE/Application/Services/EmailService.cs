using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Configuration;
using WebApiProject.Application.IServices;

namespace WebApiProject.Application.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;
        private readonly string _smtpHost;
        private readonly int _smtpPort;
        private readonly string _smtpUsername;
        private readonly string _smtpPassword;
        private readonly string _fromEmail;
        private readonly string _fromName;
        private readonly string _baseUrl;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
            _smtpHost = _configuration["Email:SmtpHost"] ?? "smtp.gmail.com";
            _smtpPort = int.Parse(_configuration["Email:SmtpPort"] ?? "587");
            _smtpUsername = _configuration["Email:SmtpUsername"] ?? "";
            _smtpPassword = _configuration["Email:SmtpPassword"] ?? "";
            _fromEmail = _configuration["Email:FromEmail"] ?? "noreply@warehouse.com";
            _fromName = _configuration["Email:FromName"] ?? "Warehouse System";
            _baseUrl = _configuration["Email:BaseUrl"] ?? "http://localhost:3000";
        }

        public async Task<bool> SendVerificationEmailAsync(string email, string userName, string verificationToken)
        {
            // Nếu không có email config (development mode), log token ra console và return true
            if (string.IsNullOrEmpty(_smtpUsername) || string.IsNullOrEmpty(_smtpPassword))
            {
                var verificationLink = $"{_baseUrl}/verify-email?token={verificationToken}";
                Console.WriteLine("========================================");
                Console.WriteLine("EMAIL VERIFICATION (Development Mode)");
                Console.WriteLine("========================================");
                Console.WriteLine($"Email: {email}");
                Console.WriteLine($"UserName: {userName}");
                Console.WriteLine($"Verification Link: {verificationLink}");
                Console.WriteLine($"Token: {verificationToken}");
                Console.WriteLine("========================================");
                Console.WriteLine("Note: Email service is not configured.");
                Console.WriteLine("In production, please configure SMTP settings in appsettings.json");
                Console.WriteLine("========================================");
                return true; // Return true để cho phép đăng ký tiếp tục
            }

            try
            {
                var verificationLink = $"{_baseUrl}/verify-email?token={verificationToken}";

                var subject = "Xác thực email đăng ký";
                var body = $@"
                    <html>
                    <body style='font-family: Arial, sans-serif;'>
                        <h2>Xin chào {userName}!</h2>
                        <p>Cảm ơn bạn đã đăng ký tài khoản tại Warehouse System.</p>
                        <p>Vui lòng click vào link sau để xác thực email của bạn:</p>
                        <p><a href='{verificationLink}' style='background-color: #4CAF50; color: white; padding: 10px 20px; text-decoration: none; border-radius: 5px; display: inline-block;'>Xác thực email</a></p>
                        <p>Hoặc copy link sau vào trình duyệt:</p>
                        <p>{verificationLink}</p>
                        <p><strong>Lưu ý:</strong> Link này sẽ hết hạn sau 5 phút.</p>
                        <p>Nếu bạn không đăng ký tài khoản này, vui lòng bỏ qua email này.</p>
                        <br>
                        <p>Trân trọng,<br>Warehouse System</p>
                    </body>
                    </html>";

                using (var client = new SmtpClient(_smtpHost, _smtpPort))
                {
                    client.EnableSsl = true;
                    client.Credentials = new NetworkCredential(_smtpUsername, _smtpPassword);

                    var mailMessage = new MailMessage
                    {
                        From = new MailAddress(_fromEmail, _fromName),
                        Subject = subject,
                        Body = body,
                        IsBodyHtml = true
                    };

                    mailMessage.To.Add(email);

                    await client.SendMailAsync(mailMessage);
                    return true;
                }
            }
            catch (Exception ex)
            {
                // Log error (có thể dùng ILogger)
                Console.WriteLine($"Error sending email: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                
                // Trong development, nếu lỗi gửi email, vẫn log token ra console
                if (string.IsNullOrEmpty(_smtpUsername) || string.IsNullOrEmpty(_smtpPassword))
                {
                    var verificationLink = $"{_baseUrl}/verify-email?token={verificationToken}";
                    Console.WriteLine($"Verification Link (fallback): {verificationLink}");
                    return true;
                }
                
                return false;
            }
        }

        public async Task<bool> SendPasswordResetEmailAsync(string email, string userName, string resetCode)
        {
            // Nếu không có email config (development mode), log code ra console và return true
            if (string.IsNullOrEmpty(_smtpUsername) || string.IsNullOrEmpty(_smtpPassword))
            {
                Console.WriteLine("========================================");
                Console.WriteLine("PASSWORD RESET CODE (Development Mode)");
                Console.WriteLine("========================================");
                Console.WriteLine($"Email: {email}");
                Console.WriteLine($"UserName: {userName}");
                Console.WriteLine($"Reset Code: {resetCode}");
                Console.WriteLine("========================================");
                Console.WriteLine("Note: Email service is not configured.");
                Console.WriteLine("In production, please configure SMTP settings in appsettings.json");
                Console.WriteLine("========================================");
                return true;
            }

            try
            {
                var subject = "Mã đặt lại mật khẩu";
                var body = $@"
                    <html>
                    <body style='font-family: Arial, sans-serif;'>
                        <h2>Xin chào {userName}!</h2>
                        <p>Bạn đã yêu cầu đặt lại mật khẩu cho tài khoản tại Warehouse System.</p>
                        <p>Mã đặt lại mật khẩu của bạn là:</p>
                        <div style='background-color: #4A90E2; color: white; padding: 15px 30px; text-align: center; font-size: 24px; font-weight: bold; border-radius: 8px; display: inline-block; margin: 20px 0;'>
                            {resetCode}
                        </div>
                        <p><strong>Lưu ý:</strong> Mã này sẽ hết hạn sau 15 phút.</p>
                        <p>Nếu bạn không yêu cầu đặt lại mật khẩu, vui lòng bỏ qua email này.</p>
                        <br>
                        <p>Trân trọng,<br>Warehouse System</p>
                    </body>
                    </html>";

                using (var client = new SmtpClient(_smtpHost, _smtpPort))
                {
                    client.EnableSsl = true;
                    client.Credentials = new NetworkCredential(_smtpUsername, _smtpPassword);

                    var mailMessage = new MailMessage
                    {
                        From = new MailAddress(_fromEmail, _fromName),
                        Subject = subject,
                        Body = body,
                        IsBodyHtml = true
                    };

                    mailMessage.To.Add(email);

                    await client.SendMailAsync(mailMessage);
                    return true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sending password reset email: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                
                if (string.IsNullOrEmpty(_smtpUsername) || string.IsNullOrEmpty(_smtpPassword))
                {
                    Console.WriteLine($"Reset Code (fallback): {resetCode}");
                    return true;
                }
                
                return false;
            }
        }
    }
}

