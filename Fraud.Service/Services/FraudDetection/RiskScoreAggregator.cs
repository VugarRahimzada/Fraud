using Fraud.Core.FraudDetection.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fraud.Service.Services.FraudDetection
{
    public static class RiskScoreAggregator
    {
        private const decimal DecayFactor = 0.6m;

        public static decimal Aggregate(IEnumerable<FraudRuleResult> results)
        {
            var triggeredScores = results
                .Where(r => r.Triggered)
                .Select(r => r.RiskScore)
                .OrderByDescending(s => s)
                .ToList();

            if (triggeredScores.Count == 0)
                return 0m;

            var total = 0m;
            var weight = 1m;

            foreach (var score in triggeredScores)
            {
                total += score * weight;
                weight *= DecayFactor;
            }

            return Math.Min(100m, Math.Round(total, 2));
        }
    }
}
