using Microsoft.EntityFrameworkCore;
using WebApiProject.Application.IRepositories;
using WebApiProject.Domain.Entities;
using WebApiProject.Infrastructure.Persistence;

namespace WebApiProject.Infrastructure.Repositories
{
    public class PasswordResetCodeRepository : IPasswordResetCodeRepository
    {
        private readonly AppDbContext _context;

        public PasswordResetCodeRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<PasswordResetCode?> GetByEmailAndCodeAsync(string email, string code)
        {
            return await _context.PasswordResetCodes
                .FirstOrDefaultAsync(p => p.Email == email && p.Code == code && !p.IsUsed && p.ExpiresAt > DateTime.UtcNow);
        }

        public async Task<PasswordResetCode?> GetByEmailAsync(string email)
        {
            return await _context.PasswordResetCodes
                .Where(p => p.Email == email && !p.IsUsed && p.ExpiresAt > DateTime.UtcNow)
                .OrderByDescending(p => p.CreatedAt)
                .FirstOrDefaultAsync();
        }

        public async Task AddAsync(PasswordResetCode resetCode)
        {
            await _context.PasswordResetCodes.AddAsync(resetCode);
        }

        public async Task RemoveAsync(PasswordResetCode resetCode)
        {
            _context.PasswordResetCodes.Remove(resetCode);
            await Task.CompletedTask;
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}

