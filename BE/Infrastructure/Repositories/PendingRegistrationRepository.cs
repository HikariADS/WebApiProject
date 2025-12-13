using Microsoft.EntityFrameworkCore;
using WebApiProject.Application.IRepositories;
using WebApiProject.Domain.Entities;
using WebApiProject.Infrastructure.Persistence;

namespace WebApiProject.Infrastructure.Repositories
{
    public class PendingRegistrationRepository : IPendingRegistrationRepository
    {
        private readonly AppDbContext _context;

        public PendingRegistrationRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<PendingRegistration?> GetByTokenAsync(string token)
        {
            return await _context.PendingRegistrations
                .FirstOrDefaultAsync(p => p.VerificationToken == token && !p.IsVerified);
        }

        public async Task<PendingRegistration?> GetByTokenIgnoreVerifiedAsync(string token)
        {
            return await _context.PendingRegistrations
                .FirstOrDefaultAsync(p => p.VerificationToken == token);
        }

        public async Task<PendingRegistration?> GetByEmailAsync(string email)
        {
            return await _context.PendingRegistrations
                .FirstOrDefaultAsync(p => p.Email == email && !p.IsVerified);
        }

        public async Task AddAsync(PendingRegistration registration)
        {
            await _context.PendingRegistrations.AddAsync(registration);
        }

        public async Task RemoveAsync(PendingRegistration registration)
        {
            _context.PendingRegistrations.Remove(registration);
            await Task.CompletedTask;
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}

