using Fraud.DTO.Transaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fraud.Service.Interfaces
{
    public interface ITransactionService
    {
        Task<TransactionResponseDto> CreateTransactionAsync(CreateTransactionDto dto, CancellationToken ct = default);
        Task<TransactionResponseDto?> GetByIdAsync(int id, CancellationToken ct = default);
        Task<List<TransactionResponseDto>> GetByCardIdAsync(int cardId, CancellationToken ct = default);

    }
}
