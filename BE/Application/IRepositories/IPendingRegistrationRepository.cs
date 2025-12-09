using WebApiProject.Domain.Entities;

namespace WebApiProject.Application.IRepositories
{
    public interface IPendingRegistrationRepository
    {
        Task<PendingRegistration?> GetByTokenAsync(string token);
        Task<PendingRegistration?> GetByEmailAsync(string email);
        Task AddAsync(PendingRegistration registration);
        Task RemoveAsync(PendingRegistration registration);
        Task SaveChangesAsync();
    }
}

