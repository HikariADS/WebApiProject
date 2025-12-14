using WebApiProject.Domain.Entities;

namespace WebApiProject.Application.IRepositories
{
    public interface IPasswordResetCodeRepository
    {
        Task<PasswordResetCode?> GetByEmailAndCodeAsync(string email, string code);
        Task<PasswordResetCode?> GetByEmailAsync(string email);
        Task AddAsync(PasswordResetCode resetCode);
        Task RemoveAsync(PasswordResetCode resetCode);
        Task SaveChangesAsync();
    }
}

