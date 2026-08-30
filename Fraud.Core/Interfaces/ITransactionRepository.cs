using Fraud.Core.Entities;
using Fraud.Core.FraudDetection.Models;
using Microsoft.EntityFrameworkCore.Storage;

namespace Fraud.Core.Interfaces
{
    public interface ITransactionRepository : IRepository<Transaction>
    {
        Task<List<Transaction>> GetByCardIdAsync(int cardId, CancellationToken cancellationToken = default);
        Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default);
        Task<List<TransactionHistoryItem>> GetApprovedOutgoingHistoryAsync(int senderUserId, DateTime fromUtc, DateTime toUtc, int maxCount, CancellationToken ct = default);

        /// <summary>Verilmiş zaman pəncərəsindəki bütün (status-dan asılı olmayaraq) outgoing transaction-lar.</summary>
        Task<List<TransactionHistoryItem>> GetOutgoingTransactionsInWindowAsync(int senderUserId, DateTime fromUtc, DateTime toUtc, CancellationToken ct = default);

        /// <summary>Sender bu recipient-ə (istənilən kartına) əvvəllər approved transaction edibmi.</summary>
        Task<bool> HasPriorApprovedTransactionToRecipientAsync(int senderUserId, int recipientUserId, DateTime beforeUtc, CancellationToken ct = default);
    }
}
