using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fraud.Core.FraudDetection.Options
{
    public class FraudDetectionOptionsValidator :IValidateOptions<FraudDetectionOptions>
    {
        public ValidateOptionsResult Validate(string? name, FraudDetectionOptions options)
        {
            var errors = new List<string>();

            if (options.RiskThresholds.Medium >= options.RiskThresholds.High)
                errors.Add("RiskThresholds.Medium must be less than RiskThresholds.High.");

            if (options.RiskThresholds.High >= options.RiskThresholds.Critical)
                errors.Add("RiskThresholds.High must be less than RiskThresholds.Critical.");

            if (options.RiskThresholds.Critical > 100)
                errors.Add("RiskThresholds.Critical cannot exceed 100.");

            if (options.HistoryLookbackDays <= 0)
                errors.Add("HistoryLookbackDays must be positive.");

            if (options.HistoryMaxRecords <= 0)
                errors.Add("HistoryMaxRecords must be positive.");

            if (options.Velocity.Windows.Any(w => w.TimeWindowMinutes <= 0 || w.MaxTransactionCount <= 0))
                errors.Add("Every Velocity window must have a positive TimeWindowMinutes and MaxTransactionCount.");

            if (options.AmountDeviation.MinimumHistoryCount <= 0)
                errors.Add("AmountDeviation.MinimumHistoryCount must be positive.");

            return errors.Count > 0
                ? ValidateOptionsResult.Fail(errors)
                : ValidateOptionsResult.Success;
        }
    }
}
