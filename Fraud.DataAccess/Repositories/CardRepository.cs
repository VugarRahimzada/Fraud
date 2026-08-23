using Fraud.Core.Entities;
using Fraud.Core.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fraud.DataAccess.Repositories
{
    public class CardRepository : Repository<Card>, ICardRepository
    {
        public CardRepository(AppDbContext context) : base(context) { }

        public async Task<bool> CodeExistsAsync(int code, int? excludeId = null, CancellationToken cancellationToken = default)
        {
            return await _dbSet.AnyAsync(
                x => x.Code == code && (excludeId == null || x.Id != excludeId),
                cancellationToken);
        }
    }
}
