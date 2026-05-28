using WMS.API.Interfaces;
using WMS.Domain.Models;
using WMS.API.Models;

namespace WMS.API.Services
{
    public class EmployeeService : IEmployeeService
    {
        private readonly IEmployeeRepository _repository;

        public EmployeeService(IEmployeeRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<Employee>> GetAllEmployeesAsync() =>
            await _repository.GetAllEmployeesAsync();

        public async Task<Employee?> GetEmployeeByIdAsync(int id) =>
            await _repository.GetEmployeeByIdAsync(id);

        public async Task<Employee> CreateEmployeeWithLoginAsync(Employee employee)
        {
            var savedEmployee = await _repository.AddEmployeeAsync(employee);

            var employeeRole = await _repository.GetRoleByNameAsync("Employee");
            if (employeeRole == null)
            {
                employeeRole = new Role { RoleName = "Employee", Description = "Standard User" };
                await _repository.AddRoleAsync(employeeRole);
            }

            string hashedPassword = BCrypt.Net.BCrypt.HashPassword("Welcome@123");
            var newLogin = new UserLogin
            {
                Username = savedEmployee.Email,
                PasswordHash = hashedPassword,
                RoleId = employeeRole.RoleId
            };

            await _repository.AddUserLoginAsync(newLogin);

            return savedEmployee;
        }

        public async Task<bool> UpdateEmployeeAsync(int id, Employee employee)
        {
            if (id != employee.EmployeeId) return false;
            if (!await _repository.EmployeeExistsAsync(id)) return false;

            await _repository.UpdateEmployeeAsync(employee);
            return true;
        }
    }
}