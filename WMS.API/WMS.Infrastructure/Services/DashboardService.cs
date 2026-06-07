using Microsoft.EntityFrameworkCore;
using WMS.Domain.Interfaces;
using WMS.Domain.Models;
using WMS.Infrastructure.Data;

namespace WMS.Infrastructure.Services
{
    public class DashboardService : IDashboardService
    {
        private readonly WMSDbContext _context;

        public DashboardService(WMSDbContext context) => _context = context;

        public async Task<DashboardSummaryDto> GetSummaryAsync()
        {
            var today = DateTime.Today;
            var sixMonthsAgo = today.AddMonths(-6);

            var kpis = await GetKpiCardsAsync(today);
            var attendance = await GetAttendanceAnalyticsAsync(today);
            var leaves = await GetLeaveAnalyticsAsync();
            var projects = await GetProjectAnalyticsAsync();
            var departments = await GetDepartmentAnalyticsAsync();
            var clients = await GetClientAnalyticsAsync();
            var activities = await GetRecentActivitiesAsync();

            return new DashboardSummaryDto
            {
                Kpis = kpis,
                Attendance = attendance,
                Leaves = leaves,
                Projects = projects,
                Departments = departments,
                Clients = clients,
                RecentActivities = activities
            };
        }

        private async Task<KpiCardsDto> GetKpiCardsAsync(DateTime today)
        {
            var totalEmployees = await _context.Employees.CountAsync();
            var activeEmployees = await _context.Employees.CountAsync(e => e.Status == "Active");

            var presentToday = await _context.Attendances
                .CountAsync(a => a.AttendanceDate == today);

            var employeesOnLeave = await _context.Leaves
                .CountAsync(l => l.Status == "Approved" && l.FromDate <= today && l.ToDate >= today);

            var activeProjects = await _context.Projects.CountAsync(p => p.Status == "Active");
            var activeClients = await _context.Clients.CountAsync();

            var totalAllocations = await _context.ProjectAllocations
                .CountAsync(pa => pa.Status == true);

            var announcementsPublished = await _context.Announcements
                .CountAsync(a => a.IsActive);

            return new KpiCardsDto
            {
                TotalEmployees = totalEmployees,
                ActiveEmployees = activeEmployees,
                PresentToday = presentToday,
                EmployeesOnLeave = employeesOnLeave,
                ActiveProjects = activeProjects,
                ActiveClients = activeClients,
                TotalAllocations = totalAllocations,
                AnnouncementsPublished = announcementsPublished
            };
        }

        private async Task<AttendanceAnalyticsDto> GetAttendanceAnalyticsAsync(DateTime today)
        {
            var activeEmpCount = await _context.Employees.CountAsync(e => e.Status == "Active");
            var presentToday = await _context.Attendances
                .CountAsync(a => a.AttendanceDate == today);
            var absentToday = Math.Max(0, activeEmpCount - presentToday);
            var attendanceRate = activeEmpCount > 0
                ? Math.Round((double)presentToday / activeEmpCount * 100, 1)
                : 0;

            var monthlyTrend = (await _context.Attendances
                .Where(a => a.AttendanceDate >= today.AddMonths(-5).AddDays(-today.Day + 1))
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
            var pendingCount = await _context.Leaves.CountAsync(l => l.Status == "Pending");
            var approvedCount = await _context.Leaves.CountAsync(l => l.Status == "Approved");
            var rejectedCount = await _context.Leaves.CountAsync(l => l.Status == "Rejected");

            var sixMonthsAgo = DateTime.Today.AddMonths(-5).AddDays(-DateTime.Today.Day + 1);
            var monthlyTrend = (await _context.Leaves
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
                PendingCount = pendingCount,
                ApprovedCount = approvedCount,
                RejectedCount = rejectedCount,
                MonthlyTrend = monthlyTrend
            };
        }

        private async Task<ProjectAnalyticsDto> GetProjectAnalyticsAsync()
        {
            var activeCount = await _context.Projects.CountAsync(p => p.Status == "Active");
            var completedCount = await _context.Projects.CountAsync(p => p.Status == "Completed");
            var onHoldCount = await _context.Projects.CountAsync(p => p.Status == "On Hold");

            var statusDistribution = await _context.Projects
                .GroupBy(p => p.Status)
                .Select(g => new StatusCountDto
                {
                    Status = g.Key ?? "Unknown",
                    Count = g.Count()
                })
                .ToListAsync();

            return new ProjectAnalyticsDto
            {
                ActiveCount = activeCount,
                CompletedCount = completedCount,
                OnHoldCount = onHoldCount,
                StatusDistribution = statusDistribution
            };
        }

        private async Task<DepartmentAnalyticsDto> GetDepartmentAnalyticsAsync()
        {
            var departments = await _context.Departments.ToListAsync();

            var employeeCounts = departments.Select(d => new DepartmentCountDto
            {
                DepartmentName = d.DepartmentName,
                EmployeeCount = _context.Employees.Count(e => e.DepartmentId == d.DepartmentId)
            })
            .OrderByDescending(d => d.EmployeeCount)
            .ToList();

            return new DepartmentAnalyticsDto { EmployeeCounts = employeeCounts };
        }

        private async Task<ClientAnalyticsDto> GetClientAnalyticsAsync()
        {
            var totalCount = await _context.Clients.CountAsync();
            var projectCounts = await _context.Clients
                .Select(c => new
                {
                    c.ClientId,
                    HasActiveProject = _context.Projects.Any(p => p.ClientId == c.ClientId && p.Status == "Active")
                })
                .ToListAsync();

            var activeCount = projectCounts.Count(c => c.HasActiveProject);
            var inactiveCount = Math.Max(0, totalCount - activeCount);

            return new ClientAnalyticsDto
            {
                ActiveCount = activeCount,
                InactiveCount = inactiveCount
            };
        }

        private async Task<List<DashboardActivityLogDto>> GetRecentActivitiesAsync()
        {
            return await _context.AuditLogs
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
