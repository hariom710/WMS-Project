
using Bogus;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using WMS.API.Data;

var builder = WebApplication.CreateBuilder(args);

// Database Connection
builder.Services.AddDbContext<WMSDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")));


// CORS Policy
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



// JWT Authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,

            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],

            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(
                    builder.Configuration["Jwt:Key"]!))
        };
    });

// Dependency Injection
builder.Services.AddScoped<WMS.API.Interfaces.IEmployeeRepository,
    WMS.API.Repositories.EmployeeRepository>();

builder.Services.AddScoped<WMS.API.Interfaces.IEmployeeService,
    WMS.API.Services.EmployeeService>();

// Controllers + Swagger
builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen();

var app = builder.Build();

// Swagger for ALL environments
app.UseSwagger();

app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "WMS API V1");
    options.RoutePrefix = string.Empty;
});

// HTTPS
if (!app.Environment.IsDevelopment())
{
    app.UseHsts();
}

app.UseHttpsRedirection();

// CORS
app.UseCors("AllowAngular");

// Authentication & Authorization
app.UseAuthentication();

app.UseAuthorization();

// Controllers
app.MapControllers();

app.Run();
