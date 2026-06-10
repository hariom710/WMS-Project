using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using WMS.Domain.Interfaces;
using WMS.Domain.Models;
using WMS.Infrastructure.Data;

namespace WMS.Infrastructure.Services
{
    public class DashboardService : IDashboardService
    {
        private readonly IServiceScopeFactory _scopeFactory;

        public DashboardService(IServiceScopeFactory scopeFactory) => _scopeFactory = scopeFactory;

        private WMSDbContext CreateContext()
        {
            return _scopeFactory.CreateScope().ServiceProvider.GetRequiredService<WMSDbContext>();
        }

        public async Task<DashboardSummaryDto> GetSummaryAsync()
        {
            var today = DateTime.Today;

            var kpisTask = GetKpiCardsAsync(today);
            var attendanceTask = GetAttendanceAnalyticsAsync(today);
            var leavesTask = GetLeaveAnalyticsAsync();
            var projectsTask = GetProjectAnalyticsAsync();
            var departmentsTask = GetDepartmentAnalyticsAsync();
            var clientsTask = GetClientAnalyticsAsync();
            var activitiesTask = GetRecentActivitiesAsync();

            await Task.WhenAll(kpisTask, attendanceTask, leavesTask, projectsTask, departmentsTask, clientsTask, activitiesTask);

            return new DashboardSummaryDto
            {
                Kpis = kpisTask.Result,
                Attendance = attendanceTask.Result,
                Leaves = leavesTask.Result,
                Projects = projectsTask.Result,
                Departments = departmentsTask.Result,
                Clients = clientsTask.Result,
                RecentActivities = activitiesTask.Result
            };
        }

        private async Task<KpiCardsDto> GetKpiCardsAsync(DateTime today)
        {
            using var context = CreateContext();

            var employeesOnLeave = await context.Leaves
                .CountAsync(l => l.Status == "Approved" && l.FromDate <= today && l.ToDate >= today);

            var totalAllocations = await context.ProjectAllocations
                .CountAsync(pa => pa.Status == true);

            var announcementsPublished = await context.Announcements
                .CountAsync(a => a.IsActive);

            var employeeStats = await context.Employees
                .GroupBy(e => 1)
                .Select(g => new
                {
                    Total = g.Count(),
                    Active = g.Count(e => e.Status == "Active")
                })
                .FirstOrDefaultAsync() ?? new { Total = 0, Active = 0 };

            var presentToday = await context.Attendances
                .CountAsync(a => a.AttendanceDate == today);

            var projectStats = await context.Projects
                .GroupBy(p => 1)
                .Select(g => new
                {
                    Active = g.Count(p => p.Status == "Active")
                })
                .FirstOrDefaultAsync() ?? new { Active = 0 };

            var activeClients = await context.Clients.CountAsync();

            return new KpiCardsDto
            {
                TotalEmployees = employeeStats.Total,
                ActiveEmployees = employeeStats.Active,
                PresentToday = presentToday,
                EmployeesOnLeave = employeesOnLeave,
                ActiveProjects = projectStats.Active,
                ActiveClients = activeClients,
                TotalAllocations = totalAllocations,
                AnnouncementsPublished = announcementsPublished
            };
        }

        private async Task<AttendanceAnalyticsDto> GetAttendanceAnalyticsAsync(DateTime today)
        {
            using var context = CreateContext();

            var stats = await context.Attendances
                .Where(a => a.AttendanceDate == today)
                .GroupBy(a => 1)
                .Select(g => new { Count = g.Count() })
                .FirstOrDefaultAsync() ?? new { Count = 0 };

            var activeEmpCount = await context.Employees.CountAsync(e => e.Status == "Active");
            var presentToday = stats.Count;
            var absentToday = Math.Max(0, activeEmpCount - presentToday);
            var attendanceRate = activeEmpCount > 0
                ? Math.Round((double)presentToday / activeEmpCount * 100, 1)
                : 0;

            var sixMonthsAgo = today.AddMonths(-5).AddDays(-today.Day + 1);
            var monthlyTrend = (await context.Attendances
                .Where(a => a.AttendanceDate >= sixMonthsAgo)
                .GroupBy(a => new { a.AttendanceDate.Year, a.AttendanceDate.Month })
                .Select(g => new { g.Key.Month, g.Key.Year, Count = g.Count() })
                .OrderBy(t => t.Year).ThenBy(t => t.Month)
                .ToListAsync())
                .Select(g => new MonthlyTrendDto
                {
                    Month = $"{g.Month:D2}/{g.Year}",
                    Count = g.Count
                })
                .ToList();

            return new AttendanceAnalyticsDto
            {
                AttendanceRate = attendanceRate,
                PresentCount = presentToday,
                AbsentCount = absentToday,
                MonthlyTrend = monthlyTrend
            };
        }

