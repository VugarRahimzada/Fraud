using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fraud.Core.Exceptions
{
    public class CardNotFoundException : Exception
    {
        public CardNotFoundException(int cardId) : base($"Card '{cardId}' was not found.") { }
    }

    public class InsufficientBalanceException : Exception
    {
        public InsufficientBalanceException(int cardId) : base($"Card '{cardId}' has insufficient balance.") { }
    }

    public class UnauthorizedAccessException : Exception
    {
        public UnauthorizedAccessException(string? message = null) : base(message ?? "You cannot send money from another user's card."){}
    }
}
