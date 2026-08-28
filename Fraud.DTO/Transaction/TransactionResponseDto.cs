using Fraud.Core.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fraud.DTO.Transaction
{
    public class TransactionResponseDto
    {
        public int Id { get; set; }
        public int FromCardId { get; set; }
        public int ToCardId { get; set; }
        public decimal Amount { get; set; }
        public TransactionType Type { get; set; }
        public TransactionStatus Status { get; set; }
        public bool IsSelfTransfer { get; set; }
        public decimal? RiskScore { get; set; }
        public string? FailureReason { get; set; }
        public Guid? FraudCaseId { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime? CompletedAt { get; set; }
    }
}
