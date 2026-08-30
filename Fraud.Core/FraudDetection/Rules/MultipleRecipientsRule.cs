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

    public sealed class MultipleRecipientsRule : FraudRuleBase
    {
        private readonly IOptionsMonitor<FraudDetectionOptions> _options;

        public MultipleRecipientsRule(IOptionsMonitor<FraudDetectionOptions> options, IRiskSeverityClassifier classifier)
            : base(classifier) => _options = options;

        public override string RuleName => "MultipleRecipients";

        public override Task<FraudRuleResult> EvaluateAsync(FraudEvaluationContext context, CancellationToken ct = default)
        {
            if (context.IsSelfTransfer)
                return Task.FromResult(NotTriggered());

            var cfg = _options.CurrentValue.MultipleRecipients;
            var windowStart = context.UtcNow.AddMinutes(-cfg.TimeWindowMinutes);

            // Uniqueness UserId əsasında - eyni recipient-in bir neçə kartı tək recipient sayılır.
            var distinctRecipients = context.RecentOutgoingTransactions
                .Where(t => t.CreateDate >= windowStart && !t.IsSelfTransfer)
                .Select(t => t.ToUserId)
                .ToHashSet();

            distinctRecipients.Add(context.RecipientUserId); // current transaction də daxil

            if (distinctRecipients.Count <= cfg.MaxDistinctRecipients)
                return Task.FromResult(NotTriggered());

            var reason = $"{distinctRecipients.Count} distinct recipients within {cfg.TimeWindowMinutes} minute(s), " +
                         $"exceeding limit of {cfg.MaxDistinctRecipients}.";

            return Task.FromResult(Trigger(cfg.RiskScore, reason));
        }
    }
}
