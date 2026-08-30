using Fraud.Core.Common;
using Fraud.Core.Entities;
using Fraud.Core.Enum;
using Fraud.Core.FraudDetection.Models;
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
        public async Task<List<TransactionHistoryItem>> GetApprovedOutgoingHistoryAsync(
        int senderUserId, DateTime fromUtc, DateTime toUtc, int maxCount, CancellationToken ct = default)
        {
            return await _context.Transactions
                .AsNoTracking()
                .Where(t => t.FromCard.UserId == senderUserId
                         && t.Status == TransactionStatus.Approved
                         && t.CreateDate >= fromUtc
                         && t.CreateDate <= toUtc)
                .OrderByDescending(t => t.CreateDate)
                .Take(maxCount)
                .Select(t => new TransactionHistoryItem
                {
                    Id = t.Id,
                    FromCardId = t.FromCardId,
                    ToCardId = t.ToCardId,
                    ToUserId = t.ToCard.UserId,
                    Amount = t.Amount,
                    Status = t.Status,
                    CreateDate = t.CreateDate,
                    CompletedAt = t.CompletedAt,
                    IsSelfTransfer = t.IsSelfTransfer
                })
                .ToListAsync(ct);
        }

        public async Task<List<TransactionHistoryItem>> GetOutgoingTransactionsInWindowAsync(
            int senderUserId, DateTime fromUtc, DateTime toUtc, CancellationToken ct = default)
        {
            return await _context.Transactions
                .AsNoTracking()
                .Where(t => t.FromCard.UserId == senderUserId
                         && t.CreateDate >= fromUtc
                         && t.CreateDate <= toUtc)
                .Select(t => new TransactionHistoryItem
                {
                    Id = t.Id,
                    FromCardId = t.FromCardId,
                    ToCardId = t.ToCardId,
                    ToUserId = t.ToCard.UserId,
                    Amount = t.Amount,
                    Status = t.Status,
                    CreateDate = t.CreateDate,
                    CompletedAt = t.CompletedAt,
                    IsSelfTransfer = t.IsSelfTransfer
                })
                .ToListAsync(ct);
        }

        public async Task<bool> HasPriorApprovedTransactionToRecipientAsync(
            int senderUserId, int recipientUserId, DateTime beforeUtc, CancellationToken ct = default)
        {
            return await _context.Transactions
                .AsNoTracking()
                .AnyAsync(t => t.FromCard.UserId == senderUserId
                            && t.ToCard.UserId == recipientUserId
                            && t.Status == TransactionStatus.Approved
                            && t.CreateDate < beforeUtc, ct);
        }

    }
}
