using Microsoft.AspNetCore.Mvc;
using WMS.API.Interfaces;
using WMS.API.Models;
using WMS.Domain.Models;

namespace WMS.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeesController : ControllerBase
    {
        private readonly IEmployeeService _employeeService;

        // The WMSDbContext is gone! We now strictly inject the Service layer.
        public EmployeesController(IEmployeeService employeeService)
        {
            _employeeService = employeeService;
        }

        // GET: api/Employees
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Employee>>> GetEmployees()
        {
            var employees = await _employeeService.GetAllEmployeesAsync();
            return Ok(employees);
        }

        // GET: api/Employees/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Employee>> GetEmployee(int id)
        {
            var employee = await _employeeService.GetEmployeeByIdAsync(id);

            if (employee == null)
                return NotFound();

            return Ok(employee);
        }

        // POST: api/Employees
        [HttpPost]
        public async Task<ActionResult<Employee>> PostEmployee(Employee employee)
        {
            // All the BCrypt hashing and UserLogin creation happens safely behind the scenes in the Service!
            var createdEmployee = await _employeeService.CreateEmployeeWithLoginAsync(employee);

            return CreatedAtAction(nameof(GetEmployee), new { id = createdEmployee.EmployeeId }, createdEmployee);
        }

        // PUT: api/Employees/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutEmployee(int id, Employee employee)
        {
            var success = await _employeeService.UpdateEmployeeAsync(id, employee);

            if (!success)
                return BadRequest(new { message = "Update failed. Check ID mismatch or if employee exists." });

            return Ok(new { message = "Employee updated successfully!" });
        }
    }
}