using Fraud.Core.Common;
using Fraud.Core.Entities;
using Fraud.DTO.Card;

namespace Fraud.Service.Interfaces
{
    public interface ICardService
    {
        Task<PagedResult<CardDto>> GetAllAsync(PaginationParams paginationParams, CancellationToken cancellationToken = default);
        Task<CardDto> GetByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<CardDto> CreateAsync(CreateCardDto dto, CancellationToken cancellationToken = default);
        Task<CardDto> UpdateAsync(int id, UpdateCardDto dto, CancellationToken cancellationToken = default);
        Task DeleteAsync(int id, CancellationToken cancellationToken = default);
    }
}