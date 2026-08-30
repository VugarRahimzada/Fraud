using Fraud.Core.FraudDetection.Abstractions;
using Fraud.Core.FraudDetection.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fraud.Core.FraudDetection.Rules
{
    public abstract class FraudRuleBase : IFraudRule
    {
        private readonly IRiskSeverityClassifier _severityClassifier;

        protected FraudRuleBase(IRiskSeverityClassifier severityClassifier)
            => _severityClassifier = severityClassifier;

        public abstract string RuleName { get; }

        public abstract Task<FraudRuleResult> EvaluateAsync(
            FraudEvaluationContext context, CancellationToken ct = default);

        protected FraudRuleResult NotTriggered() => FraudRuleResult.NotTriggered(RuleName);

        protected FraudRuleResult Trigger(decimal riskScore, string reason) =>
            FraudRuleResult.Trigger(RuleName, riskScore, reason, _severityClassifier.Classify(riskScore));
    }
}
