using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WMS.API.Data;
using WMS.Domain.Models;

namespace WMS.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ProjectsController : ControllerBase
    {
        private readonly WMSDbContext _context;

        public ProjectsController(WMSDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Project>>> GetProjects()
        {
            return await _context.Projects
                .Include(p => p.Client)
                .OrderByDescending(p => p.ProjectId)
                .ToListAsync();
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<ActionResult<Project>> PostProject(Project project)
        {
            if (project.ClientId == 0)
            {
                project.ClientId = null;
            }

            _context.Projects.Add(project);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetProjects), new { id = project.ProjectId }, project);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> PutProject(int id, Project project)
        {
            if (id != project.ProjectId) return BadRequest();

            if (project.ClientId == 0) project.ClientId = null;

            _context.Entry(project).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return Ok(new { message = "Project updated successfully!" });
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProject(int id)
        {
            var project = await _context.Projects.FindAsync(id);
            if (project == null) return NotFound();

            var hasAllocations = await _context.ProjectAllocations.AnyAsync(a => a.ProjectId == id);
            if (hasAllocations)
            {
                return BadRequest(new { message = "Cannot delete project with active allocations." });
            }

            _context.Projects.Remove(project);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Project deleted successfully!" });
        }
    }
}
