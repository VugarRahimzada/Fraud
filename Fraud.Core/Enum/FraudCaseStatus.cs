using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fraud.Core.Enum
{
    public enum FraudCaseStatus
    {
        Open = 0,
        UnderReview = 1,
        ConfirmedFraud = 2,
        Dismissed = 3
    }
}
