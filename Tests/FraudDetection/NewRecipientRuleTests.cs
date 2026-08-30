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
    public class NewRecipientRuleTests
    {
        private static NewRecipientRule CreateRule(FraudDetectionOptions options) =>
            new(TestHelpers.CreateOptions(options), TestHelpers.CreateClassifier(options));

        [Fact]
        public async Task ExistingRecipient_DoesNotTrigger()
        {
            var options = new FraudDetectionOptions();
            var context = TestHelpers.BuildContext(amount: 5000, recipientHasPriorApproved: true);

            var result = await CreateRule(options).EvaluateAsync(context);

            Assert.False(result.Triggered);
        }

        [Fact]
        public async Task FirstTimeRecipient_HighAmount_Triggers()
        {
            var options = new FraudDetectionOptions
            {
                NewRecipient = new()
                {
                    MinimumAmount = 1000,
                    MinimumSenderHistoryCount = 3,
                    RiskScore = 20,
                    LargeAmountThreshold = 5000,
                    LargeAmountMultiplier = 1.5m
                }
            };

            var context = TestHelpers.BuildContext(amount: 10000, recipientHasPriorApproved: false);

            var result = await CreateRule(options).EvaluateAsync(context);

            Assert.True(result.Triggered);
            Assert.Equal(30m, result.RiskScore); // 20 * 1.5
        }

        [Fact]
        public async Task SelfTransfer_NeverTriggers()
        {
            var options = new FraudDetectionOptions();
            var context = TestHelpers.BuildContext(amount: 50000, isSelfTransfer: true, recipientHasPriorApproved: false);

            var result = await CreateRule(options).EvaluateAsync(context);

            Assert.False(result.Triggered);
        }
    }
}
