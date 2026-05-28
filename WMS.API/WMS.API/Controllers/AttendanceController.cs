using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Fluent;
using System.Security.Claims;
using WMS.API.Data;
using WMS.API.Models;
using WMS.Domain.Models;

namespace WMS.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] // Requires a valid JWT token!
    public class AttendanceController : ControllerBase
    {
        private readonly WMSDbContext _context;

        public AttendanceController(WMSDbContext context)
        {
            _context = context;
        }

        // ==========================================
        // ADMIN ENDPOINTS (Used by your Angular UI)
        // ==========================================

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Attendance>>> GetAttendances()
        {
            return await _context.Attendances.Include(a => a.Employee).ToListAsync();
        }

        [HttpGet("monthly")]
        public async Task<ActionResult<IEnumerable<Attendance>>> GetMonthlyAttendance([FromQuery] int month, [FromQuery] int year)
        {
            var startDate = new DateTime(year, month, 1);
            var endDate = startDate.AddMonths(1).AddDays(-1);

            var monthlyData = await _context.Attendances
                .Include(a => a.Employee)
                .Where(a => a.AttendanceDate.Date >= startDate.Date && a.AttendanceDate.Date <= endDate.Date)
                .OrderByDescending(a => a.AttendanceDate)
                .ToListAsync();

            return Ok(monthlyData);
        }

        [HttpPost]
        public async Task<ActionResult<Attendance>> PostAttendance(Attendance attendance)
        {
            attendance.AttendanceDate = DateTime.Today;
            attendance.CheckIn = DateTime.Now;

            _context.Attendances.Add(attendance);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Clocked in successfully!" });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutAttendance(int id, Attendance attendance)
        {
            if (id != attendance.AttendanceId) return BadRequest();
            _context.Entry(attendance).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        // ==========================================
        // TIMESHEET REPORTING (PDF GENERATOR)
        // ==========================================

        // GET: api/Attendance/timesheet/5 (Old JSON endpoint for reference)
        [HttpGet("timesheet/{empId}")]
        public async Task<IActionResult> GetTimesheet(int empId)
        {
            var records = await _context.Attendances
                .Include(a => a.Employee)
                .Where(a => a.EmpId == empId)
                .OrderByDescending(a => a.AttendanceDate)
                .Select(a => new {
                    EmployeeName = a.Employee.FirstName + " " + a.Employee.LastName,
                    a.AttendanceDate,
                    a.CheckIn,
                    a.CheckOut,
                    a.WorkMode,
                    a.TotalHours
                })
                .ToListAsync();

            if (!records.Any()) return NotFound(new { message = "No attendance records found for this employee." });
            return Ok(records);
        }

        // NEW: GET api/Attendance/timesheet/pdf/5 (Generates the QuestPDF File)
        [HttpGet("timesheet/pdf/{empId}")]
        public async Task<IActionResult> DownloadTimesheetPdf(int empId)
        {
            // QuestPDF Community License Configuration
            QuestPDF.Settings.License = QuestPDF.Infrastructure.LicenseType.Community;

            var records = await _context.Attendances
                .Include(a => a.Employee)
                .Where(a => a.EmpId == empId)
                .OrderByDescending(a => a.AttendanceDate)
                .ToListAsync();

            if (!records.Any()) return NotFound("No attendance records found.");

            var employeeName = records.First().Employee.FirstName + " " + records.First().Employee.LastName;

            var document = QuestPDF.Fluent.Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(QuestPDF.Helpers.PageSizes.A4);
                    page.Margin(2, QuestPDF.Infrastructure.Unit.Centimetre);
                    page.PageColor(QuestPDF.Helpers.Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(11));

                    page.Header().PaddingBottom(10).Column(col =>
                    {
                        col.Item().Text($"Timesheet Report").SemiBold().FontSize(24).FontColor(QuestPDF.Helpers.Colors.Blue.Darken2);
                        col.Item().Text($"Employee: {employeeName}").FontSize(14).FontColor(QuestPDF.Helpers.Colors.Grey.Darken3);
                        col.Item().Text($"Generated On: {DateTime.Now:MMM dd, yyyy}").FontSize(10).FontColor(QuestPDF.Helpers.Colors.Grey.Medium);
                    });

                    page.Content().Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.RelativeColumn();
                            columns.RelativeColumn();
                            columns.RelativeColumn();
                            columns.RelativeColumn();
                            columns.RelativeColumn();
                        });

                        table.Header(header =>
                        {
                            header.Cell().BorderBottom(1).Padding(5).Text("Date").SemiBold();
                            header.Cell().BorderBottom(1).Padding(5).Text("Work Mode").SemiBold();
                            header.Cell().BorderBottom(1).Padding(5).Text("Check-In").SemiBold();
                            header.Cell().BorderBottom(1).Padding(5).Text("Check-Out").SemiBold();
                            header.Cell().BorderBottom(1).Padding(5).Text("Total Hrs").SemiBold();
                        });

                        foreach (var record in records)
                        {
                            table.Cell().BorderBottom(1).BorderColor(QuestPDF.Helpers.Colors.Grey.Lighten2).Padding(5).Text(record.AttendanceDate.ToShortDateString());
                            table.Cell().BorderBottom(1).BorderColor(QuestPDF.Helpers.Colors.Grey.Lighten2).Padding(5).Text(record.WorkMode ?? "Office");
                            table.Cell().BorderBottom(1).BorderColor(QuestPDF.Helpers.Colors.Grey.Lighten2).Padding(5).Text(record.CheckIn.ToString("hh:mm tt"));
                            table.Cell().BorderBottom(1).BorderColor(QuestPDF.Helpers.Colors.Grey.Lighten2).Padding(5).Text(record.CheckOut?.ToString("hh:mm tt") ?? "Pending");
                            table.Cell().BorderBottom(1).BorderColor(QuestPDF.Helpers.Colors.Grey.Lighten2).Padding(5).Text(record.TotalHours.ToString());
                        }
                    });

                    page.Footer().AlignCenter().Text(x =>
                    {
                        x.Span("Page ");
                        x.CurrentPageNumber();
                    });
                });
            });

            byte[] pdfBytes = document.GeneratePdf();
            return File(pdfBytes, "application/pdf", $"Timesheet_{employeeName}.pdf");
        }

        // ==========================================
        // EMPLOYEE SELF-SERVICE ENDPOINTS
        // ==========================================

        [HttpGet("my-attendance")]
        public async Task<IActionResult> GetMyAttendance()
        {
            var userEmail = User.FindFirst(ClaimTypes.Name)?.Value;
            var employee = await _context.Employees.FirstOrDefaultAsync(e => e.Email == userEmail);
            if (employee == null) return NotFound("Employee profile not found.");

            var records = await _context.Attendances
                .Where(a => a.EmpId == employee.EmployeeId)
                .OrderByDescending(a => a.AttendanceDate)
                .ToListAsync();

            return Ok(records);
        }

        [HttpPost("check-in")]
        public async Task<IActionResult> CheckIn([FromBody] string workMode)
        {
            var userEmail = User.FindFirst(ClaimTypes.Name)?.Value;
            var employee = await _context.Employees.FirstOrDefaultAsync(e => e.Email == userEmail);
            if (employee == null) return BadRequest("Employee not found.");

            var today = DateTime.Today;
            var existing = await _context.Attendances.FirstOrDefaultAsync(a => a.EmpId == employee.EmployeeId && a.AttendanceDate == today);
            if (existing != null) return BadRequest(new { message = "You have already checked in today!" });

            var attendance = new Attendance
            {
                EmpId = employee.EmployeeId,
                CheckIn = DateTime.Now,
                AttendanceDate = today,
                WorkMode = workMode ?? "Office"
            };

            _context.Attendances.Add(attendance);
            await _context.SaveChangesAsync();
            return Ok(new { message = "Checked in successfully!" });
        }

        [HttpPut("check-out")]
        public async Task<IActionResult> CheckOut()
        {
            var userEmail = User.FindFirst(ClaimTypes.Name)?.Value;
            var employee = await _context.Employees.FirstOrDefaultAsync(e => e.Email == userEmail);
            if (employee == null) return BadRequest("Employee not found.");

            var today = DateTime.Today;
            var attendance = await _context.Attendances.FirstOrDefaultAsync(a => a.EmpId == employee.EmployeeId && a.AttendanceDate == today);

            if (attendance == null) return BadRequest(new { message = "No check-in record found for today." });
            if (attendance.CheckOut != null) return BadRequest(new { message = "You have already checked out today!" });

            attendance.CheckOut = DateTime.Now;
            TimeSpan duration = (DateTime)attendance.CheckOut - (DateTime)attendance.CheckIn;
            attendance.TotalHours = (float)Math.Round(duration.TotalHours, 2);

            await _context.SaveChangesAsync();
            return Ok(new { message = "Checked out successfully!" });
        }
    }
}