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
    public class LeavesController : ControllerBase
    {
        private readonly ILeaveService _leaveService;
        private readonly ICurrentUserService _currentUser;
        private readonly IMapper _mapper;

        public LeavesController(ILeaveService leaveService, ICurrentUserService currentUser, IMapper mapper)
        {
            _leaveService = leaveService;
            _currentUser = currentUser;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetLeaves(
            [FromQuery] string? search, [FromQuery] string? status,
            [FromQuery] string? sortBy, [FromQuery] string? sortDirection,
            [FromQuery] int page = 1, [FromQuery] int pageSize = 20)
        {
            var result = await _leaveService.GetAllAsync(search, status, sortBy, sortDirection, page, pageSize);
            var dtos = _mapper.Map<IEnumerable<LeaveDto>>(result.Items);
            var pagination = new PaginationInfo { Page = result.Page, PageSize = result.PageSize, TotalCount = result.TotalCount };
            return Ok(ApiResponse<IEnumerable<LeaveDto>>.Ok(dtos, pagination));
        }

        [HttpGet("deleted")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetDeletedLeaves(
            [FromQuery] string? search, [FromQuery] string? sortBy,
            [FromQuery] string? sortDirection, [FromQuery] int page = 1, [FromQuery] int pageSize = 20)
        {
            var result = await _leaveService.GetDeletedAsync(search, sortBy, sortDirection, page, pageSize);
            var dtos = _mapper.Map<IEnumerable<LeaveDto>>(result.Items);
            var pagination = new PaginationInfo { Page = result.Page, PageSize = result.PageSize, TotalCount = result.TotalCount };
            return Ok(ApiResponse<IEnumerable<LeaveDto>>.Ok(dtos, pagination));
        }

        [HttpGet("pending")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> GetPendingLeaves()
        {
            var leaves = await _leaveService.GetPendingAsync();
            return Ok(ApiResponse<IEnumerable<LeaveDto>>.Ok(_mapper.Map<IEnumerable<LeaveDto>>(leaves)));
        }

        [HttpPost]
        public async Task<IActionResult> PostLeave([FromBody] CreateLeaveDto dto)
        {
            var leave = _mapper.Map<Leave>(dto);
            var userEmail = _currentUser.Username ?? "";
            var (success, message) = await _leaveService.CreateAsync(leave, userEmail);
            if (!success) return BadRequest(ApiResponse<object>.Fail(message));
            return CreatedAtAction(nameof(GetLeaves), new { id = leave.LeaveId }, ApiResponse<LeaveDto>.Ok(_mapper.Map<LeaveDto>(leave)));
        }

        [HttpPut("approve/{id}")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> ApproveLeave(int id)
        {
            var userEmail = _currentUser.Username ?? "";
            var (success, message) = await _leaveService.ApproveAsync(id, userEmail);
            if (!success) return NotFound(ApiResponse<object>.Fail(message));
            return Ok(ApiResponse<object>.Ok(null!, message));
        }

        [HttpPut("reject/{id}")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> RejectLeave(int id, [FromBody] RejectRequestDto? request)
        {
            var userEmail = _currentUser.Username ?? "";
            var (success, message) = await _leaveService.RejectAsync(id, request?.Reason, userEmail);
            if (!success) return NotFound(ApiResponse<object>.Fail(message));
            return Ok(ApiResponse<object>.Ok(null!, message));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> SoftDeleteLeave(int id)
        {
            var userEmail = _currentUser.Username ?? "";
            var (success, message) = await _leaveService.SoftDeleteAsync(id, userEmail);
            if (!success) return BadRequest(ApiResponse<object>.Fail(message));
            return Ok(ApiResponse<object>.Ok(null!, message));
        }

        [HttpPost("restore/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> RestoreLeave(int id)
        {
            var success = await _leaveService.RestoreAsync(id);
            if (!success) return NotFound(ApiResponse<object>.Fail("Deleted leave not found."));
            return Ok(ApiResponse<object>.Ok(null!, "Leave restored successfully!"));
        }
    }

    public class RejectRequestDto
    {
        public string? Reason { get; set; }
    }
}
