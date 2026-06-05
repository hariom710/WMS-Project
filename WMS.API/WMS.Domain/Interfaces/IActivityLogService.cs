using WMS.Domain.Models;

namespace WMS.Domain.Interfaces
{
    public interface IActivityLogService
    {
        Task LogAsync(string entityName, int recordId, string action, string? description, string? username, string? userRole, string? ipAddress);
        Task<PagedResult<AuditLog>> GetLogsAsync(string? entityName, string? action, string? username, DateTime? from, DateTime? to, string? sortBy, string? sortDirection, int page, int pageSize);
    }
}
