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
    public class LargeAmountRuleTests
    {
        private static LargeAmountRule CreateRule(FraudDetectionOptions options) =>
            new(TestHelpers.CreateOptions(options), TestHelpers.CreateClassifier(options));

        [Fact]
        public async Task AmountBelowThreshold_DoesNotTrigger()
        {
            var options = new FraudDetectionOptions { LargeAmount = new() { Threshold = 10000, RiskScore = 40 } };
            var rule = CreateRule(options);
            var context = TestHelpers.BuildContext(amount: 9999);

            var result = await rule.EvaluateAsync(context);

            Assert.False(result.Triggered);
        }

        [Fact]
        public async Task AmountAtThreshold_Triggers()
        {
            var options = new FraudDetectionOptions { LargeAmount = new() { Threshold = 10000, RiskScore = 40 } };
            var rule = CreateRule(options);
            var context = TestHelpers.BuildContext(amount: 10000);

            var result = await rule.EvaluateAsync(context);

            Assert.True(result.Triggered);
            Assert.Equal(40m, result.RiskScore);
        }

        [Fact]
        public async Task SelfTransfer_TriggersWithReducedRisk()
        {
            var options = new FraudDetectionOptions
            {
                LargeAmount = new() { Threshold = 10000, RiskScore = 40, SelfTransferRiskMultiplier = 0.3m }
            };
            var rule = CreateRule(options);
            var context = TestHelpers.BuildContext(amount: 15000, isSelfTransfer: true);

            var result = await rule.EvaluateAsync(context);

            Assert.True(result.Triggered);
            Assert.Equal(12m, result.RiskScore); // 40 * 0.3
        }
    }
}
