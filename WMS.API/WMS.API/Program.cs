using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using WMS.API.Data;
using WMS.API.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Use User Secrets in Development
if (builder.Environment.IsDevelopment())
{
    builder.Configuration.AddUserSecrets<Program>();
}

// Build the connection string from environment/User Secrets with fallback
var connStr = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? Environment.GetEnvironmentVariable("WMS_CONNECTION_STRING")
    ?? "";

var jwtKey = builder.Configuration["Jwt:Key"]
    ?? Environment.GetEnvironmentVariable("WMS_JWT_KEY")
    ?? "";

var jwtIssuer = builder.Configuration["Jwt:Issuer"] ?? "WMS_API";
var jwtAudience = builder.Configuration["Jwt:Audience"] ?? "WMS_Frontend";

builder.Services.AddDbContext<WMSDbContext>(options =>
    options.UseSqlServer(connStr));

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngular",
        policy =>
        {
            policy.WithOrigins(
                    "http://localhost:4200",
                    "https://localhost:4200",
                    "https://hariomwmsfrontend8501.azurewebsites.net")
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });
});

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            RoleClaimType = "http://schemas.microsoft.com/ws/2008/06/identity/claims/role",
            ValidIssuer = jwtIssuer,
            ValidAudience = jwtAudience,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtKey))
        };
    });

builder.Services.AddScoped<WMS.API.Interfaces.IEmployeeRepository, WMS.API.Repositories.EmployeeRepository>();
builder.Services.AddScoped<WMS.API.Interfaces.IEmployeeService, WMS.API.Services.EmployeeService>();

builder.Services.AddAutoMapper(typeof(WMS.Application.Mapping.MappingProfile));

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseCustomExceptionHandler();

app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "WMS API V1");
});

if (!app.Environment.IsDevelopment())
{
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseCors("AllowAngular");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
