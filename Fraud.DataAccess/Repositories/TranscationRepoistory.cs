using Fraud.Core.Common;
using Fraud.Core.Entities;
using Fraud.Core.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Fraud.DataAccess.Repositories
{
    public class TranscationRepoistory : Repository<Transaction> , ITransactionRepository
    {
        public TranscationRepoistory(AppDbContext context) : base(context)
        {
        }

        public async Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default)
        {
            return await _context.Database.BeginTransactionAsync(cancellationToken);
        }

        public async Task<List<Transaction>> GetByCardIdAsync(int cardId, CancellationToken cancellationToken = default)
        {
            return await _dbSet.Where(t => t.FromCardId == cardId || t.ToCardId == cardId)
                               .OrderByDescending(t => t.CreateDate)
                               .ToListAsync(cancellationToken);
        }

    }
}
