using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using WMS.Domain.Interfaces;
using WMS.Domain.Models;
using WMS.Infrastructure.Data;

namespace WMS.Infrastructure.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserLoginRepository _userLoginRepo;
        private readonly IRoleRepository _roleRepo;
        private readonly WMSDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly IActivityLogService _activityLog;

        public AuthService(IUserLoginRepository userLoginRepo, IRoleRepository roleRepo, WMSDbContext context, IConfiguration configuration, IActivityLogService activityLog)
        {
            _userLoginRepo = userLoginRepo;
            _roleRepo = roleRepo;
            _context = context;
            _configuration = configuration;
            _activityLog = activityLog;
        }

        public async Task<(bool success, string? token, string? username, string? role, int? roleId)> LoginAsync(string username, string password)
        {
            var user = await _userLoginRepo.GetByUsernameAsync(username);
            if (user == null || !BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
            {
                await _activityLog.LogAsync("Auth", 0, "LoginFailed",
                    $"Failed login attempt for username: {username}", username, null, null);
                return (false, null, null, null, null);
            }

            var roleName = user.Role?.RoleName ?? "Employee";
            var jwtKey = _configuration["Jwt:Key"] ?? "";
            if (string.IsNullOrEmpty(jwtKey)) jwtKey = Environment.GetEnvironmentVariable("WMS_JWT_KEY") ?? "";
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));

            var claims = new[]
            {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Role, roleName),
                new Claim("roleId", user.RoleId.ToString()),
                new Claim("roleName", roleName)
            };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddHours(2),
                Issuer = _configuration["Jwt:Issuer"],
                Audience = _configuration["Jwt:Audience"],
                SigningCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature)
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);

            await _activityLog.LogAsync("Auth", user.UserId, "Login",
                $"User {username} logged in successfully", username, roleName, null);

            return (true, tokenHandler.WriteToken(token), user.Username, roleName, user.RoleId);
        }

        public async Task<bool> ChangePasswordAsync(string username, string oldPassword, string newPassword)
        {
            var user = await _userLoginRepo.GetByUsernameAsync(username);
            if (user == null || !BCrypt.Net.BCrypt.Verify(oldPassword, user.PasswordHash))
                return false;

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(newPassword);
            await _context.SaveChangesAsync();

            await _activityLog.LogAsync("Auth", user.UserId, "ChangePassword",
                $"User {username} changed password", username, null, null);

            return true;
        }

        public async Task<(bool success, string message)> SetupDefaultAdminAsync()
        {
            var roles = new[] { ("Admin", "System Administrator"), ("Employee", "Standard Employee"), ("Manager", "Team Manager") };
            foreach (var (name, desc) in roles)
            {
                if (!await _roleRepo.AnyAsync())
                {
                    var existing = await _roleRepo.GetByNameAsync(name);
                    if (existing == null)
                        await _roleRepo.AddAsync(new Role { RoleName = name, Description = desc });
                }
            }

            var adminRole = await _roleRepo.GetByNameAsync("Admin");
            var existingAdmin = await _userLoginRepo.GetByUsernameAsync("admin");

            if (existingAdmin == null)
            {
                await _userLoginRepo.AddAsync(new UserLogin
                {
                    Username = "admin",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("admin123"),
                    RoleId = adminRole!.RoleId
                });
                await _activityLog.LogAsync("Auth", 0, "SetupAdmin",
                    "Created default admin account", "System", "System", null);
                return (true, "Created Admin + Employee + Manager roles. Username: admin | Password: admin123");
            }
            else
            {
                existingAdmin.PasswordHash = BCrypt.Net.BCrypt.HashPassword("admin123");
                await _context.SaveChangesAsync();
                await _activityLog.LogAsync("Auth", existingAdmin.UserId, "ResetAdmin",
                    "Reset admin password", "System", "System", null);
                return (true, "Updated existing Admin with secure hash. Username: admin | Password: admin123");
            }
        }
    }
}
