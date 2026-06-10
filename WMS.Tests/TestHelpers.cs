using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using WMS.Domain.Models;
using WMS.Infrastructure.Data;

namespace WMS.Tests;

public static class TestDb
{
    public static DbContextOptions<WMSDbContext> UseInMemory(string name)
    {
        return new DbContextOptionsBuilder<WMSDbContext>()
            .UseInMemoryDatabase(databaseName: name)
            .Options;
    }

    public static WMSDbContext NewContext(string? name = null)
        => new(UseInMemory(name ?? Guid.NewGuid().ToString()));
}

public class TestWebApplicationFactory : WebApplicationFactory<Program>
{
    public const string JwtSecret = "TestJwtSecretKeyThatIsAtLeast32CharactersLongForTesting!";

    static TestWebApplicationFactory()
    {
        Environment.SetEnvironmentVariable("WMS_JWT_KEY", JwtSecret);
        Environment.SetEnvironmentVariable("WMS_CONNECTION_STRING", "Server=localhost;Database=WMS_TEST_DUMMY");
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Development");

        builder.ConfigureAppConfiguration((context, config) =>
        {
            config.Sources.Clear();
            config.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Jwt:Key"] = JwtSecret,
                ["Jwt:Issuer"] = "WMS_API",
                ["Jwt:Audience"] = "WMS_Frontend",
                ["ConnectionStrings:DefaultConnection"] = "Server=localhost;Database=WMS_TEST_DUMMY",
                ["Logging:LogLevel:Default"] = "Warning",
                ["Logging:LogLevel:Microsoft.AspNetCore"] = "Warning"
            });
        });

        builder.ConfigureServices(services =>
        {
            // Remove existing DbContext registrations
            var descriptorsToRemove = services
                .Where(d => d.ServiceType == typeof(DbContextOptions<WMSDbContext>) ||
                            d.ServiceType == typeof(WMSDbContext))
                .ToList();
            foreach (var d in descriptorsToRemove) services.Remove(d);

            services.AddDbContext<WMSDbContext>(options =>
                options.UseInMemoryDatabase(databaseName: $"WMS_Test_{Guid.NewGuid()}"));

            // Replace JWT configuration entirely
            services.PostConfigure<JwtBearerOptions>(JwtBearerDefaults.AuthenticationScheme, options =>
            {
                var key = Encoding.UTF8.GetBytes(JwtSecret);
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateLifetime = false
                };
            });

            // Seed admin user after DB is built
            services.AddSingleton<Microsoft.Extensions.Hosting.IHostedService>(sp =>
                new AdminSeeder(sp));
        });
    }

    public static string GenerateJwtToken(string role = "Admin", int roleId = 1)
    {
        var key = Encoding.UTF8.GetBytes(JwtSecret);
        var tokenHandler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new System.Security.Claims.ClaimsIdentity(new[]
            {
                new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.Name, "TestUser"),
                new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.Role, role),
                new System.Security.Claims.Claim("RoleId", roleId.ToString()),
                new System.Security.Claims.Claim("EmployeeId", "1")
            }),
            Expires = DateTime.UtcNow.AddHours(1),
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256Signature)
        };
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
}

internal class AdminSeeder : Microsoft.Extensions.Hosting.IHostedService
{
    private readonly IServiceProvider _sp;
    public AdminSeeder(IServiceProvider sp) => _sp = sp;
    public Task StartAsync(CancellationToken ct)
    {
        using var scope = _sp.CreateScope();
        var ctx = scope.ServiceProvider.GetRequiredService<WMSDbContext>();
        ctx.Database.EnsureCreated();

        if (!ctx.Roles.Any())
        {
            ctx.Roles.AddRange(
                new Role { RoleId = 1, RoleName = "Admin", Description = "System Administrator" },
                new Role { RoleId = 2, RoleName = "Employee", Description = "Standard Employee" },
                new Role { RoleId = 3, RoleName = "Manager", Description = "Team Manager" }
            );
            ctx.SaveChanges();
        }

        if (!ctx.UserLogins.Any(u => u.Username == "admin"))
        {
            ctx.UserLogins.Add(new UserLogin
            {
                Username = "admin",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("admin123"),
                RoleId = 1
            });
            ctx.SaveChanges();
        }
        return Task.CompletedTask;
    }
    public Task StopAsync(CancellationToken ct) => Task.CompletedTask;
}

internal class AuthenticatedClientFixture : IDisposable
{
    public TestWebApplicationFactory Factory { get; }
    public HttpClient Client { get; }
    public string Token { get; }

    public AuthenticatedClientFixture()
    {
        Factory = new TestWebApplicationFactory();
        Token = TestWebApplicationFactory.GenerateJwtToken();
        Client = Factory.CreateClient();
        Client.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", Token);
    }

    public void Dispose()
    {
        Client?.Dispose();
        Factory?.Dispose();
    }
}
