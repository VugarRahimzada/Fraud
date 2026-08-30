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
    public sealed class LargeAmountRule : FraudRuleBase
    {
        private readonly IOptionsMonitor<FraudDetectionOptions> _options;

        public LargeAmountRule(IOptionsMonitor<FraudDetectionOptions> options, IRiskSeverityClassifier classifier)
            : base(classifier) => _options = options;

        public override string RuleName => "LargeAmount";

        public override Task<FraudRuleResult> EvaluateAsync(FraudEvaluationContext context, CancellationToken ct = default)
        {
            var cfg = _options.CurrentValue.LargeAmount;
            var amount = context.Transaction.Amount;

            if (amount < cfg.Threshold)
                return Task.FromResult(NotTriggered());

            // Self-transfer tam ignore edilmir: account-takeover ssenarisində fraudster
            // pulu əvvəlcə eyni istifadəçinin kartları arasında köçürüb sonra cash-out edə bilər.
            // Amma risk 3-cü şəxsə köçürmədən aşağıdır, ona görə çəki azaldılır.
            var riskScore = context.IsSelfTransfer
                ? Math.Round(cfg.RiskScore * cfg.SelfTransferRiskMultiplier, 2)
                : cfg.RiskScore;

            var reason = context.IsSelfTransfer
                ? $"Self-transfer amount {amount:N2} exceeds large amount threshold {cfg.Threshold:N2}."
                : $"Transaction amount {amount:N2} exceeds large amount threshold {cfg.Threshold:N2}.";

            return Task.FromResult(Trigger(riskScore, reason));
        }
    }
}
