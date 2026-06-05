using ClosedXML.Excel;
using Microsoft.EntityFrameworkCore;
using WMS.Domain.Interfaces;
using WMS.Infrastructure.Data;

namespace WMS.Infrastructure.Services
{
    public class ReportService : IReportService
    {
        private readonly WMSDbContext _context;

        public ReportService(WMSDbContext context) => _context = context;

        public async Task<byte[]> ExportEmployeesToExcelAsync(string? search, string? status)
        {
            var query = _context.Employees
                .Include(e => e.Department)
                .Include(e => e.Role)
                .AsNoTracking();

            if (!string.IsNullOrWhiteSpace(search))
                query = query.Where(e => e.FirstName.Contains(search) || e.LastName.Contains(search) || e.Email.Contains(search));
            if (!string.IsNullOrWhiteSpace(status))
                query = query.Where(e => e.Status == status);

            var employees = await query.OrderBy(e => e.EmployeeId).ToListAsync();

            using var workbook = new XLWorkbook();
            var ws = workbook.Worksheets.Add("Employees");

            AddReportHeader(ws, "Employee Directory", employees.Count, 8);

            var headers = new[] { "ID", "First Name", "Last Name", "Email", "Phone", "Department", "Role", "Status" };
            AddHeaders(ws, headers, 3);

            for (int i = 0; i < employees.Count; i++)
            {
                var row = i + 4;
                ws.Cell(row, 1).Value = employees[i].EmployeeId;
                ws.Cell(row, 2).Value = employees[i].FirstName;
                ws.Cell(row, 3).Value = employees[i].LastName;
                ws.Cell(row, 4).Value = employees[i].Email;
                ws.Cell(row, 5).Value = employees[i].PhoneNumber;
                ws.Cell(row, 6).Value = employees[i].Department?.DepartmentName ?? "N/A";
                ws.Cell(row, 7).Value = employees[i].Role?.RoleName ?? "N/A";
                ws.Cell(row, 8).Value = employees[i].Status;
            }

            FormatTable(ws, employees.Count, 8);
            return ToBytes(workbook);
        }

        public async Task<byte[]> ExportAttendanceToExcelAsync(int? empId, int? month, int? year)
        {
            var query = _context.Attendances
                .Include(a => a.Employee)
                .AsNoTracking();

            if (empId.HasValue)
                query = query.Where(a => a.EmpId == empId.Value);
            if (month.HasValue)
                query = query.Where(a => a.AttendanceDate.Month == month.Value);
            if (year.HasValue)
                query = query.Where(a => a.AttendanceDate.Year == year.Value);

            var records = await query.OrderByDescending(a => a.AttendanceDate).ToListAsync();

            using var workbook = new XLWorkbook();
            var ws = workbook.Worksheets.Add("Attendance");

            AddReportHeader(ws, "Attendance Report", records.Count, 7);

            var headers = new[] { "ID", "Employee", "Date", "Check-In", "Check-Out", "Work Mode", "Total Hours" };
            AddHeaders(ws, headers, 3);

            for (int i = 0; i < records.Count; i++)
            {
                var row = i + 4;
                ws.Cell(row, 1).Value = records[i].AttendanceId;
                ws.Cell(row, 2).Value = records[i].Employee != null ? $"{records[i].Employee.FirstName} {records[i].Employee.LastName}" : "N/A";
                ws.Cell(row, 3).Value = records[i].AttendanceDate.ToString("yyyy-MM-dd");
                ws.Cell(row, 4).Value = records[i].CheckIn.ToString("HH:mm");
                ws.Cell(row, 5).Value = records[i].CheckOut?.ToString("HH:mm") ?? "—";
                ws.Cell(row, 6).Value = records[i].WorkMode;
                ws.Cell(row, 7).Value = records[i].TotalHours?.ToString("F2") ?? "—";
            }

            FormatTable(ws, records.Count, 7);
            return ToBytes(workbook);
        }

