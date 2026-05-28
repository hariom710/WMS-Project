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
    public class LeavesController : ControllerBase
    {
        private readonly WMSDbContext _context;

        public LeavesController(WMSDbContext context)
        {
            _context = context;
        }

        // GET: api/Leaves (My History)
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Leave>>> GetLeaves()
        {
            return await _context.Leaves.Include(l => l.Employee).OrderByDescending(l => l.AppliedOn).ToListAsync();
        }

        // GET: api/Leaves/pending (For Managers)
        [HttpGet("pending")]
        public async Task<ActionResult<IEnumerable<Leave>>> GetPendingLeaves()
        {
            return await _context.Leaves
                .Include(l => l.Employee)
                .Where(l => l.Status == "Pending")
                .OrderBy(l => l.AppliedOn)
                .ToListAsync();
        }

        // POST: api/Leaves
        [HttpPost]
        public async Task<ActionResult<Leave>> PostLeave(Leave leave)
        {
            var userEmail = User.FindFirst(ClaimTypes.Name)?.Value;
            var employee = await _context.Employees.FirstOrDefaultAsync(e => e.Email == userEmail);

            leave.EmpId = employee != null ? employee.EmployeeId : 1;
            leave.AppliedOn = DateTime.Now;
            leave.Status = "Pending";

            _context.Leaves.Add(leave);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetLeaves), new { id = leave.LeaveId }, leave);
        }

        // PUT: api/Leaves/approve/5
        [HttpPut("approve/{id}")]
        public async Task<IActionResult> ApproveLeave(int id)
        {
            var leave = await _context.Leaves.FindAsync(id);
            if (leave == null) return NotFound();

            var userEmail = User.FindFirst(ClaimTypes.Name)?.Value;
            var manager = await _context.Employees.FirstOrDefaultAsync(e => e.Email == userEmail);

            leave.Status = "Approved";
            leave.ApprovedBy = manager?.EmployeeId ?? 1;
            leave.ApprovedOn = DateTime.Now;

            await _context.SaveChangesAsync();
            return Ok(new { message = "Leave Approved!" });
        }

        // PUT: api/Leaves/reject/5
        [HttpPut("reject/{id}")]
        public async Task<IActionResult> RejectLeave(int id)
        {
            var leave = await _context.Leaves.FindAsync(id);
            if (leave == null) return NotFound();

            var userEmail = User.FindFirst(ClaimTypes.Name)?.Value;
            var manager = await _context.Employees.FirstOrDefaultAsync(e => e.Email == userEmail);

            leave.Status = "Rejected";
            leave.ApprovedBy = manager?.EmployeeId ?? 1;
            leave.ApprovedOn = DateTime.Now;

            await _context.SaveChangesAsync();
            return Ok(new { message = "Leave Rejected!" });
        }

        // DELETE: api/Leaves/5 (Cancel Leave)
        [HttpDelete("{id}")]
        public async Task<IActionResult> CancelLeave(int id)
        {
            var leave = await _context.Leaves.FindAsync(id);
            if (leave == null) return NotFound();

            if (leave.Status != "Pending")
            {
                return BadRequest(new { message = "You can only cancel pending leaves." });
            }

            _context.Leaves.Remove(leave);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Leave cancelled successfully!" });
        }
    }
}