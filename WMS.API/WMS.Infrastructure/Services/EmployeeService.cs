using WMS.Domain.Interfaces;
using WMS.Domain.Models;

namespace WMS.Infrastructure.Services
{
    public class EmployeeService : IEmployeeService
    {
        private readonly IEmployeeRepository _employeeRepo;
        private readonly IRoleRepository _roleRepo;
        private readonly IUserLoginRepository _userLoginRepo;
        private readonly IActivityLogService _activityLog;

        public EmployeeService(IEmployeeRepository employeeRepo, IRoleRepository roleRepo, IUserLoginRepository userLoginRepo, IActivityLogService activityLog)
        {
            _employeeRepo = employeeRepo;
            _roleRepo = roleRepo;
            _userLoginRepo = userLoginRepo;
            _activityLog = activityLog;
        }

        public async Task<PagedResult<Employee>> GetAllAsync(string? search, string? department, string? status, string? sortBy, string? sortDirection, int page, int pageSize) =>
            await _employeeRepo.GetAllAsync(search, department, status, sortBy, sortDirection, page, pageSize);

        public async Task<PagedResult<Employee>> GetDeletedAsync(string? search, string? sortBy, string? sortDirection, int page, int pageSize) =>
            await _employeeRepo.GetDeletedAsync(search, sortBy, sortDirection, page, pageSize);

        public async Task<Employee?> GetByIdAsync(int id) =>
            await _employeeRepo.GetByIdAsync(id);

        public async Task<Employee> CreateWithLoginAsync(Employee employee, string? createdBy)
        {
            employee.CreatedBy = createdBy;
            employee.CreatedDate = DateTime.UtcNow;
            await _employeeRepo.AddAsync(employee);

            var role = await _roleRepo.GetByNameAsync("Employee");
            if (role == null)
            {
                role = new Role { RoleName = "Employee", Description = "Standard User" };
                await _roleRepo.AddAsync(role);
            }

            var login = new UserLogin
            {
                Username = employee.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Welcome@123"),
                RoleId = role.RoleId
            };
            await _userLoginRepo.AddAsync(login);

            await _activityLog.LogAsync("Employee", employee.EmployeeId, "Create",
                $"Created employee {employee.FirstName} {employee.LastName} ({employee.Email})", createdBy, null, null);

            return employee;
        }

        public async Task<bool> UpdateAsync(int id, Employee employee, string? modifiedBy)
        {
            if (id != employee.EmployeeId) return false;
            if (!await _employeeRepo.ExistsAsync(id)) return false;
            employee.ModifiedBy = modifiedBy;
            employee.ModifiedDate = DateTime.UtcNow;
            await _employeeRepo.UpdateAsync(employee);

            await _activityLog.LogAsync("Employee", employee.EmployeeId, "Update",
                $"Updated employee {employee.FirstName} {employee.LastName}", modifiedBy, null, null);

            return true;
        }

        public async Task<bool> SoftDeleteAsync(int id, string? deletedBy)
        {
            var employee = await _employeeRepo.GetByIdAsync(id);
            if (employee == null) return false;
            await _employeeRepo.SoftDeleteAsync(employee, deletedBy);

            await _activityLog.LogAsync("Employee", id, "Delete",
                $"Deleted employee {employee.FirstName} {employee.LastName}", deletedBy, null, null);

            return true;
        }

        public async Task<bool> RestoreAsync(int id)
        {
            var employees = await _employeeRepo.GetDeletedAsync(null, null, null, 1, int.MaxValue);
            var employee = employees.Items.FirstOrDefault(e => e.EmployeeId == id);
            if (employee == null) return false;
            await _employeeRepo.RestoreAsync(employee);

            await _activityLog.LogAsync("Employee", id, "Restore",
                $"Restored employee {employee.FirstName} {employee.LastName}", null, null, null);

            return true;
        }

        public async Task<int> GetCountAsync() =>
            await _employeeRepo.GetCountAsync();
    }
}
