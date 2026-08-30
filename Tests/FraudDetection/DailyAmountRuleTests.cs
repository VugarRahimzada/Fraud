using Fraud.Core.FraudDetection.Models;
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
    public class DailyAmountRuleTests
    {
        [Fact]
        public async Task TodayTotal19000_Plus2000_ExceedsLimit20000_Triggers()
        {
            var now = DateTime.UtcNow;
            var options = new FraudDetectionOptions { DailyAmount = new() { DailyLimit = 20000, RiskScore = 25 } };
            var rule = new DailyAmountRule(TestHelpers.CreateOptions(options), TestHelpers.CreateClassifier(options));

            var todayApproved = new List<TransactionHistoryItem>
        {
            TestHelpers.HistoryItem(19000, now.AddHours(-1))
        };

            var context = TestHelpers.BuildContext(amount: 2000, utcNow: now, todayApproved: todayApproved);

            var result = await rule.EvaluateAsync(context);

            Assert.True(result.Triggered);
            Assert.Equal(25m, result.RiskScore);
        }
    }
}
