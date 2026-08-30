using Fraud.Core.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fraud.Core.Entities
{
    public class FraudCase : BaseEntity
    {
        public string Reason { get; set; } = string.Empty;

        public FraudCaseStatus Status { get; set; }

        public ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();

        public ICollection<FraudCaseRuleResult> RuleResults { get; set; } = new List<FraudCaseRuleResult>();
        public DateTime? ResolvedAt { get; set; }

    }
}
