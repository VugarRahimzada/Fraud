using Fraud.Core.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fraud.Core.FraudDetection.Models
{
    public sealed class TransactionHistoryItem
    {
        public required int Id { get; init; }
        public required int FromCardId { get; init; }
        public required int ToCardId { get; init; }
        public required int ToUserId { get; init; }
        public required decimal Amount { get; init; }
        public required TransactionStatus Status { get; init; }
        public required DateTime CreateDate { get; init; }
        public DateTime? CompletedAt { get; init; }
        public required bool IsSelfTransfer { get; init; }
    }
}
