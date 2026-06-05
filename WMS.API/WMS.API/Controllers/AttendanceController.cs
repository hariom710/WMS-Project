using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WMS.API.Helpers;
using WMS.Application.DTOs;
using WMS.Domain.Interfaces;
using WMS.Domain.Models;
using System.Security.Claims;
using AutoMapper;

namespace WMS.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AttendanceController : ControllerBase
    {
        private readonly IAttendanceService _attendanceService;
        private readonly IEmployeeService _employeeService;
        private readonly ICurrentUserService _currentUser;
        private readonly IMapper _mapper;

        public AttendanceController(IAttendanceService attendanceService, IEmployeeService employeeService, ICurrentUserService currentUser, IMapper mapper)
        {
            _attendanceService = attendanceService;
            _employeeService = employeeService;
            _currentUser = currentUser;
            _mapper = mapper;
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> GetAttendances(
            [FromQuery] string? search, [FromQuery] int? empId,
            [FromQuery] int? month, [FromQuery] int? year,
            [FromQuery] string? sortBy, [FromQuery] string? sortDirection,
            [FromQuery] int page = 1, [FromQuery] int pageSize = 20)
        {
            var result = await _attendanceService.GetAllAsync(search, empId, month, year, sortBy, sortDirection, page, pageSize);
            var dtos = _mapper.Map<IEnumerable<AttendanceDto>>(result.Items);
            var pagination = new PaginationInfo { Page = result.Page, PageSize = result.PageSize, TotalCount = result.TotalCount };
            return Ok(ApiResponse<IEnumerable<AttendanceDto>>.Ok(dtos, pagination));
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> PostAttendance([FromBody] Attendance attendance)
        {
            var (success, message) = await _attendanceService.CreateAsync(attendance);
            if (!success) return BadRequest(ApiResponse<object>.Fail(message));
            return Ok(ApiResponse<object>.Ok(null!, message));
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> PutAttendance(int id, [FromBody] Attendance attendance)
        {
            if (id != attendance.AttendanceId) return BadRequest(ApiResponse<object>.Fail("ID mismatch."));
            var (success, message) = await _attendanceService.UpdateAsync(id, attendance);
            return NoContent();
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("timesheet/{empId}")]
        public async Task<IActionResult> GetTimesheet(int empId)
        {
            var employee = await _employeeService.GetByIdAsync(empId);
            if (employee == null) return NotFound(ApiResponse<object>.Fail("Employee not found."));
            return Ok(ApiResponse<object>.Ok(null!, "Timesheet endpoint"));
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("timesheet/pdf/{empId}")]
        public async Task<IActionResult> DownloadTimesheetPdf(int empId)
        {
            return Ok(ApiResponse<object>.Ok(null!, "PDF endpoint"));
        }

        [HttpGet("my-attendance")]
        public async Task<IActionResult> GetMyAttendance(
            [FromQuery] int? month, [FromQuery] int? year,
            [FromQuery] string? sortBy, [FromQuery] string? sortDirection,
            [FromQuery] int page = 1, [FromQuery] int pageSize = 20)
        {
            var userEmail = _currentUser.Username ?? "";
            var employee = await _employeeService.GetAllAsync(userEmail, null, null, null, null, 1, 1);
            var emp = employee.Items.FirstOrDefault();
            if (emp == null) return NotFound(ApiResponse<object>.Fail("Employee not found."));

            var result = await _attendanceService.GetAllAsync(null, emp.EmployeeId, month, year, sortBy, sortDirection, page, pageSize);
            var dtos = _mapper.Map<IEnumerable<AttendanceDto>>(result.Items);
            var pagination = new PaginationInfo { Page = result.Page, PageSize = result.PageSize, TotalCount = result.TotalCount };
            return Ok(ApiResponse<IEnumerable<AttendanceDto>>.Ok(dtos, pagination));
        }

        [HttpPost("check-in")]
        public async Task<IActionResult> CheckIn([FromBody] string workMode)
        {
            var userEmail = _currentUser.Username ?? "";
            var (success, message) = await _attendanceService.CheckInAsync(workMode, userEmail);
            if (!success) return BadRequest(ApiResponse<object>.Fail(message));
            return Ok(ApiResponse<object>.Ok(null!, message));
        }

        [HttpPut("check-out")]
        public async Task<IActionResult> CheckOut()
        {
            var userEmail = _currentUser.Username ?? "";
            var (success, message) = await _attendanceService.CheckOutAsync(userEmail);
            if (!success) return BadRequest(ApiResponse<object>.Fail(message));
            return Ok(ApiResponse<object>.Ok(null!, message));
        }
    }
}
