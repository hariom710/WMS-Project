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
    public class DepartmentsController : ControllerBase
    {
        private readonly WMSDbContext _context;

        public DepartmentsController(WMSDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Department>>> GetDepartments(
            [FromQuery] string? search)
        {
            var query = _context.Departments.AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                var term = search.ToLower();
                query = query.Where(d => d.DepartmentName.ToLower().Contains(term));
            }

            return await query.OrderBy(d => d.DepartmentName).ToListAsync();
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<ActionResult<Department>> PostDepartment(Department department)
        {
            department.CreatedOn = DateTime.Now;
            _context.Departments.Add(department);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetDepartments), new { id = department.DepartmentId }, department);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> PutDepartment(int id, Department department)
        {
            if (id != department.DepartmentId)
            {
                return BadRequest(new { message = "ID mismatch." });
            }

            _context.Entry(department).State = EntityState.Modified;
            _context.Entry(department).Property(x => x.CreatedOn).IsModified = false;
            await _context.SaveChangesAsync();

            return Ok(new { message = "Department updated successfully!" });
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDepartment(int id)
        {
            var department = await _context.Departments.FindAsync(id);
            if (department == null)
            {
                return NotFound();
            }

            var hasEmployees = await _context.Employees.AnyAsync(e => e.DepartmentId == id);
            if (hasEmployees)
            {
                return BadRequest(new { message = "Cannot delete this department because employees are currently assigned to it." });
            }

            _context.Departments.Remove(department);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Department deleted successfully!" });
        }
    }
}
