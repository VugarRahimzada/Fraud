using Fraud.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fraud.Service
{
    public interface IFraudDetectionEngine
    {
        Task<FraudEvaluationResult> EvaluateAsync(Transaction transaction, CancellationToken ct = default);
    }

    /// <summary>
    /// Result shape a real engine will eventually fill in with actual rule output.
    /// Defined now so the contract is stable when the real engine arrives.
    /// </summary>
    public class FraudEvaluationResult
    {
        public required bool Approved { get; init; }
        public decimal? RiskScore { get; init; }
        public string? FailureReason { get; init; }

        /// <summary>Non-null only when the engine wants a FraudCase created/attached.</summary>
        public string? FraudCaseReason { get; init; }
    }

    /// <summary>
    /// Current stand-in implementation: approves everything, sets no risk score,
    /// creates no fraud case. This is intentionally NOT fraud logic — it is a
    /// placeholder that satisfies requirement #10 (Approved by default, no fake
    /// fraud logic). Registered as the DI default until a real engine exists.
    /// </summary>
    public class AutoApproveFraudEngine : IFraudDetectionEngine
    {
        public Task<FraudEvaluationResult> EvaluateAsync(Transaction transaction, CancellationToken ct = default)
        {
            return Task.FromResult(new FraudEvaluationResult
            {
                Approved = true,
                RiskScore = null,
                FailureReason = null,
                FraudCaseReason = null
            });
        }
    }
}
