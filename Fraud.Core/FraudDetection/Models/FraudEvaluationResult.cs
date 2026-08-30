using Fraud.Core.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fraud.Core.FraudDetection.Models
{
    public sealed class FraudEvaluationResult
    {
        public required decimal RiskScore { get; init; }
        public required FraudSeverity Severity { get; init; }
        public required bool Approved { get; init; }
        public required bool RequiresFraudCase { get; init; }
        public string? FailureReason { get; init; }
        public required IReadOnlyCollection<FraudRuleResult> RuleResults { get; init; }
    }
}
