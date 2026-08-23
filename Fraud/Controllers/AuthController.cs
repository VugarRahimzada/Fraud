//using Fraud.DTO.DTOs;
//using Fraud.Service.Interfaces;
//using Microsoft.AspNetCore.Mvc;

//namespace Fraud.Controllers
//{
//    [Route("api/[controller]")]
//    [ApiController]
//    public class AuthController : ControllerBase
//    {
//        private readonly IUserService _userService;

//        public AuthController(IUserService userService)
//        {
//            _userService = userService;
//        }

//        [HttpPost("register")]
//        public async Task<IActionResult> Register(RegisterDto dto)
//        {
//            var user = await _userService.CreateAsync(dto);

//            return Ok(user);
//        }

//        [HttpPost("login")]
//        public IActionResult Login(LoginDto dto)
//        {
//            // Login məntiqini növbəti mərhələdə yazacağıq

//            return Ok(dto);
//        }

//        [HttpGet("getuser")]
//        public async Task<IActionResult> GetUsers()
//        {
//            var users = await _userService.GetAllAsync();

//            return Ok(users);
//        }
//    }
//}