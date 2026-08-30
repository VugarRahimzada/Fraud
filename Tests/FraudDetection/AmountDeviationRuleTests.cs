using Fraud.Core.FraudDetection.Options;
using Fraud.Core.FraudDetection.Rules;
using System;
using System.Collections.Generic;
using System.Linq;  
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Tests.FraudDetection
{
    public class AmountDeviationRuleTests
    {
        [Fact]
        public async Task Sudden10xTransaction_Triggers()
        {
            var now = DateTime.UtcNow;
            var options = new FraudDetectionOptions
            {
                AmountDeviation = new() { MinimumHistoryCount = 5, StandardDeviationMultiplier = 3, RiskScore = 30 }
            };
            var rule = new AmountDeviationRule(TestHelpers.CreateOptions(options), TestHelpers.CreateClassifier(options));

            var history = Enumerable.Range(0, 10)
                .Select(i => TestHelpers.HistoryItem(100 + i, now.AddDays(-i - 1)))
                .ToList(); // ~100-109 aralığında normal davranış

            var context = TestHelpers.BuildContext(amount: 1200, utcNow: now, approvedHistory: history);

            var result = await rule.EvaluateAsync(context);

            Assert.True(result.Triggered);
        }

        [Fact]
        public async Task InsufficientHistory_DoesNotTrigger()
        {
            var now = DateTime.UtcNow;
            var options = new FraudDetectionOptions
            {
                AmountDeviation = new() { MinimumHistoryCount = 5, StandardDeviationMultiplier = 3, RiskScore = 30 }
            };
            var rule = new AmountDeviationRule(TestHelpers.CreateOptions(options), TestHelpers.CreateClassifier(options));

            var history = Enumerable.Range(0, 2)
                .Select(i => TestHelpers.HistoryItem(100, now.AddDays(-i - 1)))
                .ToList();

            var context = TestHelpers.BuildContext(amount: 5000, utcNow: now, approvedHistory: history);

            var result = await rule.EvaluateAsync(context);

            Assert.False(result.Triggered);
        }
    }
}
