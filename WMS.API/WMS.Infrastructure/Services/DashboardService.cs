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

        public async Task<DashboardSummaryDto> GetSummaryAsync()
        {
            var today = DateTime.Today;

            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<WMSDbContext>();

            var kpisTask = GetKpiCardsAsync(context, today);
            var attendanceTask = GetAttendanceAnalyticsAsync(context, today);
            var leavesTask = GetLeaveAnalyticsAsync(context);
            var projectsTask = GetProjectAnalyticsAsync(context);
            var departmentsTask = GetDepartmentAnalyticsAsync(context);
            var clientsTask = GetClientAnalyticsAsync(context);
            var activitiesTask = GetRecentActivitiesAsync(context);

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

        private async Task<KpiCardsDto> GetKpiCardsAsync(WMSDbContext context, DateTime today)
        {
            var task1 = context.Leaves
                .CountAsync(l => l.Status == "Approved" && l.FromDate <= today && l.ToDate >= today);

            var task2 = context.ProjectAllocations
                .CountAsync(pa => pa.Status == true);

            var task3 = context.Announcements
                .CountAsync(a => a.IsActive);

            var task4Total = context.Employees.CountAsync();
            var task4Active = context.Employees.CountAsync(e => e.Status == "Active");

            var task5 = context.Attendances
                .CountAsync(a => a.AttendanceDate == today);

            var task6 = context.Projects.CountAsync(p => p.Status == "Active");

            var task7 = context.Clients.CountAsync();

            await Task.WhenAll(task1, task2, task3, task4Total, task4Active, task5, task6, task7);

            return new KpiCardsDto
            {
                TotalEmployees = task4Total.Result,
                ActiveEmployees = task4Active.Result,
                PresentToday = task5.Result,
                EmployeesOnLeave = task1.Result,
                ActiveProjects = task6.Result,
                ActiveClients = task7.Result,
                TotalAllocations = task2.Result,
                AnnouncementsPublished = task3.Result
            };
        }

        private async Task<AttendanceAnalyticsDto> GetAttendanceAnalyticsAsync(WMSDbContext context, DateTime today)
        {
            var statsTask = context.Attendances
                .CountAsync(a => a.AttendanceDate == today);

            var activeEmpCountTask = context.Employees.CountAsync(e => e.Status == "Active");

            var sixMonthsAgo = today.AddMonths(-5).AddDays(-today.Day + 1);
            var monthlyTrendTask = context.Attendances
                .Where(a => a.AttendanceDate >= sixMonthsAgo)
                .GroupBy(a => new { a.AttendanceDate.Year, a.AttendanceDate.Month })
                .Select(g => new { g.Key.Month, g.Key.Year, Count = g.Count() })
                .OrderBy(t => t.Year).ThenBy(t => t.Month)
                .ToListAsync();

            await Task.WhenAll(statsTask, activeEmpCountTask, monthlyTrendTask);

            var presentToday = statsTask.Result;
            var activeEmpCount = activeEmpCountTask.Result;
            var absentToday = Math.Max(0, activeEmpCount - presentToday);
            var attendanceRate = activeEmpCount > 0
                ? Math.Round((double)presentToday / activeEmpCount * 100, 1)
                : 0;

            var monthlyTrend = monthlyTrendTask.Result
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

        private async Task<LeaveAnalyticsDto> GetLeaveAnalyticsAsync(WMSDbContext context)
        {
            var sixMonthsAgo = DateTime.Today.AddMonths(-5).AddDays(-DateTime.Today.Day + 1);

            var statusCountsTask = context.Leaves
                .GroupBy(l => l.Status)
                .Select(g => new { Status = g.Key, Count = g.Count() })
                .ToDictionaryAsync(g => g.Status, g => g.Count);

            var monthlyTrendTask = context.Leaves
                .Where(l => l.CreatedDate >= sixMonthsAgo)
                .GroupBy(l => new { l.CreatedDate.Year, l.CreatedDate.Month })
                .Select(g => new { g.Key.Month, g.Key.Year, Count = g.Count() })
                .OrderBy(t => t.Year).ThenBy(t => t.Month)
                .ToListAsync();

            await Task.WhenAll(statusCountsTask, monthlyTrendTask);

            var statusCounts = statusCountsTask.Result;

            var monthlyTrend = monthlyTrendTask.Result
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

        private async Task<ProjectAnalyticsDto> GetProjectAnalyticsAsync(WMSDbContext context)
        {
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

        private async Task<DepartmentAnalyticsDto> GetDepartmentAnalyticsAsync(WMSDbContext context)
        {
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

        private async Task<ClientAnalyticsDto> GetClientAnalyticsAsync(WMSDbContext context)
        {
            var totalCountTask = context.Clients.CountAsync();
            var clientsWithProjectsTask = context.Projects
                .Where(p => p.Status == "Active")
                .Select(p => p.ClientId)
                .Distinct()
                .CountAsync();

            await Task.WhenAll(totalCountTask, clientsWithProjectsTask);

            var totalCount = totalCountTask.Result;
            var clientsWithProjects = clientsWithProjectsTask.Result;

            return new ClientAnalyticsDto
            {
                ActiveCount = clientsWithProjects,
                InactiveCount = Math.Max(0, totalCount - clientsWithProjects)
            };
        }

        private async Task<List<DashboardActivityLogDto>> GetRecentActivitiesAsync(WMSDbContext context)
        {
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
