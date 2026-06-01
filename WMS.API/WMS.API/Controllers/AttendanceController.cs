using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Fluent;
using System.Security.Claims;
using WMS.API.Data;
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

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Attendance>>> GetAttendances()
        {
            return await _context.Attendances.Include(a => a.Employee).ToListAsync();
        }

        [Authorize(Roles = "Admin")]
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

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<ActionResult<Attendance>> PostAttendance(Attendance attendance)
        {
            attendance.AttendanceDate = DateTime.Today;
            attendance.CheckIn = DateTime.Now;

            _context.Attendances.Add(attendance);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Clocked in successfully!" });
        }

        [Authorize(Roles = "Admin")]
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
        [Authorize(Roles = "Admin")]
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
        [Authorize(Roles = "Admin")]
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

            var employee = records.First().Employee;
            var employeeName = employee.FirstName + " " + employee.LastName;

            // Compute summary statistics
            int totalDaysWorked = records.Count(r => r.CheckOut.HasValue);
            double totalHoursWorked = (float)records.Where(r => r.TotalHours.HasValue).Sum(r => r.TotalHours);
            double avgHoursPerDay = totalDaysWorked > 0 ? Math.Round(totalHoursWorked / totalDaysWorked, 2) : 0;
            var modeBreakdown = records.GroupBy(r => r.WorkMode ?? "Office")
                                       .ToDictionary(g => g.Key, g => g.Count());

            var document = QuestPDF.Fluent.Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(QuestPDF.Helpers.PageSizes.A4);
                    page.Margin(2, QuestPDF.Infrastructure.Unit.Centimetre);
                    page.PageColor(QuestPDF.Helpers.Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(11));

                    // ── HEADER: Company Branding ──
                    page.Header().PaddingBottom(10).Column(col =>
                    {
                        col.Item().Row(row =>
                        {
                            row.RelativeItem().Column(c =>
                            {
                                c.Item().Text("WMS - Workforce Management System").SemiBold().FontSize(20).FontColor(QuestPDF.Helpers.Colors.Blue.Darken3);
                                c.Item().Text("Capgemini Project Portal").FontSize(10).FontColor(QuestPDF.Helpers.Colors.Grey.Medium);
                            });
                            row.ConstantItem(120).AlignRight().Column(c =>
                            {
                                c.Item().Text($"Date: {DateTime.Now:MMM dd, yyyy}").FontSize(10).FontColor(QuestPDF.Helpers.Colors.Grey.Darken2);
                                c.Item().Text($"Ref: TS-{empId}-{DateTime.Now:yyyyMM}").FontSize(10).FontColor(QuestPDF.Helpers.Colors.Grey.Darken2);
                            });
                        });
                        col.Item().PaddingTop(8).LineHorizontal(1).LineColor(QuestPDF.Helpers.Colors.Blue.Darken2);
                    });

                    // ── EMPLOYEE INFO ──
                    page.Content().Column(col =>
                    {
                        col.Item().PaddingBottom(12).Row(row =>
                        {
                            row.RelativeItem().Column(c =>
                            {
                                c.Item().Text($"Employee: {employeeName}").SemiBold().FontSize(14).FontColor(QuestPDF.Helpers.Colors.Grey.Darken4);
                                c.Item().Text($"Email: {employee.Email}").FontSize(11).FontColor(QuestPDF.Helpers.Colors.Grey.Darken2);
                            });
                            row.ConstantItem(160).Column(c =>
                            {
                                c.Item().Background(QuestPDF.Helpers.Colors.Grey.Lighten4).Padding(8).Column(sum =>
                                {
                                    sum.Item().Text("Summary").SemiBold().FontSize(11).FontColor(QuestPDF.Helpers.Colors.Blue.Darken2);
                                    sum.Item().Text($"Days Worked: {totalDaysWorked}").FontSize(10);
                                    sum.Item().Text($"Total Hours: {totalHoursWorked:F1}").FontSize(10);
                                    sum.Item().Text($"Avg / Day: {avgHoursPerDay} hrs").FontSize(10);
                                });
                            });
                        });

                        // ── WORK MODE BREAKDOWN ──
                        col.Item().PaddingBottom(10).Row(row =>
                        {
                            foreach (var mode in modeBreakdown)
                            {
                                row.RelativeItem().Border(1).BorderColor(QuestPDF.Helpers.Colors.Grey.Lighten2).Padding(8).Column(c =>
                                {
                                    c.Item().AlignCenter().Text(mode.Key).SemiBold().FontSize(11).FontColor(QuestPDF.Helpers.Colors.Blue.Darken2);
                                    c.Item().AlignCenter().Text(mode.Value.ToString()).FontSize(18).FontColor(QuestPDF.Helpers.Colors.Grey.Darken4);
                                });
                            }
                        });

                        // ── ATTENDANCE TABLE ──
                        col.Item().PaddingBottom(4).Text("Attendance Log").SemiBold().FontSize(13).FontColor(QuestPDF.Helpers.Colors.Grey.Darken3);

                        col.Item().Table(table =>
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
                                header.Cell().Background(QuestPDF.Helpers.Colors.Blue.Darken2).BorderBottom(1).Padding(5).Text("Date").SemiBold().FontColor(QuestPDF.Helpers.Colors.White);
                                header.Cell().Background(QuestPDF.Helpers.Colors.Blue.Darken2).BorderBottom(1).Padding(5).Text("Work Mode").SemiBold().FontColor(QuestPDF.Helpers.Colors.White);
                                header.Cell().Background(QuestPDF.Helpers.Colors.Blue.Darken2).BorderBottom(1).Padding(5).Text("Check-In").SemiBold().FontColor(QuestPDF.Helpers.Colors.White);
                                header.Cell().Background(QuestPDF.Helpers.Colors.Blue.Darken2).BorderBottom(1).Padding(5).Text("Check-Out").SemiBold().FontColor(QuestPDF.Helpers.Colors.White);
                                header.Cell().Background(QuestPDF.Helpers.Colors.Blue.Darken2).BorderBottom(1).Padding(5).Text("Total Hrs").SemiBold().FontColor(QuestPDF.Helpers.Colors.White);
                            });

                            foreach (var record in records)
                            {
                                table.Cell().BorderBottom(1).BorderColor(QuestPDF.Helpers.Colors.Grey.Lighten2).Padding(5).Text(record.AttendanceDate.ToShortDateString());
                                table.Cell().BorderBottom(1).BorderColor(QuestPDF.Helpers.Colors.Grey.Lighten2).Padding(5).Text(record.WorkMode ?? "Office");
                                table.Cell().BorderBottom(1).BorderColor(QuestPDF.Helpers.Colors.Grey.Lighten2).Padding(5).Text(record.CheckIn.ToString("hh:mm tt"));
                                table.Cell().BorderBottom(1).BorderColor(QuestPDF.Helpers.Colors.Grey.Lighten2).Padding(5).Text(record.CheckOut?.ToString("hh:mm tt") ?? "Pending");
                                table.Cell().BorderBottom(1).BorderColor(QuestPDF.Helpers.Colors.Grey.Lighten2).Padding(5).AlignRight().Text(record.TotalHours.HasValue ? $"{record.TotalHours:F1}" : "-");
                            }
                        });

                        // ── FOOTER NOTE ──
                        col.Item().PaddingTop(12).Text("This is a system-generated timesheet report.").FontSize(9).FontColor(QuestPDF.Helpers.Colors.Grey.Medium).Italic();
                    });

                    page.Footer().AlignCenter().Text(x =>
                    {
                        x.Span("Page ");
                        x.CurrentPageNumber();
                        x.Span(" of ");
                        x.TotalPages();
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