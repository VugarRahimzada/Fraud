using AutoMapper;
using FluentValidation;
using Fraud.Core.Common;
using Fraud.Core.Entities;
using Fraud.Core.Exceptions;
using Fraud.Core.Interfaces;
using Fraud.DataAccess.Repositories;
using Fraud.DTO.Auth;
using Fraud.DTO.Card;
using Fraud.DTO.DTOs;
using Fraud.Service.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading;


namespace Fraud.Service
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly IValidator<RegisterRequestDto> _registerValidator;
        private readonly IValidator<LoginRequestDto> _loginValidator;

        private readonly JwtSettings _jwtSettings;
        private readonly PasswordHasher<User> _passwordHasher = new();

        public AuthService(
            IUserRepository userRepository,
            IMapper mapper, 
            IOptions<JwtSettings> jwtOptions,
            IValidator<RegisterRequestDto> registerValidator,
            IValidator<LoginRequestDto> loginValidator)
        {
            _userRepository = userRepository;
            _mapper = mapper;
            _jwtSettings = jwtOptions.Value;
            _registerValidator = registerValidator;
            _loginValidator = loginValidator;
        }

        public async Task<RegisterResponseDto> RegisterAsync(RegisterRequestDto request, CancellationToken cancellationToken = default )
        {
            if (await _userRepository.ExistsByEmailAsync(request.Email))
                throw new ConflictException("Bu email artıq istifadə olunub.");


            var validationResult = await _registerValidator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
                throw new Fraud.Core.Exceptions.ValidationException(validationResult.Errors.Select(e => e.ErrorMessage));


            var user = _mapper.Map<User>(request);

            user = new User
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                Email = request.Email,
                Role = "User",
                UserCode = await GenerateUniqueUserCodeAsync(),
                CreateDate = DateTime.UtcNow,
                UpdateDate = DateTime.UtcNow,
            };
            user.PasswordHash = _passwordHasher.HashPassword(user, request.Password);

            await _userRepository.AddAsync(user, cancellationToken);
            await _userRepository.SaveChangesAsync(cancellationToken);

            return _mapper.Map<RegisterResponseDto>(user);

        }

        public async Task<LoginResponseDto> LoginAsync(LoginRequestDto request, CancellationToken cancellationToken = default)
        {
            var user = await _userRepository.GetByEmailAsync(request.Email);
            if (user is null)
                throw new BusinessException("Email və ya şifrə yanlışdır.");

            var validationResult = await _loginValidator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
                throw new Fraud.Core.Exceptions.ValidationException(validationResult.Errors.Select(e => e.ErrorMessage));

            var result = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, request.Password);

            if (result == PasswordVerificationResult.Failed)
                throw new BusinessException("Email və ya şifrə yanlışdır.");

            var token = GenerateToken(user);

            var response =  _mapper.Map<LoginResponseDto>(user);

            response.Token = token;

            return response;
        }


        private async Task<string> GenerateUniqueUserCodeAsync()
        {
            string code;
            bool exists;
            do
            {
                code = UserCodeGenerator.Generate();
                exists = await _userRepository.ExistsByUserCodeAsync(code);
            } while (exists);

            return code;
        }
        private string GenerateToken(User user)
        {
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim("userId", user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _jwtSettings.Issuer,
                audience: _jwtSettings.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(_jwtSettings.ExpirationMinutes),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public async Task<MeResponseDto> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            var user = await _userRepository.GetByIdAsync(id, cancellationToken) ?? throw new NotFoundException(nameof(User), id);

            return _mapper.Map<MeResponseDto>(user);
        }
    }
}