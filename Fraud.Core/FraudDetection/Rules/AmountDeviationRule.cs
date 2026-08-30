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
    public sealed class AmountDeviationRule : FraudRuleBase
    {
        private readonly IOptionsMonitor<FraudDetectionOptions> _options;

        public AmountDeviationRule(IOptionsMonitor<FraudDetectionOptions> options, IRiskSeverityClassifier classifier)
            : base(classifier) => _options = options;

        public override string RuleName => "AmountDeviation";

        public override Task<FraudRuleResult> EvaluateAsync(FraudEvaluationContext context, CancellationToken ct = default)
        {
            var cfg = _options.CurrentValue.AmountDeviation;

            // Self-transfer-lər normal xərcləmə pattern-inin bir hissəsi deyil, baseline-dan çıxarılır.
            var amounts = context.ApprovedOutgoingHistory
                .Where(h => !h.IsSelfTransfer)
                .Select(h => h.Amount)
                .ToList();

            if (amounts.Count < cfg.MinimumHistoryCount)
                return Task.FromResult(NotTriggered());

            var average = amounts.Average();
            var variance = amounts.Sum(a => (a - average) * (a - average)) / amounts.Count;
            var stdDev = (decimal)Math.Sqrt((double)variance);

            // Degenerate distribution (bütün tarixi məbləğlər eynidir) — division/false-positive qorunması.
            if (stdDev <= 0)
                return Task.FromResult(NotTriggered());

            var threshold = average + (cfg.StandardDeviationMultiplier * stdDev);
            var amount = context.Transaction.Amount;

            if (amount <= threshold)
                return Task.FromResult(NotTriggered());

            var riskScore = context.IsSelfTransfer
                ? Math.Round(cfg.RiskScore * 0.3m, 2)
                : cfg.RiskScore;

            var reason = $"Amount {amount:N2} deviates significantly from user's average {average:N2} " +
                         $"(std dev {stdDev:N2}, threshold {threshold:N2}).";

            return Task.FromResult(Trigger(riskScore, reason));
        }
    }
}
