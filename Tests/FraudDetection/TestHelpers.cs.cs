using Fraud.Core.FraudDetection.Options;
using Fraud.Core.FraudDetection.Abstractions;
using Fraud.Core.FraudDetection.Models;
using Fraud.Core.Entities;
using Fraud.Core.FraudDetection.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Moq;
using Fraud.Core.Enum;



namespace Tests.FraudDetection
{
    internal static class TestHelpers
    {
        public static IOptionsMonitor<FraudDetectionOptions> CreateOptions(FraudDetectionOptions options)
        {
            var mock = new Mock<IOptionsMonitor<FraudDetectionOptions>>();
            mock.Setup(o => o.CurrentValue).Returns(options);
            return mock.Object;
        }

        public static IRiskSeverityClassifier CreateClassifier(FraudDetectionOptions options)
        {
            var mockMonitor = new Mock<IOptionsMonitor<FraudDetectionOptions>>();
            mockMonitor.Setup(o => o.CurrentValue).Returns(options);
            return new Fraud.Service.Services.FraudDetection.RiskSeverityClassifier(mockMonitor.Object);
        }

        public static FraudEvaluationContext BuildContext(
            decimal amount,
            bool isSelfTransfer = false,
            int senderUserId = 1,
            int recipientUserId = 2,
            DateTime? utcNow = null,
            List<TransactionHistoryItem>? approvedHistory = null,
            List<TransactionHistoryItem>? recentWindow = null,
            List<TransactionHistoryItem>? todayApproved = null,
            bool recipientHasPriorApproved = false)
        {
            var now = utcNow ?? DateTime.UtcNow;

            var fromCard = new Card { Id = 1, UserId = senderUserId, Balance = 100000 };
            var toCard = new Card { Id = 2, UserId = recipientUserId, Balance = 0 };

            var transaction = new Transaction
            {
                Amount = amount,
                IsSelfTransfer = isSelfTransfer,
                FromCard = fromCard,
                ToCard = toCard,
                FromCardId = fromCard.Id,
                ToCardId = toCard.Id,
                Status = TransactionStatus.Pending
            };

            return new FraudEvaluationContext
            {
                Transaction = transaction,
                FromCard = fromCard,
                ToCard = toCard,
                SenderUserId = senderUserId,
                RecipientUserId = recipientUserId,
                UtcNow = now,
                ApprovedOutgoingHistory = approvedHistory ?? new List<TransactionHistoryItem>(),
                RecentOutgoingTransactions = recentWindow ?? new List<TransactionHistoryItem>(),
                TodayApprovedOutgoingTransactions = todayApproved ?? new List<TransactionHistoryItem>(),
                RecipientHasPriorApprovedTransaction = recipientHasPriorApproved
            };
        }

        public static TransactionHistoryItem HistoryItem(
            decimal amount, DateTime createDate, TransactionStatus status = TransactionStatus.Approved,
            bool isSelfTransfer = false, int toUserId = 2) => new()
            {
                Id = Random.Shared.Next(1, 1_000_000),
                FromCardId = 1,
                ToCardId = 2,
                ToUserId = toUserId,
                Amount = amount,
                Status = status,
                CreateDate = createDate,
                IsSelfTransfer = isSelfTransfer
            };
    }
}
