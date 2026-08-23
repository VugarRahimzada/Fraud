//using Fraud.Core.Entities;
//using Fraud.Core.Interfaces;
//using Fraud.DTO.DTOs;
//using Fraud.Service.Interfaces;

//namespace Fraud.Service
//{
//    public class UserService : IUserService
//    {
//        private readonly IUserRepository _userRepository;

//        public UserService(IUserRepository userRepository)
//        {
//            _userRepository = userRepository;
//        }

//        public async Task<RegisterDto> CreateAsync(RegisterDto dto)
//        {
//            var user = new User
//            {
//                Name = dto.Name,
//                Surname = dto.Surname,
//                Email = dto.Email,
//                CountryCode = dto.CountryCode,
//                Password = dto.Password,
//                UniqueCode = Guid.NewGuid().ToString(),
//            };

//            await _userRepository.AddAsync(user);

//            return dto;
//        }

//        public async Task<List<UserDto>> GetAllAsync()
//        {
//            var users = await _userRepository.GetAllAsync();

//            return users
//                .Where(x => !x.IsDelete)
//                .Select(MapToDto)
//                .ToList();
//        }

//        public async Task<UserDto?> GetByIdAsync(int id)
//        {
//            var user = await _userRepository.GetByIdAsync(id);

//            if (user == null || user.IsDelete)
//                return null;

//            return MapToDto(user);
//        }

//        public async Task<bool> UpdateAsync(int id, RegisterDto dto)
//        {
//            var user = await _userRepository.GetByIdAsync(id);

//            if (user == null || user.IsDelete)
//                return false;

//            user.Name = dto.Name;
//            user.Surname = dto.Surname;
//            user.Email = dto.Email;
//            user.CountryCode = dto.CountryCode;

//            await _userRepository.UpdateAsync(user);

//            return true;
//        }

//        public async Task<bool> DeleteAsync(int id)
//        {
//            var user = await _userRepository.GetByIdAsync(id);

//            if (user == null || user.IsDelete)
//                return false;

//            await _userRepository.DeleteAsync(user);

//            return true;
//        }

//        private static UserDto MapToDto(User user)
//        {
//            return new UserDto
//            {
//                Name = user.Name,
//                Surname = user.Surname,
//                Email = user.Email,
//                UniqueCode = user.UniqueCode,
//                CountryCode = user.CountryCode,
//                Balance = user.Balance
//            };
//        }
//    }
//}