using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using WMS.API.Data;
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

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var user = await _context.UserLogins
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.Username == request.Username);

            if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            {
                return Unauthorized(new { message = "Invalid Username or Password" });
            }

            user.LastLogin = DateTime.Now;
            await _context.SaveChangesAsync();

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"]!);
            var roleName = user.Role?.RoleName ?? "Employee";

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.Name, user.Username),
                    new Claim(ClaimTypes.Role, roleName),
                    new Claim("roleId", user.RoleId.ToString()),
                    new Claim("roleName", roleName)
                }),
                Expires = DateTime.UtcNow.AddHours(2),
                Issuer = _configuration["Jwt:Issuer"],
                Audience = _configuration["Jwt:Audience"],
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);

            return Ok(new
            {
                token = tokenHandler.WriteToken(token),
                username = user.Username,
                role = roleName,
                roleId = user.RoleId
            });
        }

        [HttpPost("setup-default-admin")]
        public async Task<IActionResult> SetupDefaultAdmin()
        {
            if (!await _context.Roles.AnyAsync(r => r.RoleName == "Admin"))
            {
                _context.Roles.Add(new Role { RoleName = "Admin", Description = "System Administrator" });
                await _context.SaveChangesAsync();
            }

            if (!await _context.Roles.AnyAsync(r => r.RoleName == "Employee"))
            {
                _context.Roles.Add(new Role { RoleName = "Employee", Description = "Standard Employee" });
                await _context.SaveChangesAsync();
            }

            if (!await _context.Roles.AnyAsync(r => r.RoleName == "Manager"))
            {
                _context.Roles.Add(new Role { RoleName = "Manager", Description = "Team Manager" });
                await _context.SaveChangesAsync();
            }

            var adminRole = await _context.Roles.FirstOrDefaultAsync(r => r.RoleName == "Admin");
            var existingAdmin = await _context.UserLogins.FirstOrDefaultAsync(u => u.Username == "admin");

            if (existingAdmin == null)
            {
                _context.UserLogins.Add(new UserLogin
                {
                    Username = "admin",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("admin123"),
                    RoleId = adminRole.RoleId
                });

                await _context.SaveChangesAsync();
                return Ok(new { message = "Created Admin + Employee + Manager roles. Username: admin | Password: admin123" });
            }
            else
            {
                existingAdmin.PasswordHash = BCrypt.Net.BCrypt.HashPassword("admin123");
                await _context.SaveChangesAsync();
                return Ok(new { message = "Updated existing Admin with secure hash. Username: admin | Password: admin123" });
            }
        }

        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
        {
            var user = await _context.UserLogins.FirstOrDefaultAsync(u => u.Username == request.Username);

            if (user == null || !BCrypt.Net.BCrypt.Verify(request.OldPassword, user.PasswordHash))
            {
                return BadRequest(new { message = "Invalid username or current password." });
            }

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Password successfully changed!" });
        }
    }
}
