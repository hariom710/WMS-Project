namespace WMS.Domain.Models
{
    public class DashboardSummaryDto
    {
        public KpiCardsDto Kpis { get; set; } = new();
        public AttendanceAnalyticsDto Attendance { get; set; } = new();
        public LeaveAnalyticsDto Leaves { get; set; } = new();
        public ProjectAnalyticsDto Projects { get; set; } = new();
        public DepartmentAnalyticsDto Departments { get; set; } = new();
        public ClientAnalyticsDto Clients { get; set; } = new();
        public List<DashboardActivityLogDto> RecentActivities { get; set; } = new();
    }

    public class KpiCardsDto
    {
        public int TotalEmployees { get; set; }
        public int ActiveEmployees { get; set; }
        public int PresentToday { get; set; }
        public int EmployeesOnLeave { get; set; }
        public int ActiveProjects { get; set; }
        public int ActiveClients { get; set; }
        public int TotalAllocations { get; set; }
        public int AnnouncementsPublished { get; set; }
    }

    public class AttendanceAnalyticsDto
    {
        public double AttendanceRate { get; set; }
        public int PresentCount { get; set; }
        public int AbsentCount { get; set; }
        public List<MonthlyTrendDto> MonthlyTrend { get; set; } = new();
    }

    public class LeaveAnalyticsDto
    {
        public int PendingCount { get; set; }
        public int ApprovedCount { get; set; }
        public int RejectedCount { get; set; }
        public List<MonthlyTrendDto> MonthlyTrend { get; set; } = new();
    }

    public class ProjectAnalyticsDto
    {
        public int ActiveCount { get; set; }
        public int CompletedCount { get; set; }
        public int OnHoldCount { get; set; }
        public List<StatusCountDto> StatusDistribution { get; set; } = new();
    }

    public class DepartmentAnalyticsDto
    {
        public List<DepartmentCountDto> EmployeeCounts { get; set; } = new();
    }

    public class ClientAnalyticsDto
    {
        public int ActiveCount { get; set; }
        public int InactiveCount { get; set; }
    }

    public class MonthlyTrendDto
    {
        public string Month { get; set; } = string.Empty;
        public int Count { get; set; }
    }

    public class StatusCountDto
    {
        public string Status { get; set; } = string.Empty;
        public int Count { get; set; }
    }

    public class DepartmentCountDto
    {
        public string DepartmentName { get; set; } = string.Empty;
        public int EmployeeCount { get; set; }
    }

    public class DashboardActivityLogDto
    {
        public int AuditId { get; set; }
        public string EntityName { get; set; } = string.Empty;
        public int RecordId { get; set; }
        public string Action { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? Username { get; set; }
        public string? UserRole { get; set; }
        public string? IpAddress { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
