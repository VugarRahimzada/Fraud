using Fraud.Core.Entities;
using Microsoft.EntityFrameworkCore.Storage;

namespace Fraud.Core.Interfaces
{
    public interface ITransactionRepository : IRepository<Transaction>
    {
        Task<List<Transaction>> GetByCardIdAsync(int cardId, CancellationToken cancellationToken = default);
        Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default);
    }
}
