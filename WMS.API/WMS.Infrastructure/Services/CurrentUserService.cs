using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace WMS.Infrastructure.Services
{
    public class CurrentUserService : Domain.Interfaces.ICurrentUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CurrentUserService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public string? Username =>
            _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Name)?.Value;

        public string? Role =>
            _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Role)?.Value;

        public int? RoleId
        {
            get
            {
                var claim = _httpContextAccessor.HttpContext?.User?.FindFirst("roleId")?.Value;
                return int.TryParse(claim, out var roleId) ? roleId : null;
            }
        }
    }
}
