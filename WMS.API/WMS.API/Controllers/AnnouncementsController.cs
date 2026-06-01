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
    public class AnnouncementsController : ControllerBase
    {
        private readonly WMSDbContext _context;

        public AnnouncementsController(WMSDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Announcement>>> GetAnnouncements()
        {
            return await _context.Announcements
                .Include(a => a.CreatedByEmployee)
                .OrderByDescending(a => a.CreatedOn)
                .ToListAsync();
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<ActionResult<Announcement>> PostAnnouncement(Announcement announcement)
        {
            var userEmail = User.FindFirst(ClaimTypes.Name)?.Value;
            var employee = await _context.Employees.FirstOrDefaultAsync(e => e.Email == userEmail);

            announcement.CreatedOn = DateTime.Now;
            announcement.CreatedBy = employee != null ? employee.EmployeeId : 1;

            _context.Announcements.Add(announcement);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetAnnouncements), new { id = announcement.AnnouncementId }, announcement);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> PutAnnouncement(int id, Announcement announcement)
        {
            if (id != announcement.AnnouncementId) return BadRequest();

            _context.Entry(announcement).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return Ok(new { message = "Announcement updated!" });
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAnnouncement(int id)
        {
            var announcement = await _context.Announcements.FindAsync(id);
            if (announcement == null) return NotFound();

            _context.Announcements.Remove(announcement);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Announcement deleted!" });
        }
    }
}
