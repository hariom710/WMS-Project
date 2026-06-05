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
    public class ProjectsController : ControllerBase
    {
        private readonly IProjectService _projectService;
        private readonly ICurrentUserService _currentUser;
        private readonly IMapper _mapper;

        public ProjectsController(IProjectService projectService, ICurrentUserService currentUser, IMapper mapper)
        {
            _projectService = projectService;
            _currentUser = currentUser;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetProjects(
            [FromQuery] string? search, [FromQuery] string? status,
            [FromQuery] int? clientId, [FromQuery] string? sortBy,
            [FromQuery] string? sortDirection, [FromQuery] int page = 1, [FromQuery] int pageSize = 20)
        {
            var result = await _projectService.GetAllAsync(search, status, clientId, sortBy, sortDirection, page, pageSize);
            var dtos = _mapper.Map<IEnumerable<ProjectDto>>(result.Items);
            var pagination = new PaginationInfo { Page = result.Page, PageSize = result.PageSize, TotalCount = result.TotalCount };
            return Ok(ApiResponse<IEnumerable<ProjectDto>>.Ok(dtos, pagination));
        }

        [HttpGet("deleted")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetDeletedProjects(
            [FromQuery] string? search, [FromQuery] string? sortBy,
            [FromQuery] string? sortDirection, [FromQuery] int page = 1, [FromQuery] int pageSize = 20)
        {
            var result = await _projectService.GetDeletedAsync(search, sortBy, sortDirection, page, pageSize);
            var dtos = _mapper.Map<IEnumerable<ProjectDto>>(result.Items);
            var pagination = new PaginationInfo { Page = result.Page, PageSize = result.PageSize, TotalCount = result.TotalCount };
            return Ok(ApiResponse<IEnumerable<ProjectDto>>.Ok(dtos, pagination));
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> PostProject([FromBody] CreateProjectDto dto)
        {
            var project = _mapper.Map<Project>(dto);
            await _projectService.CreateAsync(project, _currentUser.Username);
            return CreatedAtAction(nameof(GetProjects), new { id = project.ProjectId }, ApiResponse<ProjectDto>.Ok(_mapper.Map<ProjectDto>(project)));
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> PutProject(int id, [FromBody] UpdateProjectDto dto)
        {
            var project = _mapper.Map<Project>(dto);
            project.ProjectId = id;
            await _projectService.UpdateAsync(id, project, _currentUser.Username);
            return Ok(ApiResponse<object>.Ok(null!, "Project updated successfully!"));
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteProject(int id)
        {
            var (success, message) = await _projectService.SoftDeleteAsync(id, _currentUser.Username);
            if (!success) return BadRequest(ApiResponse<object>.Fail(message));
            return Ok(ApiResponse<object>.Ok(null!, message));
        }

        [HttpPost("restore/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> RestoreProject(int id)
        {
            var success = await _projectService.RestoreAsync(id);
            if (!success) return NotFound(ApiResponse<object>.Fail("Deleted project not found."));
            return Ok(ApiResponse<object>.Ok(null!, "Project restored successfully!"));
        }
    }
}
