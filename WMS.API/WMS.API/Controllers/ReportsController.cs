using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WMS.Domain.Interfaces;

namespace WMS.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ReportsController : ControllerBase
    {
        private readonly IReportService _reportService;
        private readonly IPdfReportService _pdfReportService;

        public ReportsController(IReportService reportService, IPdfReportService pdfReportService)
        {
            _reportService = reportService;
            _pdfReportService = pdfReportService;
        }

        [HttpGet("employees/excel")]
        public async Task<IActionResult> ExportEmployeesExcel([FromQuery] string? search, [FromQuery] string? status)
        {
            var bytes = await _reportService.ExportEmployeesToExcelAsync(search, status);
            return File(bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"Employees_{DateTime.Now:yyyyMMdd}.xlsx");
        }

        [HttpGet("employees/pdf")]
        public async Task<IActionResult> ExportEmployeesPdf([FromQuery] string? search, [FromQuery] string? status)
        {
            var bytes = await _pdfReportService.ExportEmployeesPdfAsync(search, status);
            return File(bytes, "application/pdf", $"Employees_{DateTime.Now:yyyyMMdd}.pdf");
        }

        [HttpGet("attendance/excel")]
        public async Task<IActionResult> ExportAttendanceExcel([FromQuery] int? empId, [FromQuery] int? month, [FromQuery] int? year)
        {
            var bytes = await _reportService.ExportAttendanceToExcelAsync(empId, month, year);
            return File(bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"Attendance_{DateTime.Now:yyyyMMdd}.xlsx");
        }

        [HttpGet("attendance/pdf")]
        public async Task<IActionResult> ExportAttendancePdf([FromQuery] int? empId, [FromQuery] int? month, [FromQuery] int? year)
        {
            var bytes = await _pdfReportService.ExportAttendancePdfAsync(empId, month, year);
            return File(bytes, "application/pdf", $"Attendance_{DateTime.Now:yyyyMMdd}.pdf");
        }

        [HttpGet("leaves/excel")]
        public async Task<IActionResult> ExportLeavesExcel([FromQuery] string? status)
        {
            var bytes = await _reportService.ExportLeavesToExcelAsync(status);
            return File(bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"Leaves_{DateTime.Now:yyyyMMdd}.xlsx");
        }

        [HttpGet("leaves/pdf")]
        public async Task<IActionResult> ExportLeavesPdf([FromQuery] string? status)
        {
            var bytes = await _pdfReportService.ExportLeavesPdfAsync(status);
            return File(bytes, "application/pdf", $"Leaves_{DateTime.Now:yyyyMMdd}.pdf");
        }

        [HttpGet("projects/excel")]
        public async Task<IActionResult> ExportProjectsExcel([FromQuery] string? status)
        {
            var bytes = await _reportService.ExportProjectsToExcelAsync(status);
            return File(bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"Projects_{DateTime.Now:yyyyMMdd}.xlsx");
        }

        [HttpGet("projects/pdf")]
        public async Task<IActionResult> ExportProjectsPdf([FromQuery] string? status)
        {
            var bytes = await _pdfReportService.ExportProjectsPdfAsync(status);
            return File(bytes, "application/pdf", $"Projects_{DateTime.Now:yyyyMMdd}.pdf");
        }

        [HttpGet("clients/excel")]
        public async Task<IActionResult> ExportClientsExcel()
        {
            var bytes = await _reportService.ExportClientsToExcelAsync();
            return File(bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"Clients_{DateTime.Now:yyyyMMdd}.xlsx");
        }

        [HttpGet("dashboard/pdf")]
        public async Task<IActionResult> ExportDashboardPdf()
        {
            var bytes = await _pdfReportService.ExportDashboardPdfAsync();
            return File(bytes, "application/pdf", $"Dashboard_{DateTime.Now:yyyyMMdd}.pdf");
        }
    }
}