        public async Task<byte[]> ExportLeavesToExcelAsync(string? status)
        {
            var query = _context.Leaves
                .Include(l => l.Employee)
                .AsNoTracking();

            if (!string.IsNullOrWhiteSpace(status))
                query = query.Where(l => l.Status == status);

            var leaves = await query.OrderByDescending(l => l.FromDate).ToListAsync();

            using var workbook = new XLWorkbook();
            var ws = workbook.Worksheets.Add("Leaves");

            AddReportHeader(ws, "Leave Report", leaves.Count, 7);

            var headers = new[] { "ID", "Employee", "Leave Type", "From", "To", "Reason", "Status" };
            AddHeaders(ws, headers, 3);

            for (int i = 0; i < leaves.Count; i++)
            {
                var row = i + 4;
                ws.Cell(row, 1).Value = leaves[i].LeaveId;
                ws.Cell(row, 2).Value = leaves[i].Employee != null ? $"{leaves[i].Employee.FirstName} {leaves[i].Employee.LastName}" : "N/A";
                ws.Cell(row, 3).Value = leaves[i].LeaveType;
                ws.Cell(row, 4).Value = leaves[i].FromDate.ToString("yyyy-MM-dd");
                ws.Cell(row, 5).Value = leaves[i].ToDate.ToString("yyyy-MM-dd");
                ws.Cell(row, 6).Value = leaves[i].Reason;
                ws.Cell(row, 7).Value = leaves[i].Status;
            }

            FormatTable(ws, leaves.Count, 7);
            return ToBytes(workbook);
        }

        public async Task<byte[]> ExportProjectsToExcelAsync(string? status)
        {
            var query = _context.Projects
                .Include(p => p.Client)
                .AsNoTracking();

            if (!string.IsNullOrWhiteSpace(status))
                query = query.Where(p => p.Status == status);

            var projects = await query.OrderBy(p => p.ProjectId).ToListAsync();

            using var workbook = new XLWorkbook();
            var ws = workbook.Worksheets.Add("Projects");

            AddReportHeader(ws, "Project Report", projects.Count, 6);

            var headers = new[] { "ID", "Project Name", "Client", "Start Date", "End Date", "Status" };
            AddHeaders(ws, headers, 3);

            for (int i = 0; i < projects.Count; i++)
            {
                var row = i + 4;
                ws.Cell(row, 1).Value = projects[i].ProjectId;
                ws.Cell(row, 2).Value = projects[i].ProjectName;
                ws.Cell(row, 3).Value = projects[i].Client?.ClientName ?? "N/A";
                ws.Cell(row, 4).Value = projects[i].StartDate?.ToString("yyyy-MM-dd") ?? "";
                ws.Cell(row, 5).Value = projects[i].EndDate?.ToString("yyyy-MM-dd") ?? "";
                ws.Cell(row, 6).Value = projects[i].Status;
            }

            FormatTable(ws, projects.Count, 6);
            return ToBytes(workbook);
        }

        public async Task<byte[]> ExportClientsToExcelAsync()
        {
            var clients = await _context.Clients.AsNoTracking().OrderBy(c => c.ClientId).ToListAsync();

            using var workbook = new XLWorkbook();
            var ws = workbook.Worksheets.Add("Clients");

            AddReportHeader(ws, "Client Directory", clients.Count, 5);

            var headers = new[] { "ID", "Client Name", "Phone", "Location", "Address" };
            AddHeaders(ws, headers, 3);

            for (int i = 0; i < clients.Count; i++)
            {
                var row = i + 4;
                ws.Cell(row, 1).Value = clients[i].ClientId;
                ws.Cell(row, 2).Value = clients[i].ClientName;
                ws.Cell(row, 3).Value = clients[i].ClientPhoneNumber;
                ws.Cell(row, 4).Value = clients[i].ClientLocation;
                ws.Cell(row, 5).Value = clients[i].ClientAddress;
            }

            FormatTable(ws, clients.Count, 5);
            return ToBytes(workbook);
        }

        private static void AddReportHeader(IXLWorksheet ws, string title, int recordCount, int colCount)
        {
            ws.Range(1, 1, 1, colCount).Merge();
            ws.Cell(1, 1).Value = title;
            ws.Cell(1, 1).Style.Font.Bold = true;
            ws.Cell(1, 1).Style.Font.FontSize = 16;
            ws.Cell(1, 1).Style.Font.FontColor = XLColor.FromHtml("#2563EB");
            ws.Cell(1, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
        }

        private static void AddHeaders(IXLWorksheet ws, string[] headers, int row)
        {
            for (int i = 0; i < headers.Length; i++)
            {
                ws.Cell(row, i + 1).Value = headers[i];
                ws.Cell(row, i + 1).Style.Font.Bold = true;
                ws.Cell(row, i + 1).Style.Fill.BackgroundColor = XLColor.FromHtml("#2563EB");
                ws.Cell(row, i + 1).Style.Font.FontColor = XLColor.White;
                ws.Cell(row, i + 1).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
            }
        }

        private static void FormatTable(IXLWorksheet ws, int dataRows, int colCount)
        {
            ws.Columns().AdjustToContents();
            var dataRange = ws.Range(4, 1, dataRows + 3, colCount);
            dataRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            dataRange.Style.Border.InsideBorder = XLBorderStyleValues.Thin;
        }

        private static byte[] ToBytes(XLWorkbook workbook)
        {
            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            return stream.ToArray();
        }
    }
}
