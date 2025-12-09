namespace WebApiProject.Application.IServices
{
    public interface IEmailService
    {
        Task<bool> SendVerificationEmailAsync(string email, string userName, string verificationToken);
    }
}

