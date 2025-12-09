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
                        <p><strong>Lưu ý:</strong> Link này sẽ hết hạn sau 24 giờ.</p>
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
                return false;
            }
        }
    }
}

