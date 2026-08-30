using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fraud.Core.Enum
{
    public enum TransactionStatus
    {
        Pending = 0,
        UnderReview = 1,
        Approved = 2,
        Completed = 3,
        Blocked = 4,
        Rejected = 5
    }
}
