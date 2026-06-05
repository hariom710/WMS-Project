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
    public class AnnouncementsController : ControllerBase
    {
        private readonly IAnnouncementService _announcementService;
        private readonly ICurrentUserService _currentUser;
        private readonly IMapper _mapper;

        public AnnouncementsController(IAnnouncementService announcementService, ICurrentUserService currentUser, IMapper mapper)
        {
            _announcementService = announcementService;
            _currentUser = currentUser;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetAnnouncements(
            [FromQuery] string? search, [FromQuery] bool? isActive,
            [FromQuery] string? sortBy, [FromQuery] string? sortDirection,
            [FromQuery] int page = 1, [FromQuery] int pageSize = 20)
        {
            var result = await _announcementService.GetAllAsync(search, isActive, sortBy, sortDirection, page, pageSize);
            var dtos = _mapper.Map<IEnumerable<AnnouncementDto>>(result.Items);
            var pagination = new PaginationInfo { Page = result.Page, PageSize = result.PageSize, TotalCount = result.TotalCount };
            return Ok(ApiResponse<IEnumerable<AnnouncementDto>>.Ok(dtos, pagination));
        }

        [HttpGet("deleted")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetDeletedAnnouncements(
            [FromQuery] string? search, [FromQuery] string? sortBy,
            [FromQuery] string? sortDirection, [FromQuery] int page = 1, [FromQuery] int pageSize = 20)
        {
            var result = await _announcementService.GetDeletedAsync(search, sortBy, sortDirection, page, pageSize);
            var dtos = _mapper.Map<IEnumerable<AnnouncementDto>>(result.Items);
            var pagination = new PaginationInfo { Page = result.Page, PageSize = result.PageSize, TotalCount = result.TotalCount };
            return Ok(ApiResponse<IEnumerable<AnnouncementDto>>.Ok(dtos, pagination));
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> PostAnnouncement([FromBody] CreateAnnouncementDto dto)
        {
            var announcement = _mapper.Map<Announcement>(dto);
            var userEmail = _currentUser.Username ?? "";
            var (success, message) = await _announcementService.CreateAsync(announcement, userEmail);
            return CreatedAtAction(nameof(GetAnnouncements), new { id = announcement.AnnouncementId }, ApiResponse<AnnouncementDto>.Ok(_mapper.Map<AnnouncementDto>(announcement)));
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> PutAnnouncement(int id, [FromBody] UpdateAnnouncementDto dto)
        {
            var announcement = _mapper.Map<Announcement>(dto);
            announcement.AnnouncementId = id;
            var userEmail = _currentUser.Username ?? "";
            var (success, message) = await _announcementService.UpdateAsync(id, announcement, userEmail);
            return Ok(ApiResponse<object>.Ok(null!, message));
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> SoftDeleteAnnouncement(int id)
        {
            var userEmail = _currentUser.Username ?? "";
            var (success, message) = await _announcementService.SoftDeleteAsync(id, userEmail);
            if (!success) return NotFound(ApiResponse<object>.Fail(message));
            return Ok(ApiResponse<object>.Ok(null!, message));
        }

        [HttpPost("restore/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> RestoreAnnouncement(int id)
        {
            var success = await _announcementService.RestoreAsync(id);
            if (!success) return NotFound(ApiResponse<object>.Fail("Deleted announcement not found."));
            return Ok(ApiResponse<object>.Ok(null!, "Announcement restored successfully!"));
        }
    }
}
