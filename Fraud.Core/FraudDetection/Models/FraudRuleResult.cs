using Fraud.Core.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fraud.Core.FraudDetection.Models
{
    public sealed class FraudRuleResult
    {
        public required string RuleName { get; init; }
        public bool Triggered { get; init; }
        public decimal RiskScore { get; init; }
        public string? Reason { get; init; }
        public FraudSeverity Severity { get; init; }

        public static FraudRuleResult NotTriggered(string ruleName) => new()
        {
            RuleName = ruleName,
            Triggered = false,
            RiskScore = 0,
            Severity = FraudSeverity.Low
        };

        public static FraudRuleResult Trigger(
            string ruleName, decimal riskScore, string reason, FraudSeverity severity) => new()
            {
                RuleName = ruleName,
                Triggered = true,
                RiskScore = riskScore,
                Reason = reason,
                Severity = severity
            };
    }
}
