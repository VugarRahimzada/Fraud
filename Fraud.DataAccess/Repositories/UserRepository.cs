using Fraud.Core.Entities;
using Fraud.Core.Interfaces;

namespace Fraud.DataAccess.Repositories
{
    public class UserRepository : Repository<User>, IUserRepository
    {
        public UserRepository(AppDbContext context)
            : base(context)
        {
        }
    }
}