using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using WMS.API.Data;
using WMS.Domain.Models;

namespace WMS.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AllocationsController : ControllerBase
    {
        private readonly WMSDbContext _context;

        public AllocationsController(WMSDbContext context)
        {
            _context = context;
        }

        // GET: api/Allocations
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProjectAllocation>>> GetAllocations()
        {
            return await _context.ProjectAllocations
                .Include(a => a.Employee)
                .Include(a => a.Project)
                .OrderByDescending(a => a.CreateDate)
                .ToListAsync();
        }

        // POST: api/Allocations
        [HttpPost]
        public async Task<ActionResult<ProjectAllocation>> PostAllocation(ProjectAllocation allocation)
        {
            var userEmail = User.FindFirst(ClaimTypes.Name)?.Value;
            var admin = await _context.Employees.FirstOrDefaultAsync(e => e.Email == userEmail);

            allocation.CreateDate = DateTime.Now;
            allocation.CreatedBy = admin != null ? $"{admin.FirstName} {admin.LastName}" : "System Admin";
            allocation.Status = true;

            // Check if this exact employee is already assigned to this exact project to prevent duplicates
            var exists = await _context.ProjectAllocations
                .AnyAsync(a => a.EmpId == allocation.EmpId && a.ProjectId == allocation.ProjectId);

            if (exists) return BadRequest(new { message = "This employee is already assigned to this project." });

            _context.ProjectAllocations.Add(allocation);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetAllocations), new { id = allocation.AllocationId }, allocation);
        }

        // DELETE: api/Allocations/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAllocation(int id)
        {
            var allocation = await _context.ProjectAllocations.FindAsync(id);
            if (allocation == null) return NotFound();

            _context.ProjectAllocations.Remove(allocation);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Allocation removed successfully!" });
        }
    }
}