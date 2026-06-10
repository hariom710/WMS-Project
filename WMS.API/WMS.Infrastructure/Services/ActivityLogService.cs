using Microsoft.EntityFrameworkCore;
using WMS.Domain.Interfaces;
using WMS.Domain.Models;
using WMS.Infrastructure.Data;

namespace WMS.Infrastructure.Services
{
    public class ActivityLogService : IActivityLogService
    {
        private readonly WMSDbContext _context;
        private readonly AuditLogChannel _channel;

        public ActivityLogService(WMSDbContext context, AuditLogChannel channel)
        {
            _context = context;
            _channel = channel;
        }

        public Task LogAsync(string entityName, int recordId, string action, string? description, string? username, string? userRole, string? ipAddress)
        {
            _channel.Channel.Writer.TryWrite(new AuditLogEntry(entityName, recordId, action, description, username, userRole, ipAddress, DateTime.UtcNow));
            return Task.CompletedTask;
        }

        public async Task<PagedResult<AuditLog>> GetLogsAsync(string? entityName, string? action, string? username, DateTime? from, DateTime? to, string? sortBy, string? sortDirection, int page, int pageSize)
        {
            var query = _context.AuditLogs.AsNoTracking();

            if (!string.IsNullOrWhiteSpace(entityName))
                query = query.Where(l => l.EntityName == entityName);
            if (!string.IsNullOrWhiteSpace(action))
                query = query.Where(l => l.Action == action);
            if (!string.IsNullOrWhiteSpace(username))
                query = query.Where(l => l.Username != null && l.Username.Contains(username));
            if (from.HasValue)
                query = query.Where(l => l.Timestamp >= from.Value);
            if (to.HasValue)
                query = query.Where(l => l.Timestamp <= to.Value);

            var totalCount = await query.CountAsync();

            query = sortBy?.ToLower() switch
            {
                "entityname" => sortDirection == "desc" ? query.OrderByDescending(l => l.EntityName) : query.OrderBy(l => l.EntityName),
                "action" => sortDirection == "desc" ? query.OrderByDescending(l => l.Action) : query.OrderBy(l => l.Action),
                "username" => sortDirection == "desc" ? query.OrderByDescending(l => l.Username) : query.OrderBy(l => l.Username),
                "recordid" => sortDirection == "desc" ? query.OrderByDescending(l => l.RecordId) : query.OrderBy(l => l.RecordId),
                _ => sortDirection == "desc" ? query.OrderByDescending(l => l.Timestamp) : query.OrderBy(l => l.Timestamp)
            };

            var items = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResult<AuditLog>(items, totalCount, page, pageSize);
        }
    }
}
