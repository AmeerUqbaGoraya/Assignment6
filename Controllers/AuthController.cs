using Microsoft.AspNetCore.Mvc;
using Assignment6.Models.DTOs;
using Assignment6.Services;

namespace Assignment6.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            var result = await _authService.LoginAsync(loginDto);
            if (result == null)
            {
                return BadRequest(new ApiResponseDto<object>
                {
                    Success = false,
                    Message = "Invalid email or password"
                });
            }

            return Ok(new ApiResponseDto<AuthResponseDto>
            {
                Success = true,
                Data = result,
                Message = "Login successful"
            });
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
        {
            var result = await _authService.RegisterAsync(registerDto);
            if (!result)
            {
                return BadRequest(new ApiResponseDto<object>
                {
                    Success = false,
                    Message = "Email already exists"
                });
            }

            return Ok(new ApiResponseDto<object>
            {
                Success = true,
                Message = "Registration successful"
            });
        }

        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto changePasswordDto)
        {
            // TODO: Get user ID from JWT token
            var userId = 1; // Placeholder

            var result = await _authService.ChangePasswordAsync(userId, changePasswordDto);
            if (!result)
            {
                return BadRequest(new ApiResponseDto<object>
                {
                    Success = false,
                    Message = "Current password is incorrect"
                });
            }

            return Ok(new ApiResponseDto<object>
            {
                Success = true,
                Message = "Password changed successfully"
            });
        }
    }
}
