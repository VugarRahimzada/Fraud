using Fraud.Core.Enum;
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

    public class FailedTransactionPatternRuleTests
    {
        [Fact]
        public async Task ThreeFailedTransactionsInTenMinutes_Triggers()
        {
            var now = DateTime.UtcNow;
            var options = new FraudDetectionOptions
            {
                FailedTransactionPattern = new() { TimeWindowMinutes = 10, MaxFailedTransactions = 3, RiskScore = 40 }
            };
            var rule = new FailedTransactionPatternRule(TestHelpers.CreateOptions(options), TestHelpers.CreateClassifier(options));

            var recentWindow = new List<TransactionHistoryItem>
        {
            TestHelpers.HistoryItem(100, now.AddMinutes(-1), TransactionStatus.Blocked),
            TestHelpers.HistoryItem(100, now.AddMinutes(-2), TransactionStatus.Rejected),
            TestHelpers.HistoryItem(100, now.AddMinutes(-3), TransactionStatus.Blocked),
        };

            var context = TestHelpers.BuildContext(amount: 500, utcNow: now, recentWindow: recentWindow);

            var result = await rule.EvaluateAsync(context);

            Assert.True(result.Triggered);
            Assert.Equal(40m, result.RiskScore);
        }
    }
}
