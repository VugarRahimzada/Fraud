using Fraud.Core.Entities;
using Fraud.Core.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Fraud.DataAccess.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _context;

        public UserRepository(AppDbContext context)
        {
            _context = context;
        }
        public async Task AddAsync(User user, CancellationToken cancellationToken = default)
        {
            await _context.AddAsync(user, cancellationToken);
        }

        public async Task<bool> ExistsByEmailAsync(string email, CancellationToken cancellationToken = default)
        {
            return await _context.Users.AnyAsync(u => u.Email == email, cancellationToken);
        }

        public async Task<bool> ExistsByUserCodeAsync(string userCode, CancellationToken cancellationToken = default)
        {
            return await _context.Users.AnyAsync(u => u.UserCode == userCode, cancellationToken);
        }

        public async Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Email == email, cancellationToken);
        }

        public async Task<User?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Id == id, cancellationToken);
        }

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return await _context.SaveChangesAsync(cancellationToken);
        }
    }
}