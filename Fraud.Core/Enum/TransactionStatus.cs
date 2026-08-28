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
        Approved = 1,
        Blocked = 2,
        Rejected = 3
    }
}
