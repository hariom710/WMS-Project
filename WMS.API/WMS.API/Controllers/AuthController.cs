using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using WMS.API.Data;
using WMS.API.Models;
using WMS.Domain.Models;

namespace WMS.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly WMSDbContext _context;
        private readonly IConfiguration _configuration;

        public AuthController(WMSDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        // --- DTOs ---
        public class LoginRequest
        {
            public string Username { get; set; }
            public string Password { get; set; }
        }

        public class ChangePasswordRequest
        {
            public string Username { get; set; }
            public string OldPassword { get; set; }
            public string NewPassword { get; set; }
        }

        // --- ENDPOINTS ---

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            // 1. Check if user exists in the database
            var user = await _context.UserLogins.FirstOrDefaultAsync(u => u.Username == request.Username);

            // 2. SECURE: Verify the BCrypt Hashed Password
            if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            {
                return Unauthorized(new { message = "Invalid Username or Password" });
            }

            // 3. Update Last Login Time
            user.LastLogin = DateTime.Now;
            await _context.SaveChangesAsync();

            // 4. Generate JWT Token
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"]!);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.Name, user.Username),
                    new Claim(ClaimTypes.Role, user.RoleId.ToString())
                }),
                Expires = DateTime.UtcNow.AddHours(2), // Token valid for 2 hours
                Issuer = _configuration["Jwt:Issuer"],
                Audience = _configuration["Jwt:Audience"],
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);

            return Ok(new
            {
                token = tokenHandler.WriteToken(token),
                username = user.Username,
                roleId = user.RoleId
            });
        }

        [HttpPost("setup-default-admin")]
        public async Task<IActionResult> SetupDefaultAdmin()
        {
            // 1. Ensure an "Admin" role exists to prevent Foreign Key errors
            if (!await _context.Roles.AnyAsync(r => r.RoleName == "Admin"))
            {
                _context.Roles.Add(new Role { RoleName = "Admin", Description = "System Administrator" });
                await _context.SaveChangesAsync();
            }

            var adminRole = await _context.Roles.FirstOrDefaultAsync(r => r.RoleName == "Admin");

            // 2. Look for the existing admin
            var existingAdmin = await _context.UserLogins.FirstOrDefaultAsync(u => u.Username == "admin");

            if (existingAdmin == null)
            {
                // Create a brand new admin user
                _context.UserLogins.Add(new UserLogin
                {
                    Username = "admin",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("admin123"),
                    RoleId = adminRole.RoleId
                });

                await _context.SaveChangesAsync();
                return Ok(new { message = "Success! Created new Admin. Username: admin | Password: admin123" });
            }
            else
            {
                // FIX: Overwrite the old plain-text password with the new BCrypt hash!
                existingAdmin.PasswordHash = BCrypt.Net.BCrypt.HashPassword("admin123");
                await _context.SaveChangesAsync();

                return Ok(new { message = "Success! Updated existing Admin with secure hash. Username: admin | Password: admin123" });
            }
        }

        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
        {
            // Find the user
            var user = await _context.UserLogins.FirstOrDefaultAsync(u => u.Username == request.Username);

            // SECURE: Verify old password hash
            if (user == null || !BCrypt.Net.BCrypt.Verify(request.OldPassword, user.PasswordHash))
            {
                return BadRequest(new { message = "Invalid username or current password." });
            }

            // SECURE: Hash the new password
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Password successfully changed!" });
        }
    }
}