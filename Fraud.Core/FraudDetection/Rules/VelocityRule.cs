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
    public sealed class VelocityRule : FraudRuleBase
    {
        private readonly IOptionsMonitor<FraudDetectionOptions> _options;

        public VelocityRule(IOptionsMonitor<FraudDetectionOptions> options, IRiskSeverityClassifier classifier)
            : base(classifier) => _options = options;

        public override string RuleName => "Velocity";

        public override Task<FraudRuleResult> EvaluateAsync(FraudEvaluationContext context, CancellationToken ct = default)
        {
            var cfg = _options.CurrentValue.Velocity;
            if (cfg.Windows.Count == 0)
                return Task.FromResult(NotTriggered());

            // Bütün configured window-lar yoxlanılır və ən yüksək risk saxlanılır — qısa window
            // keçsə belə, uzun window-un pozulması eyni dərəcədə əhəmiyyətlidir.
            FraudRuleResult? worst = null;

            foreach (var window in cfg.Windows)
            {
                var windowStart = context.UtcNow.AddMinutes(-window.TimeWindowMinutes);

                // +1: hazırda yaradılan (hələ save olunmamış) transaction da hesaba qatılır.
                var countInWindow = context.RecentOutgoingTransactions
                    .Count(t => t.CreateDate >= windowStart) + 1;

                if (countInWindow <= window.MaxTransactionCount)
                    continue;

                var riskScore = context.IsSelfTransfer
                    ? Math.Round(window.RiskScore * 0.5m, 2)
                    : window.RiskScore;

                var reason = $"{countInWindow} transactions within {window.TimeWindowMinutes} minute(s), " +
                             $"exceeding limit of {window.MaxTransactionCount}.";

                var candidate = Trigger(riskScore, reason);
                if (worst is null || candidate.RiskScore > worst.RiskScore)
                    worst = candidate;
            }

            return Task.FromResult(worst ?? NotTriggered());
        }
    }
}
