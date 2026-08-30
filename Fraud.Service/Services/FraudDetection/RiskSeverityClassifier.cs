using Fraud.Core.Enum;
using Fraud.Core.FraudDetection.Abstractions;
using Fraud.Core.FraudDetection.Options;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fraud.Service.Services.FraudDetection
{
    public sealed class RiskSeverityClassifier : IRiskSeverityClassifier
    {
        private readonly IOptionsMonitor<FraudDetectionOptions> _options;

        public RiskSeverityClassifier(IOptionsMonitor<FraudDetectionOptions> options) => _options = options;

        public FraudSeverity Classify(decimal riskScore)
        {
            var t = _options.CurrentValue.RiskThresholds;

            if (riskScore >= t.Critical) return FraudSeverity.Critical;
            if (riskScore >= t.High) return FraudSeverity.High;
            if (riskScore >= t.Medium) return FraudSeverity.Medium;
            return FraudSeverity.Low;
        }
    }
}
