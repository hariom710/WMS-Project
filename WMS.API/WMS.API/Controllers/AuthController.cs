using Microsoft.AspNetCore.Mvc;
using WMS.Application.DTOs;
using WMS.Domain.Interfaces;

namespace WMS.API.Controllers
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

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
        {
            var (success, token, username, role, roleId) = await _authService.LoginAsync(request.Username, request.Password);
            if (!success)
                return Unauthorized(new { message = "Invalid Username or Password" });

            return Ok(new { token, username, role, roleId });
        }

        [HttpPost("setup-default-admin")]
        public async Task<IActionResult> SetupDefaultAdmin()
        {
            var (success, message) = await _authService.SetupDefaultAdminAsync();
            return Ok(new { message });
        }

        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequestDto request)
        {
            var success = await _authService.ChangePasswordAsync(request.Username, request.OldPassword, request.NewPassword);
            if (!success)
                return BadRequest(new { message = "Invalid username or current password." });

            return Ok(new { message = "Password successfully changed!" });
        }
    }
}
