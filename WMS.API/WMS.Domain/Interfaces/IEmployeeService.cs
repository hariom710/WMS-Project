using WMS.API.Models;
using WMS.Domain.Models;

namespace WMS.API.Interfaces
{
    public interface IEmployeeService
    {
        Task<IEnumerable<Employee>> GetAllEmployeesAsync();
        Task<Employee?> GetEmployeeByIdAsync(int id);
        Task<Employee> CreateEmployeeWithLoginAsync(Employee employee);
        Task<bool> UpdateEmployeeAsync(int id, Employee employee);
    }
}