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
    public sealed class NewRecipientRule : FraudRuleBase
    {
        private readonly IOptionsMonitor<FraudDetectionOptions> _options;

        public NewRecipientRule(IOptionsMonitor<FraudDetectionOptions> options, IRiskSeverityClassifier classifier)
            : base(classifier) => _options = options;

        public override string RuleName => "NewRecipient";

        public override Task<FraudRuleResult> EvaluateAsync(FraudEvaluationContext context, CancellationToken ct = default)
        {
            // Self-transfer-də "recipient" anlayışı yoxdur.
            if (context.IsSelfTransfer)
                return Task.FromResult(NotTriggered());

            var cfg = _options.CurrentValue.NewRecipient;

            // Recipient-Card yox, Recipient-User əsasında yoxlanılıb (context bunu artıq
            // ToCard.UserId üzrə hesablayıb) — eyni istifadəçinin başqa kartına əvvəlki
            // transfer də "tanış recipient" sayılır.
            if (context.RecipientHasPriorApprovedTransaction)
                return Task.FromResult(NotTriggered());

            var senderHistoryCount = context.ApprovedOutgoingHistory.Count;
            var amount = context.Transaction.Amount;

            var isLowAmount = amount < cfg.MinimumAmount;
            var senderIsEstablished = senderHistoryCount >= cfg.MinimumSenderHistoryCount;

            // Kiçik məbləğ + təcrübəli sender -> avtomatik fraud sayılmır.
            if (isLowAmount && senderIsEstablished)
                return Task.FromResult(NotTriggered());

            var riskScore = cfg.RiskScore;
            if (amount >= cfg.LargeAmountThreshold)
                riskScore = Math.Round(riskScore * cfg.LargeAmountMultiplier, 2);

            var reason = senderIsEstablished
                ? $"First transfer to this recipient, amount {amount:N2}."
                : $"First transfer to this recipient, amount {amount:N2}, sender has limited transaction history ({senderHistoryCount}).";

            return Task.FromResult(Trigger(riskScore, reason));
        }
    }
}
