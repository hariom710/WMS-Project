using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WMS.API.Data;
using WMS.API.Models;
using System; // Required for DateTime

namespace WMS.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DepartmentsController : ControllerBase
    {
        private readonly WMSDbContext _context;

        public DepartmentsController(WMSDbContext context)
        {
            _context = context;
        }

        // GET: api/Departments
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Department>>> GetDepartments()
        {
            return await _context.Departments.ToListAsync();
        }

        // POST: api/Departments
        [HttpPost]
        public async Task<ActionResult<Department>> PostDepartment(Department department)
        {
            // ENTERPRISE FIX: Prevent SQL Server DateTime crash by setting the time right now!
            department.CreatedOn = DateTime.Now;

            _context.Departments.Add(department);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetDepartments), new { id = department.DepartmentId }, department);
        }

        // PUT: api/Departments/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutDepartment(int id, Department department)
        {
            if (id != department.DepartmentId)
            {
                return BadRequest(new { message = "ID mismatch." });
            }

            // Ensure the creation date is preserved during an update
            _context.Entry(department).State = EntityState.Modified;

            // We tell EF Core NOT to modify the CreatedOn column during an update
            _context.Entry(department).Property(x => x.CreatedOn).IsModified = false;

            await _context.SaveChangesAsync();

            return Ok(new { message = "Department updated successfully!" });
        }

        // DELETE: api/Departments/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDepartment(int id)
        {
            var department = await _context.Departments.FindAsync(id);
            if (department == null)
            {
                return NotFound();
            }

            // ENTERPRISE SAFEGUARD: Prevent deletion if employees are assigned
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