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
    public sealed class BehavioralAnomalyRule : FraudRuleBase
    {
        private readonly IOptionsMonitor<FraudDetectionOptions> _options;

        public BehavioralAnomalyRule(IOptionsMonitor<FraudDetectionOptions> options, IRiskSeverityClassifier classifier)
            : base(classifier) => _options = options;

        public override string RuleName => "BehavioralAnomaly";

        public override Task<FraudRuleResult> EvaluateAsync(FraudEvaluationContext context, CancellationToken ct = default)
        {
            var cfg = _options.CurrentValue.BehavioralAnomaly;
            var history = context.ApprovedOutgoingHistory.Where(h => !h.IsSelfTransfer).ToList();

            if (history.Count < cfg.MinimumHistoryCount)
                return Task.FromResult(NotTriggered());

            var triggeredReasons = new List<string>();
            decimal riskScore = 0;

            // --- Unusual hour ---
            // Hazırda UTC istifadə olunur. Saat çıxarma logikası GetHour() metoduna
            // izolyasiya olunub ki, gələcəkdə user timezone dəstəyi (məs. stored UTC offset)
            // əlavə edilərkən yalnız bu metod dəyişsin, qalan alqoritm toxunulmasın.
            var historicalHours = history.Select(h => GetHour(h.CreateDate)).ToList();
            var currentHour = GetHour(context.UtcNow);

            var closeHourCount = historicalHours.Count(h => IsCloseHour(h, currentHour, toleranceHours: 3));
            var hourRatio = (decimal)closeHourCount / historicalHours.Count;

            if (hourRatio < 0.1m) // tarixçənin 10%-dən azı bu saat ətrafında baş verib
            {
                riskScore += cfg.UnusualHourRiskScore;
                triggeredReasons.Add($"Unusual transaction hour ({currentHour:00}:00 UTC)");
            }

            // --- Unusual amount ---
            var average = history.Average(h => h.Amount);
            var variance = history.Sum(h => (h.Amount - average) * (h.Amount - average)) / history.Count;
            var stdDev = (decimal)Math.Sqrt((double)variance);

            if (stdDev > 0 && context.Transaction.Amount > average + (cfg.AmountStandardDeviationMultiplier * stdDev))
            {
                riskScore += cfg.UnusualAmountRiskScore;
                triggeredReasons.Add($"Amount significantly above user's normal behavior (avg {average:N2})");
            }

            // --- Unusual frequency ---
            var frequencyWindowStart = context.UtcNow.AddMinutes(-cfg.FrequencyWindowMinutes);
            var recentCount = context.RecentOutgoingTransactions.Count(t => t.CreateDate >= frequencyWindowStart) + 1;

            var oldestHistoryEntry = history.Min(h => h.CreateDate);
            var observedDays = Math.Max(1.0, (context.UtcNow - oldestHistoryEntry).TotalDays);
            var typicalDailyRate = history.Count / observedDays;
            var expectedInWindow = typicalDailyRate * (cfg.FrequencyWindowMinutes / 1440.0);

            if (expectedInWindow > 0 && recentCount > expectedInWindow * (double)cfg.FrequencyDeviationMultiplier)
            {
                riskScore += cfg.UnusualFrequencyRiskScore;
                triggeredReasons.Add("Transaction frequency significantly above user's normal pattern");
            }

            if (triggeredReasons.Count == 0)
                return Task.FromResult(NotTriggered());

            var combinedReason = string.Join(" AND ", triggeredReasons);
            return Task.FromResult(Trigger(Math.Min(riskScore, 100m), combinedReason));
        }

        private static int GetHour(DateTime utc) => utc.Hour;

        private static bool IsCloseHour(int a, int b, int toleranceHours)
        {
            var diff = Math.Abs(a - b);
            return Math.Min(diff, 24 - diff) <= toleranceHours;
        }
    }
}
