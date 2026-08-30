using Fraud.Core.Entities;
using Fraud.Core.Enum;
using Fraud.Core.FraudDetection.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fraud.Service.Services.FraudDetection
{
    public sealed class FraudCaseFactory : IFraudCaseFactory
    {
        public FraudCase Create(Core.FraudDetection.Models.FraudEvaluationResult evaluation, Transaction transaction)
        {
            var triggeredRules = evaluation.RuleResults.Where(r => r.Triggered).ToList();

            var fraudCase = new FraudCase
            {
                Reason = evaluation.FailureReason ?? "Fraud evaluation flagged this transaction for review.",
                // NOTE: FraudCaseStatus enum-unuzda uyğun "Open" üzvünün adını yoxlayın.
                Status = FraudCaseStatus.Open
            };

            foreach (var rule in triggeredRules)
            {
                fraudCase.RuleResults.Add(new FraudCaseRuleResult
                {
                    RuleName = rule.RuleName,
                    RiskScore = rule.RiskScore,
                    Severity = rule.Severity,
                    Reason = rule.Reason,
                    Transaction = transaction
                });
            }

            return fraudCase;
        }
    }
}
