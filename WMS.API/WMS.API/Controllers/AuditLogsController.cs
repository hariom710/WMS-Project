using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WMS.API.Helpers;
using WMS.Domain.Interfaces;
using WMS.Domain.Models;

namespace WMS.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class AuditLogsController : ControllerBase
    {
        private readonly IActivityLogService _activityLogService;

        public AuditLogsController(IActivityLogService activityLogService)
        {
            _activityLogService = activityLogService;
        }

        [HttpGet]
        public async Task<IActionResult> GetLogs(
            [FromQuery] string? entityName,
            [FromQuery] string? action,
            [FromQuery] string? username,
            [FromQuery] DateTime? from,
            [FromQuery] DateTime? to,
            [FromQuery] string? sortBy,
            [FromQuery] string? sortDirection,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            var result = await _activityLogService.GetLogsAsync(entityName, action, username, from, to, sortBy, sortDirection, page, pageSize);
            var pagination = new PaginationInfo
            {
                Page = result.Page,
                PageSize = result.PageSize,
                TotalCount = result.TotalCount
            };
            return Ok(ApiResponse<List<AuditLog>>.Ok(result.Items.ToList(), pagination));
        }
    }
}
