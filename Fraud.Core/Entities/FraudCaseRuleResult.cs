using Fraud.Core.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fraud.Core.Entities
{
    public class FraudCaseRuleResult : BaseEntity
    {
        public int FraudCaseId { get; set; }
        public FraudCase FraudCase { get; set; } = null!;
        public int TransactionId { get; set; }
        public Transaction Transaction { get; set; } = null!;
        public string RuleName { get; set; } = string.Empty;
        public decimal RiskScore { get; set; }
        public FraudSeverity Severity { get; set; }
        public string? Reason { get; set; }
    }
}
