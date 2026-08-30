using Fraud.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fraud.Core.FraudDetection.Models
{
    public sealed class FraudEvaluationContext
    {
        public required Transaction Transaction { get; init; }
        public required Card FromCard { get; init; }
        public required Card ToCard { get; init; }

        public required int SenderUserId { get; init; }
        public required int RecipientUserId { get; init; }

        public required DateTime UtcNow { get; init; }

        /// <summary>Sender-in approved outgoing tarixçəsi (HistoryLookbackDays / HistoryMaxRecords ilə bounded).
        /// AmountDeviation, BehavioralAnomaly və DailyAmount (in-memory filter ilə) tərəfindən istifadə olunur.</summary>
        public required IReadOnlyList<TransactionHistoryItem> ApprovedOutgoingHistory { get; init; }

        /// <summary>Son qısa zaman pəncərəsi (bütün rule-ların ən böyük window-u) daxilindəki BÜTÜN
        /// (status-dan asılı olmayaraq) outgoing transaction-lar. Velocity, MultipleRecipients,
        /// FailedTransactionPattern və BehavioralAnomaly-frequency bu list-i in-memory filtrləyir.</summary>
        public required IReadOnlyList<TransactionHistoryItem> RecentOutgoingTransactions { get; init; }

        /// <summary>Bugünkü (UTC) approved outgoing transaction-lar — ApprovedOutgoingHistory-dən derive olunub.</summary>
        public required IReadOnlyList<TransactionHistoryItem> TodayApprovedOutgoingTransactions { get; init; }

        /// <summary>Sender bu recipient-ə əvvəllər heç olmasa bir approved transaction edibmi
        /// (recipient-in bütün kartları üzrə, UserId əsasında).</summary>
        public required bool RecipientHasPriorApprovedTransaction { get; init; }

        public bool IsSelfTransfer => Transaction.IsSelfTransfer;
    }
}