        private async Task<LeaveAnalyticsDto> GetLeaveAnalyticsAsync()
        {
            using var context = CreateContext();
            var sixMonthsAgo = DateTime.Today.AddMonths(-5).AddDays(-DateTime.Today.Day + 1);

            var statusCounts = await context.Leaves
                .GroupBy(l => l.Status)
                .Select(g => new { Status = g.Key, Count = g.Count() })
                .ToDictionaryAsync(g => g.Status, g => g.Count);

            var monthlyTrend = (await context.Leaves
                .Where(l => l.CreatedDate >= sixMonthsAgo)
                .GroupBy(l => new { l.CreatedDate.Year, l.CreatedDate.Month })
                .Select(g => new { g.Key.Month, g.Key.Year, Count = g.Count() })
                .OrderBy(t => t.Year).ThenBy(t => t.Month)
                .ToListAsync())
                .Select(g => new MonthlyTrendDto
                {
                    Month = $"{g.Month:D2}/{g.Year}",
                    Count = g.Count
                })
                .ToList();

            return new LeaveAnalyticsDto
            {
                PendingCount = statusCounts.GetValueOrDefault("Pending", 0),
                ApprovedCount = statusCounts.GetValueOrDefault("Approved", 0),
                RejectedCount = statusCounts.GetValueOrDefault("Rejected", 0),
                MonthlyTrend = monthlyTrend
            };
        }

        private async Task<ProjectAnalyticsDto> GetProjectAnalyticsAsync()
        {
            using var context = CreateContext();

            var statusCounts = await context.Projects
                .GroupBy(p => p.Status)
                .Select(g => new { Status = g.Key, Count = g.Count() })
                .ToDictionaryAsync(g => g.Status ?? "Unknown", g => g.Count);

            var statusDistribution = statusCounts
                .Select(kvp => new StatusCountDto { Status = kvp.Key, Count = kvp.Value })
                .ToList();

            return new ProjectAnalyticsDto
            {
                ActiveCount = statusCounts.GetValueOrDefault("Active", 0),
                CompletedCount = statusCounts.GetValueOrDefault("Completed", 0),
                OnHoldCount = statusCounts.GetValueOrDefault("On Hold", 0),
                StatusDistribution = statusDistribution
            };
        }

        private async Task<DepartmentAnalyticsDto> GetDepartmentAnalyticsAsync()
        {
            using var context = CreateContext();

            var employeeCounts = await context.Employees
                .GroupBy(e => e.DepartmentId)
                .Select(g => new { DepartmentId = g.Key, Count = g.Count() })
                .Join(
                    context.Departments,
                    emp => emp.DepartmentId,
                    dept => dept.DepartmentId,
                    (emp, dept) => new DepartmentCountDto
                    {
                        DepartmentName = dept.DepartmentName,
                        EmployeeCount = emp.Count
                    })
                .OrderByDescending(d => d.EmployeeCount)
                .ToListAsync();

            return new DepartmentAnalyticsDto { EmployeeCounts = employeeCounts };
        }

        private async Task<ClientAnalyticsDto> GetClientAnalyticsAsync()
        {
            using var context = CreateContext();

            var totalCount = await context.Clients.CountAsync();

            var clientsWithProjects = await context.Projects
                .Where(p => p.Status == "Active")
                .Select(p => p.ClientId)
                .Distinct()
                .CountAsync();

            return new ClientAnalyticsDto
            {
                ActiveCount = clientsWithProjects,
                InactiveCount = Math.Max(0, totalCount - clientsWithProjects)
            };
        }

        private async Task<List<DashboardActivityLogDto>> GetRecentActivitiesAsync()
        {
            using var context = CreateContext();

            return await context.AuditLogs
                .OrderByDescending(a => a.Timestamp)
                .Take(10)
                .Select(a => new DashboardActivityLogDto
                {
                    AuditId = a.AuditId,
                    EntityName = a.EntityName,
                    RecordId = a.RecordId,
                    Action = a.Action,
                    Description = a.Description,
                    Username = a.Username,
                    UserRole = a.UserRole,
                    IpAddress = a.IpAddress,
                    Timestamp = a.Timestamp
                })
                .ToListAsync();
        }
    }
}
