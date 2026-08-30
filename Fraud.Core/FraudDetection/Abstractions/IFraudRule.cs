using Fraud.Core.FraudDetection.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fraud.Core.FraudDetection.Abstractions
{
    public interface IFraudRule
    {
        string RuleName { get; }

        Task<FraudRuleResult> EvaluateAsync(
            FraudEvaluationContext context,
            CancellationToken ct = default);
    }
}
