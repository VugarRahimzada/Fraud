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
    public class VelocityRuleTests
    {
        private static VelocityRule CreateRule(FraudDetectionOptions options) =>
            new(TestHelpers.CreateOptions(options), TestHelpers.CreateClassifier(options));

        [Fact]
        public async Task FourTransactionsInFiveMinutes_DoesNotTrigger()
        {
            var now = DateTime.UtcNow;
            var options = new FraudDetectionOptions
            {
                Velocity = new() { Windows = { new() { TimeWindowMinutes = 5, MaxTransactionCount = 5, RiskScore = 30 } } }
            };

            var recentWindow = Enumerable.Range(0, 4)
                .Select(i => TestHelpers.HistoryItem(100, now.AddMinutes(-i)))
                .ToList();

            var context = TestHelpers.BuildContext(amount: 100, utcNow: now, recentWindow: recentWindow);

            var result = await CreateRule(options).EvaluateAsync(context);

            // 4 tarixi + 1 cari = 5, limit 5-dir (>5 lazımdır) -> trigger olmamalıdır
            Assert.False(result.Triggered);
        }

        [Fact]
        public async Task SixTransactionsInFiveMinutes_Triggers()
        {
            var now = DateTime.UtcNow;
            var options = new FraudDetectionOptions
            {
                Velocity = new() { Windows = { new() { TimeWindowMinutes = 5, MaxTransactionCount = 5, RiskScore = 30 } } }
            };

            var recentWindow = Enumerable.Range(0, 5)
                .Select(i => TestHelpers.HistoryItem(100, now.AddMinutes(-i)))
                .ToList();

            var context = TestHelpers.BuildContext(amount: 100, utcNow: now, recentWindow: recentWindow);

            var result = await CreateRule(options).EvaluateAsync(context);

            // 5 tarixi + 1 cari = 6 > 5 -> trigger
            Assert.True(result.Triggered);
            Assert.Equal(30m, result.RiskScore);
        }
    }
}
