using Fraud.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fraud.Core.Interfaces
{
    public interface ICardRepository : IRepository<Card>
    {
        Task<bool> CodeExistsAsync(int code, int? excludeId = null, CancellationToken cancellationToken = default);

    }
}
