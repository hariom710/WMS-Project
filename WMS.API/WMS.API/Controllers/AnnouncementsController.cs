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
    [Authorize] // Ensure only logged-in users can post
    public class AnnouncementsController : ControllerBase
    {
        private readonly WMSDbContext _context;

        public AnnouncementsController(WMSDbContext context)
        {
            _context = context;
        }

        // GET: api/Announcements
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Announcement>>> GetAnnouncements()
        {
            return await _context.Announcements.OrderByDescending(a => a.CreatedOn).ToListAsync();
        }

        // POST: api/Announcements
        [HttpPost]
        public async Task<ActionResult<Announcement>> PostAnnouncement(Announcement announcement)
        {
            // 1. Get the currently logged-in user from the JWT Token
            var userEmail = User.FindFirst(ClaimTypes.Name)?.Value;
            var employee = await _context.Employees.FirstOrDefaultAsync(e => e.Email == userEmail);

            // 2. Fulfill the strict Capgemini database constraints!
            announcement.CreatedOn = DateTime.Now;

            // If we found the logged-in admin, use their ID. Otherwise, fallback to ID 1 to prevent crashes.
            announcement.CreatedBy = employee != null ? employee.EmployeeId : 1;

            _context.Announcements.Add(announcement);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetAnnouncements), new { id = announcement.AnnouncementId }, announcement);
        }

        // PUT: api/Announcements/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutAnnouncement(int id, Announcement announcement)
        {
            if (id != announcement.AnnouncementId) return BadRequest();

            _context.Entry(announcement).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return Ok(new { message = "Announcement updated!" });
        }

        // DELETE: api/Announcements/5
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