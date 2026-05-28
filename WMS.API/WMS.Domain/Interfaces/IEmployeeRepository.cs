using WMS.Domain.Models;
using WMS.API.Models; // For UserLogin

namespace WMS.API.Interfaces
{
    public interface IEmployeeRepository
    {
        Task<IEnumerable<Employee>> GetAllEmployeesAsync();
        Task<Employee?> GetEmployeeByIdAsync(int id);
        Task<Employee> AddEmployeeAsync(Employee employee);
        Task UpdateEmployeeAsync(Employee employee);
        Task<bool> EmployeeExistsAsync(int id);
        Task<Role?> GetRoleByNameAsync(string roleName);
        Task AddRoleAsync(Role role);
        Task AddUserLoginAsync(UserLogin login);
    }
}