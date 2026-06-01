using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WMS.API.Data;
using WMS.API.Interfaces;
using WMS.Domain.Models;

namespace WMS.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class EmployeesController : ControllerBase
    {
        private readonly IEmployeeService _employeeService;
        private readonly WMSDbContext _context;

        public EmployeesController(IEmployeeService employeeService, WMSDbContext context)
        {
            _employeeService = employeeService;
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Employee>>> GetEmployees(
            [FromQuery] string? search,
            [FromQuery] string? department,
            [FromQuery] string? status,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 50)
        {
            var query = _context.Employees
                .Include(e => e.Department)
                .Include(e => e.Role)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                var term = search.ToLower();
                query = query.Where(e =>
                    e.FirstName.ToLower().Contains(term) ||
                    e.LastName.ToLower().Contains(term) ||
                    e.Email.ToLower().Contains(term));
            }

            if (!string.IsNullOrWhiteSpace(department))
            {
                query = query.Where(e => e.Department != null && e.Department.DepartmentName.ToLower() == department.ToLower());
            }

            if (!string.IsNullOrWhiteSpace(status))
            {
                query = query.Where(e => e.Status.ToLower() == status.ToLower());
            }

            var total = await query.CountAsync();
            var employees = await query
                .OrderBy(e => e.FirstName)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            Response.Headers["X-Total-Count"] = total.ToString();
            Response.Headers["X-Page"] = page.ToString();
            Response.Headers["X-Page-Size"] = pageSize.ToString();

            return Ok(employees);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Employee>> GetEmployee(int id)
        {
            var employee = await _employeeService.GetEmployeeByIdAsync(id);

            if (employee == null)
                return NotFound();

            return Ok(employee);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<Employee>> PostEmployee(Employee employee)
        {
            var createdEmployee = await _employeeService.CreateEmployeeWithLoginAsync(employee);

            return CreatedAtAction(nameof(GetEmployee), new { id = createdEmployee.EmployeeId }, createdEmployee);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> PutEmployee(int id, Employee employee)
        {
            var success = await _employeeService.UpdateEmployeeAsync(id, employee);

            if (!success)
                return BadRequest(new { message = "Update failed. Check ID mismatch or if employee exists." });

            return Ok(new { message = "Employee updated successfully!" });
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteEmployee(int id)
        {
            var employee = await _context.Employees.FindAsync(id);
            if (employee == null)
                return NotFound(new { message = "Employee not found." });

            var hasAttendance = await _context.Attendances.AnyAsync(a => a.EmpId == id);
            if (hasAttendance)
            {
                return BadRequest(new { message = "Cannot delete employee with attendance records. Deactivate instead." });
            }

            var userLogin = await _context.UserLogins.FirstOrDefaultAsync(u => u.Username == employee.Email);
            if (userLogin != null)
            {
                _context.UserLogins.Remove(userLogin);
            }

            _context.Employees.Remove(employee);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Employee and associated login deleted successfully!" });
        }
    }
}
