using Fraud.Core.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace Fraud.Core.Entities
{
    public class Transaction : BaseEntity
    {
        public int FromCardId { get; set; }
        public Card FromCard { get; set; } = null!;

        public int ToCardId { get; set; }
        public Card ToCard { get; set; } = null!;

        public decimal Amount { get; set; }

        public TransactionType Type { get; set; }

        public Fraud.Core.Enum.TransactionStatus Status { get; set; }

        public bool IsSelfTransfer { get; set; }

        public decimal? RiskScore { get; set; }
        public FraudSeverity? RiskLevel { get; set; }

        public DateTime? FraudEvaluatedAt { get; set; }

        public string? FailureReason { get; set; }

        public int? FraudCaseId { get; set; }
        public FraudCase? FraudCase { get; set; }

        public DateTime? CompletedAt { get; set; }
    }
}
