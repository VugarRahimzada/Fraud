using Fraud.DTO.DTOs;

namespace Fraud.Service.Interfaces
{
    public interface IUserService
    {
        Task<List<UserDto>> GetAllAsync();
        Task<UserDto?> GetByIdAsync(int id);
        Task<RegisterDto> CreateAsync(RegisterDto dto);
        Task<bool> UpdateAsync(int id, RegisterDto dto);
        Task<bool> DeleteAsync(int id);
    }
}