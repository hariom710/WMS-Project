using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WMS.API.Data;
using WMS.API.Models;

namespace WMS.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserLoginsController : ControllerBase
    {
        private readonly WMSDbContext _context;

        public UserLoginsController(WMSDbContext context)
        {
            _context = context;
        }

        // GET: api/UserLogins
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserLogin>>> GetUserLogins()
        {
            // FIXED: Now we include the Role, which matches your model!
            return await _context.UserLogins
                .Include(u => u.Role)
                .ToListAsync();
        }

        // POST: api/UserLogins
        [HttpPost]
        public async Task<ActionResult<UserLogin>> PostUserLogin(UserLogin userLogin)
        {
            _context.UserLogins.Add(userLogin);
            await _context.SaveChangesAsync();

            // FIXED: Now we use UserId, which matches your model!
            return CreatedAtAction(nameof(GetUserLogins), new { id = userLogin.UserId }, userLogin);
        }
    }
}