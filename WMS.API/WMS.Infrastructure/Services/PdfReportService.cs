using Microsoft.EntityFrameworkCore;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using WMS.Domain.Interfaces;
using WMS.Infrastructure.Data;

namespace WMS.Infrastructure.Services
{
    public class PdfReportService : IPdfReportService
    {
        private readonly WMSDbContext _context;
        private readonly IDashboardService _dashboardService;

        public PdfReportService(WMSDbContext context, IDashboardService dashboardService)
        {
            _context = context;
            _dashboardService = dashboardService;
        }

        public async Task<byte[]> ExportEmployeesPdfAsync(string? search, string? status)
        {
            var query = _context.Employees.Include(e => e.Department).Include(e => e.Role).AsNoTracking();
            if (!string.IsNullOrWhiteSpace(search))
                query = query.Where(e => e.FirstName.Contains(search) || e.LastName.Contains(search) || e.Email.Contains(search));
            if (!string.IsNullOrWhiteSpace(status))
                query = query.Where(e => e.Status == status);
            var employees = await query.OrderBy(e => e.EmployeeId).ToListAsync();

            return Document.Create(doc =>
            {
                doc.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(40);
                    page.Header().Element(container => container.Column(col =>
                    {
                        col.Item().Text("WMS — Workforce Management System").FontSize(10).FontColor(Colors.Grey.Medium);
                        col.Item().Text("Employee Directory").FontSize(20).Bold().FontColor(Colors.Blue.Medium);
                        col.Item().Text($"Generated: {DateTime.Now:dd MMM yyyy HH:mm}  |  Records: {employees.Count}").FontSize(9).FontColor(Colors.Grey.Medium);
                        col.Item().PaddingTop(5).LineHorizontal(1).LineColor(Colors.Blue.Medium);
                    }));
                    page.Content().Element(container => container.Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.ConstantColumn(40); columns.RelativeColumn(); columns.RelativeColumn();
                            columns.RelativeColumn(); columns.ConstantColumn(60); columns.ConstantColumn(70);
                        });
                        table.Header(header =>
                        {
                            header.Cell().Background(Colors.Blue.Medium).PaddingHorizontal(4).PaddingVertical(3).Text("ID").FontColor(Colors.White).Bold().FontSize(8);
                            header.Cell().Background(Colors.Blue.Medium).PaddingHorizontal(4).PaddingVertical(3).Text("Name").FontColor(Colors.White).Bold().FontSize(8);
                            header.Cell().Background(Colors.Blue.Medium).PaddingHorizontal(4).PaddingVertical(3).Text("Email").FontColor(Colors.White).Bold().FontSize(8);
                            header.Cell().Background(Colors.Blue.Medium).PaddingHorizontal(4).PaddingVertical(3).Text("Department").FontColor(Colors.White).Bold().FontSize(8);
                            header.Cell().Background(Colors.Blue.Medium).PaddingHorizontal(4).PaddingVertical(3).Text("Role").FontColor(Colors.White).Bold().FontSize(8);
                            header.Cell().Background(Colors.Blue.Medium).PaddingHorizontal(4).PaddingVertical(3).Text("Status").FontColor(Colors.White).Bold().FontSize(8);
                        });
                        foreach (var emp in employees)
                        {
                            table.Cell().Text(emp.EmployeeId.ToString()).FontSize(8);
                            table.Cell().Text($"{emp.FirstName} {emp.LastName}").FontSize(8);
                            table.Cell().Text(emp.Email).FontSize(8);
                            table.Cell().Text(emp.Department?.DepartmentName ?? "N/A").FontSize(8);
                            table.Cell().Text(emp.Role?.RoleName ?? "N/A").FontSize(8);
                            table.Cell().Text(emp.Status).FontSize(8);
                        }
                    }));
                    page.Footer().Element(container => container.AlignCenter().Text(txt =>
                    {
                        txt.Span("Page ").FontSize(8); txt.CurrentPageNumber().FontSize(8);
                        txt.Span(" of ").FontSize(8); txt.TotalPages().FontSize(8);
                    }));
                });
            }).GeneratePdf();
        }

        public async Task<byte[]> ExportAttendancePdfAsync(int? empId, int? month, int? year)
        {
            var query = _context.Attendances.Include(a => a.Employee).AsNoTracking();
            if (empId.HasValue) query = query.Where(a => a.EmpId == empId.Value);
            if (month.HasValue) query = query.Where(a => a.AttendanceDate.Month == month.Value);
            if (year.HasValue) query = query.Where(a => a.AttendanceDate.Year == year.Value);
            var records = await query.OrderByDescending(a => a.AttendanceDate).ToListAsync();

            return Document.Create(doc =>
            {
                doc.Page(page =>
                {
                    page.Size(PageSizes.A4.Landscape());
                    page.Margin(40);
                    page.Header().Element(container => container.Column(col =>
                    {
                        col.Item().Text("WMS — Workforce Management System").FontSize(10).FontColor(Colors.Grey.Medium);
                        col.Item().Text("Attendance Report").FontSize(20).Bold().FontColor(Colors.Blue.Medium);
                        col.Item().Text($"Generated: {DateTime.Now:dd MMM yyyy HH:mm}  |  Records: {records.Count}").FontSize(9).FontColor(Colors.Grey.Medium);
                        col.Item().PaddingTop(5).LineHorizontal(1).LineColor(Colors.Blue.Medium);
                    }));
                    page.Content().Element(container => container.Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.ConstantColumn(40); columns.RelativeColumn(); columns.ConstantColumn(80);
                            columns.ConstantColumn(70); columns.ConstantColumn(70); columns.ConstantColumn(70); columns.ConstantColumn(60);
                        });
                        table.Header(header =>
                        {
                            header.Cell().Background(Colors.Blue.Medium).PaddingHorizontal(4).PaddingVertical(3).Text("ID").FontColor(Colors.White).Bold().FontSize(8);
                            header.Cell().Background(Colors.Blue.Medium).PaddingHorizontal(4).PaddingVertical(3).Text("Employee").FontColor(Colors.White).Bold().FontSize(8);
                            header.Cell().Background(Colors.Blue.Medium).PaddingHorizontal(4).PaddingVertical(3).Text("Date").FontColor(Colors.White).Bold().FontSize(8);
                            header.Cell().Background(Colors.Blue.Medium).PaddingHorizontal(4).PaddingVertical(3).Text("Check-In").FontColor(Colors.White).Bold().FontSize(8);
                            header.Cell().Background(Colors.Blue.Medium).PaddingHorizontal(4).PaddingVertical(3).Text("Check-Out").FontColor(Colors.White).Bold().FontSize(8);
                            header.Cell().Background(Colors.Blue.Medium).PaddingHorizontal(4).PaddingVertical(3).Text("Mode").FontColor(Colors.White).Bold().FontSize(8);
                            header.Cell().Background(Colors.Blue.Medium).PaddingHorizontal(4).PaddingVertical(3).Text("Hours").FontColor(Colors.White).Bold().FontSize(8);
                        });
                        foreach (var r in records)
                        {
                            table.Cell().Text(r.AttendanceId.ToString()).FontSize(8);
                            table.Cell().Text(r.Employee != null ? $"{r.Employee.FirstName} {r.Employee.LastName}" : "N/A").FontSize(8);
                            table.Cell().Text(r.AttendanceDate.ToString("dd MMM yyyy")).FontSize(8);
                            table.Cell().Text(r.CheckIn.ToString("HH:mm")).FontSize(8);
                            table.Cell().Text(r.CheckOut?.ToString("HH:mm") ?? "—").FontSize(8);
                            table.Cell().Text(r.WorkMode).FontSize(8);
                            table.Cell().Text(r.TotalHours?.ToString("F2") ?? "—").FontSize(8);
                        }
                    }));
                    page.Footer().Element(container => container.AlignCenter().Text(txt =>
                    {
                        txt.Span("Page ").FontSize(8); txt.CurrentPageNumber().FontSize(8);
                        txt.Span(" of ").FontSize(8); txt.TotalPages().FontSize(8);
                    }));
                });
            }).GeneratePdf();
        }

        public async Task<byte[]> ExportLeavesPdfAsync(string? status)
        {
            var query = _context.Leaves.Include(l => l.Employee).AsNoTracking();
            if (!string.IsNullOrWhiteSpace(status)) query = query.Where(l => l.Status == status);
            var leaves = await query.OrderByDescending(l => l.FromDate).ToListAsync();

            return Document.Create(doc =>
            {
                doc.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(40);
                    page.Header().Element(container => container.Column(col =>
                    {
                        col.Item().Text("WMS — Workforce Management System").FontSize(10).FontColor(Colors.Grey.Medium);
                        col.Item().Text("Leave Report").FontSize(20).Bold().FontColor(Colors.Blue.Medium);
                        col.Item().Text($"Generated: {DateTime.Now:dd MMM yyyy HH:mm}  |  Records: {leaves.Count}").FontSize(9).FontColor(Colors.Grey.Medium);
                        col.Item().PaddingTop(5).LineHorizontal(1).LineColor(Colors.Blue.Medium);
                    }));
                    page.Content().Element(container => container.Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.ConstantColumn(40); columns.RelativeColumn(); columns.ConstantColumn(80);
                            columns.ConstantColumn(70); columns.ConstantColumn(70); columns.RelativeColumn(); columns.ConstantColumn(60);
                        });
                        table.Header(header =>
                        {
                            header.Cell().Background(Colors.Blue.Medium).PaddingHorizontal(4).PaddingVertical(3).Text("ID").FontColor(Colors.White).Bold().FontSize(8);
                            header.Cell().Background(Colors.Blue.Medium).PaddingHorizontal(4).PaddingVertical(3).Text("Employee").FontColor(Colors.White).Bold().FontSize(8);
                            header.Cell().Background(Colors.Blue.Medium).PaddingHorizontal(4).PaddingVertical(3).Text("Type").FontColor(Colors.White).Bold().FontSize(8);
                            header.Cell().Background(Colors.Blue.Medium).PaddingHorizontal(4).PaddingVertical(3).Text("From").FontColor(Colors.White).Bold().FontSize(8);
                            header.Cell().Background(Colors.Blue.Medium).PaddingHorizontal(4).PaddingVertical(3).Text("To").FontColor(Colors.White).Bold().FontSize(8);
                            header.Cell().Background(Colors.Blue.Medium).PaddingHorizontal(4).PaddingVertical(3).Text("Reason").FontColor(Colors.White).Bold().FontSize(8);
                            header.Cell().Background(Colors.Blue.Medium).PaddingHorizontal(4).PaddingVertical(3).Text("Status").FontColor(Colors.White).Bold().FontSize(8);
                        });
                        foreach (var l in leaves)
                        {
                            table.Cell().Text(l.LeaveId.ToString()).FontSize(8);
                            table.Cell().Text(l.Employee != null ? $"{l.Employee.FirstName} {l.Employee.LastName}" : "N/A").FontSize(8);
                            table.Cell().Text(l.LeaveType).FontSize(8);
                            table.Cell().Text(l.FromDate.ToString("dd MMM yyyy")).FontSize(8);
                            table.Cell().Text(l.ToDate.ToString("dd MMM yyyy")).FontSize(8);
                            table.Cell().Text(l.Reason.Length > 50 ? l.Reason[..50] + "…" : l.Reason).FontSize(8);
                            table.Cell().Text(l.Status).FontSize(8);
                        }
                    }));
                    page.Footer().Element(container => container.AlignCenter().Text(txt =>
                    {
                        txt.Span("Page ").FontSize(8); txt.CurrentPageNumber().FontSize(8);
                        txt.Span(" of ").FontSize(8); txt.TotalPages().FontSize(8);
                    }));
                });
            }).GeneratePdf();
        }

        public async Task<byte[]> ExportProjectsPdfAsync(string? status)
        {
            var query = _context.Projects.Include(p => p.Client).AsNoTracking();
            if (!string.IsNullOrWhiteSpace(status)) query = query.Where(p => p.Status == status);
            var projects = await query.OrderBy(p => p.ProjectId).ToListAsync();

            return Document.Create(doc =>
            {
                doc.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(40);
                    page.Header().Element(container => container.Column(col =>
                    {
                        col.Item().Text("WMS — Workforce Management System").FontSize(10).FontColor(Colors.Grey.Medium);
                        col.Item().Text("Project Report").FontSize(20).Bold().FontColor(Colors.Blue.Medium);
                        col.Item().Text($"Generated: {DateTime.Now:dd MMM yyyy HH:mm}  |  Records: {projects.Count}").FontSize(9).FontColor(Colors.Grey.Medium);
                        col.Item().PaddingTop(5).LineHorizontal(1).LineColor(Colors.Blue.Medium);
                    }));
                    page.Content().Element(container => container.Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.ConstantColumn(40); columns.RelativeColumn(); columns.RelativeColumn();
                            columns.ConstantColumn(80); columns.ConstantColumn(80); columns.ConstantColumn(70);
                        });
                        table.Header(header =>
                        {
                            header.Cell().Background(Colors.Blue.Medium).PaddingHorizontal(4).PaddingVertical(3).Text("ID").FontColor(Colors.White).Bold().FontSize(8);
                            header.Cell().Background(Colors.Blue.Medium).PaddingHorizontal(4).PaddingVertical(3).Text("Project Name").FontColor(Colors.White).Bold().FontSize(8);
                            header.Cell().Background(Colors.Blue.Medium).PaddingHorizontal(4).PaddingVertical(3).Text("Client").FontColor(Colors.White).Bold().FontSize(8);
                            header.Cell().Background(Colors.Blue.Medium).PaddingHorizontal(4).PaddingVertical(3).Text("Start Date").FontColor(Colors.White).Bold().FontSize(8);
                            header.Cell().Background(Colors.Blue.Medium).PaddingHorizontal(4).PaddingVertical(3).Text("End Date").FontColor(Colors.White).Bold().FontSize(8);
                            header.Cell().Background(Colors.Blue.Medium).PaddingHorizontal(4).PaddingVertical(3).Text("Status").FontColor(Colors.White).Bold().FontSize(8);
                        });
                        foreach (var p in projects)
                        {
                            table.Cell().Text(p.ProjectId.ToString()).FontSize(8);
                            table.Cell().Text(p.ProjectName).FontSize(8);
                            table.Cell().Text(p.Client?.ClientName ?? "N/A").FontSize(8);
                            table.Cell().Text(p.StartDate?.ToString("dd MMM yyyy") ?? "N/A").FontSize(8);
                            table.Cell().Text(p.EndDate?.ToString("dd MMM yyyy") ?? "N/A").FontSize(8);
                            table.Cell().Text(p.Status).FontSize(8);
                        }
                    }));
                    page.Footer().Element(container => container.AlignCenter().Text(txt =>
                    {
                        txt.Span("Page ").FontSize(8); txt.CurrentPageNumber().FontSize(8);
                        txt.Span(" of ").FontSize(8); txt.TotalPages().FontSize(8);
                    }));
                });
            }).GeneratePdf();
        }

        public async Task<byte[]> ExportDashboardPdfAsync()
        {
            var summary = await _dashboardService.GetSummaryAsync();

            return Document.Create(doc =>
            {
                doc.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(40);
                    page.Header().Element(container => container.Column(col =>
                    {
                        col.Item().Text("WMS — Workforce Management System").FontSize(10).FontColor(Colors.Grey.Medium);
                        col.Item().Text("Executive Dashboard Summary").FontSize(20).Bold().FontColor(Colors.Blue.Medium);
                        col.Item().Text($"Generated: {DateTime.Now:dd MMM yyyy HH:mm}").FontSize(9).FontColor(Colors.Grey.Medium);
                        col.Item().PaddingTop(5).LineHorizontal(1).LineColor(Colors.Blue.Medium);
                    }));
                    page.Content().Element(container => container.Column(col =>
                    {
                        col.Item().PaddingTop(10).Text("Key Performance Indicators").FontSize(14).Bold();
                        col.Item().PaddingTop(3).LineHorizontal(0.5f).LineColor(Colors.Grey.Lighten3);
                        col.Item().PaddingTop(5).Table(table =>
                        {
                            table.ColumnsDefinition(columns => { columns.RelativeColumn(); columns.ConstantColumn(80); });
                            AddKpiRow(table, "Total Employees", summary.Kpis.TotalEmployees.ToString());
                            AddKpiRow(table, "Active Employees", summary.Kpis.ActiveEmployees.ToString());
                            AddKpiRow(table, "Present Today", summary.Kpis.PresentToday.ToString());
                            AddKpiRow(table, "Employees On Leave", summary.Kpis.EmployeesOnLeave.ToString());
                            AddKpiRow(table, "Active Projects", summary.Kpis.ActiveProjects.ToString());
                            AddKpiRow(table, "Active Clients", summary.Kpis.ActiveClients.ToString());
                            AddKpiRow(table, "Active Allocations", summary.Kpis.TotalAllocations.ToString());
                            AddKpiRow(table, "Announcements", summary.Kpis.AnnouncementsPublished.ToString());
                        });
                        col.Item().PaddingTop(12).Text("Leave Summary").FontSize(14).Bold();
                        col.Item().PaddingTop(3).LineHorizontal(0.5f).LineColor(Colors.Grey.Lighten3);
                        col.Item().PaddingTop(5).Table(table =>
                        {
                            table.ColumnsDefinition(columns => { columns.RelativeColumn(); columns.ConstantColumn(80); });
                            AddKpiRow(table, "Pending", summary.Leaves.PendingCount.ToString());
                            AddKpiRow(table, "Approved", summary.Leaves.ApprovedCount.ToString());
                            AddKpiRow(table, "Rejected", summary.Leaves.RejectedCount.ToString());
                        });
                        col.Item().PaddingTop(12).Text("Project Summary").FontSize(14).Bold();
                        col.Item().PaddingTop(3).LineHorizontal(0.5f).LineColor(Colors.Grey.Lighten3);
                        col.Item().PaddingTop(5).Table(table =>
                        {
                            table.ColumnsDefinition(columns => { columns.RelativeColumn(); columns.ConstantColumn(80); });
                            AddKpiRow(table, "Active", summary.Projects.ActiveCount.ToString());
                            AddKpiRow(table, "Completed", summary.Projects.CompletedCount.ToString());
                            AddKpiRow(table, "On Hold", summary.Projects.OnHoldCount.ToString());
                        });
                    }));
                    page.Footer().Element(container => container.AlignCenter().Text(txt =>
                    {
                        txt.Span("Page ").FontSize(8); txt.CurrentPageNumber().FontSize(8);
                        txt.Span(" of ").FontSize(8); txt.TotalPages().FontSize(8);
                    }));
                });
            }).GeneratePdf();
        }

        private static void AddKpiRow(TableDescriptor table, string label, string value)
        {
            table.Cell().Text(label).FontSize(9);
            table.Cell().AlignRight().Text(value).FontSize(9).Bold();
        }
    }
}
