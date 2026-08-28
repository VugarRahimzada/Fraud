using Fraud.Core.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fraud.DTO.Transaction
{
    public class CreateTransactionDto
    {
        public int FromCardId { get; set; }
        public int ToCardId { get; set; }
        public decimal Amount { get; set; }
        public TransactionType Type { get; set; }
    }
}
