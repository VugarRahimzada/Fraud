using Fraud.Core.Enum;
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
    public sealed class FailedTransactionPatternRule : FraudRuleBase
    {
        private readonly IOptionsMonitor<FraudDetectionOptions> _options;

        private static readonly TransactionStatus[] FailureStatuses =
        {
        TransactionStatus.Rejected,
        TransactionStatus.Blocked
        // Gələcəkdə ayrıca "Failed" statusu əlavə olunarsa, buraya əlavə edin.
    };

        public FailedTransactionPatternRule(IOptionsMonitor<FraudDetectionOptions> options, IRiskSeverityClassifier classifier)
            : base(classifier) => _options = options;

        public override string RuleName => "FailedTransactionPattern";

        public override Task<FraudRuleResult> EvaluateAsync(FraudEvaluationContext context, CancellationToken ct = default)
        {
            var cfg = _options.CurrentValue.FailedTransactionPattern;
            var windowStart = context.UtcNow.AddMinutes(-cfg.TimeWindowMinutes);

            // Hazırkı (hələ Pending) transaction bura daxil deyil — yalnız keçmiş failed transaction-lar.
            var failedCount = context.RecentOutgoingTransactions
                .Count(t => t.CreateDate >= windowStart && FailureStatuses.Contains(t.Status));

            if (failedCount < cfg.MaxFailedTransactions)
                return Task.FromResult(NotTriggered());

            var reason = $"{failedCount} failed/blocked/rejected transactions within {cfg.TimeWindowMinutes} minute(s).";

            return Task.FromResult(Trigger(cfg.RiskScore, reason));
        }
    }
}
