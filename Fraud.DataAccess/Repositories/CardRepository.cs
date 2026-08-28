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

        public async Task<bool> CodeExistsAsync(string code, int? excludeId = null, CancellationToken cancellationToken = default)
        {
            return await _dbSet.AnyAsync(
                x => x.Code == code && (excludeId == null || x.Id != excludeId),
                cancellationToken);
        }

        public async Task<Card?> GetCardForUpdateAsync(int cardId, CancellationToken cancellationToken = default)
        {
            // Tracked (non-AsNoTracking) read: the entity stays attached so the
            // service's balance mutation is captured by SaveChangesAsync inside
            // the same DB transaction. Combined with the DB transaction + normal
            // optimistic concurrency (add a RowVersion column on Card if your
            // existing Card entity doesn't already have one) this prevents lost
            // updates under concurrent transfers.
            //return await _dbSet.Cards.FirstOrDefaultAsync(c => c.Id == cardId, cancellationToken);

            return await _dbSet.FirstOrDefaultAsync(c => c.Id == cardId, cancellationToken);
        }
    }
}
