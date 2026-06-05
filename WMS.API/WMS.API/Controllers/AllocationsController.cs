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
    public class AllocationsController : ControllerBase
    {
        private readonly IAllocationService _allocationService;
        private readonly ICurrentUserService _currentUser;
        private readonly IMapper _mapper;

        public AllocationsController(IAllocationService allocationService, ICurrentUserService currentUser, IMapper mapper)
        {
            _allocationService = allocationService;
            _currentUser = currentUser;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllocations(
            [FromQuery] string? search, [FromQuery] string? sortBy,
            [FromQuery] string? sortDirection, [FromQuery] int page = 1, [FromQuery] int pageSize = 20)
        {
            var result = await _allocationService.GetAllAsync(search, sortBy, sortDirection, page, pageSize);
            var dtos = _mapper.Map<IEnumerable<AllocationDto>>(result.Items);
            var pagination = new PaginationInfo { Page = result.Page, PageSize = result.PageSize, TotalCount = result.TotalCount };
            return Ok(ApiResponse<IEnumerable<AllocationDto>>.Ok(dtos, pagination));
        }

        [HttpGet("deleted")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetDeletedAllocations(
            [FromQuery] string? search, [FromQuery] string? sortBy,
            [FromQuery] string? sortDirection, [FromQuery] int page = 1, [FromQuery] int pageSize = 20)
        {
            var result = await _allocationService.GetDeletedAsync(search, sortBy, sortDirection, page, pageSize);
            var dtos = _mapper.Map<IEnumerable<AllocationDto>>(result.Items);
            var pagination = new PaginationInfo { Page = result.Page, PageSize = result.PageSize, TotalCount = result.TotalCount };
            return Ok(ApiResponse<IEnumerable<AllocationDto>>.Ok(dtos, pagination));
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> PostAllocation([FromBody] CreateAllocationDto dto)
        {
            var allocation = _mapper.Map<ProjectAllocation>(dto);
            var userEmail = _currentUser.Username ?? "";
            var (success, message) = await _allocationService.CreateAsync(allocation, userEmail);
            if (!success) return BadRequest(ApiResponse<object>.Fail(message));
            return CreatedAtAction(nameof(GetAllocations), new { id = allocation.AllocationId }, ApiResponse<AllocationDto>.Ok(_mapper.Map<AllocationDto>(allocation)));
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> SoftDeleteAllocation(int id)
        {
            var userEmail = _currentUser.Username ?? "";
            var (success, message) = await _allocationService.SoftDeleteAsync(id, userEmail);
            if (!success) return NotFound(ApiResponse<object>.Fail(message));
            return Ok(ApiResponse<object>.Ok(null!, message));
        }

        [HttpPost("restore/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> RestoreAllocation(int id)
        {
            var success = await _allocationService.RestoreAsync(id);
            if (!success) return NotFound(ApiResponse<object>.Fail("Deleted allocation not found."));
            return Ok(ApiResponse<object>.Ok(null!, "Allocation restored successfully!"));
        }
    }
}
