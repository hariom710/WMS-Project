using System.Threading.Channels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using WMS.Domain.Models;
using WMS.Infrastructure.Data;

namespace WMS.Infrastructure.Services
{
    public record AuditLogEntry(string EntityName, int RecordId, string Action, string? Description, string? Username, string? UserRole, string? IpAddress, DateTime Timestamp);

    public class AuditLogChannel
    {
        public Channel<AuditLogEntry> Channel { get; }

        public AuditLogChannel()
        {
            Channel = System.Threading.Channels.Channel.CreateUnbounded<AuditLogEntry>(new UnboundedChannelOptions { SingleReader = true, SingleWriter = false });
        }
    }

    public class AuditLogWorker : BackgroundService
    {
        private readonly IServiceProvider _sp;
        private readonly ILogger<AuditLogWorker> _logger;

        public AuditLogWorker(IServiceProvider sp, ILogger<AuditLogWorker> logger)
        {
            _sp = sp;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var channel = _sp.GetRequiredService<AuditLogChannel>().Channel;
            await foreach (var entry in channel.Reader.ReadAllAsync(stoppingToken))
            {
                try
                {
                    using var scope = _sp.CreateScope();
                    var ctx = scope.ServiceProvider.GetRequiredService<WMSDbContext>();
                    ctx.AuditLogs.Add(new AuditLog
                    {
                        EntityName = entry.EntityName,
                        RecordId = entry.RecordId,
                        Action = entry.Action,
                        Description = entry.Description,
                        Username = entry.Username,
                        UserRole = entry.UserRole,
                        IpAddress = entry.IpAddress,
                        Timestamp = entry.Timestamp
                    });
                    await ctx.SaveChangesAsync(stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Failed to write audit log for {Action} on {Entity}", entry.Action, entry.EntityName);
                }
            }
        }
    }
}
