using Fraud.Core.Enum;
using Fraud.Core.FraudDetection.Models;
using Fraud.Service.Services.FraudDetection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Tests.FraudDetection
{
public class RiskScoreAggregatorTests
{
    [Fact]
    public void NoTriggeredRules_ReturnsZero()
    {
        var results = new[] { FraudRuleResult.NotTriggered("A"), FraudRuleResult.NotTriggered("B") };
        Assert.Equal(0m, RiskScoreAggregator.Aggregate(results));
    }

    [Fact]
    public void MultipleTriggeredRules_UsesDiminishingWeights_NotNaiveSum()
    {
        var results = new[]
        {
            FraudRuleResult.Trigger("A", 40, "r", FraudSeverity.Medium),
            FraudRuleResult.Trigger("B", 20, "r", FraudSeverity.Low),
            FraudRuleResult.Trigger("C", 35, "r", FraudSeverity.Medium),
        };

        var score = RiskScoreAggregator.Aggregate(results);

        // Naiv cəm 95 olardı; diminishing aggregation bundan aşağı olmalıdır.
        Assert.True(score < 95m);
        Assert.True(score > 40m); // ən yüksək fərdi score-dan az olmamalıdır
    }

    [Fact]
    public void Score_NeverExceeds100()
    {
        var results = Enumerable.Range(0, 10)
            .Select(i => FraudRuleResult.Trigger($"R{i}", 50, "r", FraudSeverity.High));

        Assert.True(RiskScoreAggregator.Aggregate(results) <= 100m);
    }
}
}
