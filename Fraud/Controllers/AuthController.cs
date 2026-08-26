using Fraud.Core.Common;
using Fraud.Core.Entities;
using Fraud.Core.Exceptions;
using Fraud.DTO.Auth;
using Fraud.DTO.Card;
using Fraud.Service.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading;

namespace Fraud.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequestDto request, CancellationToken cancellationToken)
        {
            var result = await _authService.RegisterAsync(request, cancellationToken);

            return Ok(ApiResponse<RegisterResponseDto>.SuccessResponse(result, "Qeydiyyat uğurludur"));

        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto request, CancellationToken cancellationToken)
        {
            var result = await _authService.LoginAsync(request, cancellationToken);
            return Ok(ApiResponse<LoginResponseDto>.SuccessResponse(result, "Giriş uğurludur."));
        }

        [Authorize]
        [HttpGet("me")]
        public async Task<IActionResult> Me()
        {
            var userId = int.Parse(User.FindFirstValue("userId")!);
            var user = await _authService.GetByIdAsync(userId);

            if (user is null)
                throw new NotFoundException("İstifadəçi tapılmadı.");

            var dto = new MeResponseDto
            {
                Id = user.Id,
                UserCode = user.UserCode,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                Role = user.Role,
                Cards = user.Cards
            };

            return Ok(ApiResponse<MeResponseDto>.SuccessResponse(dto));
        }
    }
}