using Fraud.Core.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fraud.Core.FraudDetection.Abstractions
{
    public interface IRiskSeverityClassifier
    {
        FraudSeverity Classify(decimal riskScore);
    }
}
