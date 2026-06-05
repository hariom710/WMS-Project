using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WMS.API.Helpers;
using WMS.Application.DTOs;
using WMS.Domain.Interfaces;
using WMS.Domain.Models;
using AutoMapper;

namespace WMS.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class EmployeesController : ControllerBase
    {
        private readonly IEmployeeService _employeeService;
        private readonly ICurrentUserService _currentUser;
        private readonly IMapper _mapper;

        public EmployeesController(IEmployeeService employeeService, ICurrentUserService currentUser, IMapper mapper)
        {
            _employeeService = employeeService;
            _currentUser = currentUser;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetEmployees(
            [FromQuery] string? search, [FromQuery] string? department,
            [FromQuery] string? status, [FromQuery] string? sortBy,
            [FromQuery] string? sortDirection, [FromQuery] int page = 1, [FromQuery] int pageSize = 20)
        {
            var result = await _employeeService.GetAllAsync(search, department, status, sortBy, sortDirection, page, pageSize);
            var dtos = _mapper.Map<IEnumerable<EmployeeDto>>(result.Items);
            var pagination = new PaginationInfo { Page = result.Page, PageSize = result.PageSize, TotalCount = result.TotalCount };
            return Ok(ApiResponse<IEnumerable<EmployeeDto>>.Ok(dtos, pagination));
        }

        [HttpGet("deleted")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetDeletedEmployees(
            [FromQuery] string? search, [FromQuery] string? sortBy,
            [FromQuery] string? sortDirection, [FromQuery] int page = 1, [FromQuery] int pageSize = 20)
        {
            var result = await _employeeService.GetDeletedAsync(search, sortBy, sortDirection, page, pageSize);
            var dtos = _mapper.Map<IEnumerable<EmployeeDto>>(result.Items);
            var pagination = new PaginationInfo { Page = result.Page, PageSize = result.PageSize, TotalCount = result.TotalCount };
            return Ok(ApiResponse<IEnumerable<EmployeeDto>>.Ok(dtos, pagination));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetEmployee(int id)
        {
            var employee = await _employeeService.GetByIdAsync(id);
            if (employee == null) return NotFound(ApiResponse<object>.Fail("Employee not found."));
            return Ok(ApiResponse<EmployeeDto>.Ok(_mapper.Map<EmployeeDto>(employee)));
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> PostEmployee([FromBody] CreateEmployeeDto dto)
        {
            var employee = _mapper.Map<Employee>(dto);
            employee.FirstName = employee.FirstName?.Trim();
            employee.LastName = employee.LastName?.Trim();
            employee.Email = employee.Email?.Trim().ToLower();
            employee.PhoneNumber = employee.PhoneNumber?.Trim();

            var created = await _employeeService.CreateWithLoginAsync(employee, _currentUser.Username);
            return CreatedAtAction(nameof(GetEmployee), new { id = created.EmployeeId }, ApiResponse<EmployeeDto>.Ok(_mapper.Map<EmployeeDto>(created)));
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> PutEmployee(int id, [FromBody] UpdateEmployeeDto dto)
        {
            var employee = _mapper.Map<Employee>(dto);
            employee.EmployeeId = id;
            employee.FirstName = employee.FirstName?.Trim();
            employee.LastName = employee.LastName?.Trim();
            employee.Email = employee.Email?.Trim().ToLower();
            employee.PhoneNumber = employee.PhoneNumber?.Trim();

            var success = await _employeeService.UpdateAsync(id, employee, _currentUser.Username);
            if (!success) return BadRequest(ApiResponse<object>.Fail("Update failed."));
            return Ok(ApiResponse<object>.Ok(null!, "Employee updated successfully!"));
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteEmployee(int id)
        {
            var success = await _employeeService.SoftDeleteAsync(id, _currentUser.Username);
            if (!success) return NotFound(ApiResponse<object>.Fail("Employee not found."));
            return Ok(ApiResponse<object>.Ok(null!, "Employee deleted successfully!"));
        }

        [HttpPost("restore/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> RestoreEmployee(int id)
        {
            var success = await _employeeService.RestoreAsync(id);
            if (!success) return NotFound(ApiResponse<object>.Fail("Deleted employee not found."));
            return Ok(ApiResponse<object>.Ok(null!, "Employee restored successfully!"));
        }
    }
}
