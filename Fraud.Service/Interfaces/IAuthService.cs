using Fraud.DTO.Auth;
using Fraud.DTO.Card;

namespace Fraud.Service.Interfaces
{
    public interface IAuthService
    {
        Task<RegisterResponseDto> RegisterAsync(RegisterRequestDto request, CancellationToken cancellationToken = default);
        Task<LoginResponseDto> LoginAsync(LoginRequestDto request, CancellationToken cancellationToken = default);
        Task<MeResponseDto> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    }
}