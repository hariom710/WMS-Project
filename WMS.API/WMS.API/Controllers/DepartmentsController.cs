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
    public class DepartmentsController : ControllerBase
    {
        private readonly IDepartmentService _departmentService;
        private readonly ICurrentUserService _currentUser;
        private readonly IMapper _mapper;

        public DepartmentsController(IDepartmentService departmentService, ICurrentUserService currentUser, IMapper mapper)
        {
            _departmentService = departmentService;
            _currentUser = currentUser;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetDepartments(
            [FromQuery] string? search, [FromQuery] string? sortBy,
            [FromQuery] string? sortDirection, [FromQuery] int page = 1, [FromQuery] int pageSize = 20)
        {
            var result = await _departmentService.GetAllAsync(search, sortBy, sortDirection, page, pageSize);
            var dtos = _mapper.Map<IEnumerable<DepartmentDto>>(result.Items);
            var pagination = new PaginationInfo { Page = result.Page, PageSize = result.PageSize, TotalCount = result.TotalCount };
            return Ok(ApiResponse<IEnumerable<DepartmentDto>>.Ok(dtos, pagination));
        }

        [HttpGet("deleted")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetDeletedDepartments(
            [FromQuery] string? search, [FromQuery] string? sortBy,
            [FromQuery] string? sortDirection, [FromQuery] int page = 1, [FromQuery] int pageSize = 20)
        {
            var result = await _departmentService.GetDeletedAsync(search, sortBy, sortDirection, page, pageSize);
            var dtos = _mapper.Map<IEnumerable<DepartmentDto>>(result.Items);
            var pagination = new PaginationInfo { Page = result.Page, PageSize = result.PageSize, TotalCount = result.TotalCount };
            return Ok(ApiResponse<IEnumerable<DepartmentDto>>.Ok(dtos, pagination));
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> PostDepartment([FromBody] CreateDepartmentDto dto)
        {
            var department = _mapper.Map<Department>(dto);
            await _departmentService.CreateAsync(department, _currentUser.Username);
            return CreatedAtAction(nameof(GetDepartments), new { id = department.DepartmentId }, ApiResponse<DepartmentDto>.Ok(_mapper.Map<DepartmentDto>(department)));
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> PutDepartment(int id, [FromBody] UpdateDepartmentDto dto)
        {
            var department = _mapper.Map<Department>(dto);
            department.DepartmentId = id;
            await _departmentService.UpdateAsync(id, department, _currentUser.Username);
            return Ok(ApiResponse<object>.Ok(null!, "Department updated successfully!"));
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteDepartment(int id)
        {
            var (success, message) = await _departmentService.SoftDeleteAsync(id, _currentUser.Username);
            if (!success) return BadRequest(ApiResponse<object>.Fail(message));
            return Ok(ApiResponse<object>.Ok(null!, message));
        }

        [HttpPost("restore/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> RestoreDepartment(int id)
        {
            var success = await _departmentService.RestoreAsync(id);
            if (!success) return NotFound(ApiResponse<object>.Fail("Deleted department not found."));
            return Ok(ApiResponse<object>.Ok(null!, "Department restored successfully!"));
        }
    }
}
