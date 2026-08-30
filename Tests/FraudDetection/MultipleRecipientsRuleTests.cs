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
    public class MultipleRecipientsRuleTests
    {
        private static MultipleRecipientsRule CreateRule(FraudDetectionOptions options) =>
            new(TestHelpers.CreateOptions(options), TestHelpers.CreateClassifier(options));

        [Fact]
        public async Task FiveDistinctRecipients_DoesNotTrigger()
        {
            var now = DateTime.UtcNow;
            var options = new FraudDetectionOptions
            {
                MultipleRecipients = new() { TimeWindowMinutes = 10, MaxDistinctRecipients = 5, RiskScore = 35 }
            };

            var recentWindow = Enumerable.Range(10, 4) // 4 fərqli recipient (userId 10-13)
                .Select(uid => TestHelpers.HistoryItem(100, now.AddMinutes(-1), toUserId: uid))
                .ToList();

            var context = TestHelpers.BuildContext(amount: 100, utcNow: now, recipientUserId: 20, recentWindow: recentWindow);

            var result = await CreateRule(options).EvaluateAsync(context);

            // 4 tarixi + 1 cari (userId 20) = 5, limitə bərabərdir -> trigger olmamalıdır
            Assert.False(result.Triggered);
        }

        [Fact]
        public async Task SixDistinctRecipients_Triggers()
        {
            var now = DateTime.UtcNow;
            var options = new FraudDetectionOptions
            {
                MultipleRecipients = new() { TimeWindowMinutes = 10, MaxDistinctRecipients = 5, RiskScore = 35 }
            };

            var recentWindow = Enumerable.Range(10, 5) // 5 fərqli recipient
                .Select(uid => TestHelpers.HistoryItem(100, now.AddMinutes(-1), toUserId: uid))
                .ToList();

            var context = TestHelpers.BuildContext(amount: 100, utcNow: now, recipientUserId: 20, recentWindow: recentWindow);

            var result = await CreateRule(options).EvaluateAsync(context);

            Assert.True(result.Triggered);
        }

        [Fact]
        public async Task SelfTransfer_NeverTriggers()
        {
            var options = new FraudDetectionOptions();
            var context = TestHelpers.BuildContext(amount: 100, isSelfTransfer: true);

            var result = await CreateRule(options).EvaluateAsync(context);

            Assert.False(result.Triggered);
        }
    }
}
