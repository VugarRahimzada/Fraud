using Fraud.Core.FraudDetection.Abstractions;
using Fraud.Core.FraudDetection.Models;
using Fraud.Core.FraudDetection.Options;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fraud.Core.FraudDetection.Rules
{
    public sealed class DailyAmountRule : FraudRuleBase
    {
        private readonly IOptionsMonitor<FraudDetectionOptions> _options;

        public DailyAmountRule(IOptionsMonitor<FraudDetectionOptions> options, IRiskSeverityClassifier classifier)
            : base(classifier) => _options = options;

        public override string RuleName => "DailyAmount";

        public override Task<FraudRuleResult> EvaluateAsync(FraudEvaluationContext context, CancellationToken ct = default)
        {
            var cfg = _options.CurrentValue.DailyAmount;

            // Yalnız Approved outgoing transaction-lar - Rejected/Blocked daxil edilmir.
            var todayTotal = context.TodayApprovedOutgoingTransactions.Sum(t => t.Amount);
            var projectedTotal = todayTotal + context.Transaction.Amount;

            if (projectedTotal <= cfg.DailyLimit)
                return Task.FromResult(NotTriggered());

            var riskScore = context.IsSelfTransfer
                ? Math.Round(cfg.RiskScore * 0.3m, 2)
                : cfg.RiskScore;

            var reason = $"Projected daily outgoing total {projectedTotal:N2} exceeds daily limit {cfg.DailyLimit:N2}.";

            return Task.FromResult(Trigger(riskScore, reason));
        }
    }
}
