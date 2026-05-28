using Microsoft.EntityFrameworkCore;
using WMS.API.Data;
using WMS.API.Interfaces;
using WMS.Domain.Models;
using WMS.API.Models;

namespace WMS.API.Repositories
{
    public class EmployeeRepository : IEmployeeRepository
    {
        private readonly WMSDbContext _context;

        public EmployeeRepository(WMSDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Employee>> GetAllEmployeesAsync()
        {
            return await _context.Employees
                .Include(e => e.Department)
                .Include(e => e.Role)
                .ToListAsync();
        }

        public async Task<Employee?> GetEmployeeByIdAsync(int id)
        {
            return await _context.Employees
                .Include(e => e.Department)
                .Include(e => e.Role)
                .FirstOrDefaultAsync(e => e.EmployeeId == id);
        }

        public async Task<Employee> AddEmployeeAsync(Employee employee)
        {
            _context.Employees.Add(employee);
            await _context.SaveChangesAsync();
            return employee;
        }

        public async Task UpdateEmployeeAsync(Employee employee)
        {
            _context.Entry(employee).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task<bool> EmployeeExistsAsync(int id)
        {
            return await _context.Employees.AnyAsync(e => e.EmployeeId == id);
        }

        public async Task<Role?> GetRoleByNameAsync(string roleName) =>
            await _context.Roles.FirstOrDefaultAsync(r => r.RoleName == roleName);

        public async Task AddRoleAsync(Role role)
        {
            _context.Roles.Add(role);
            await _context.SaveChangesAsync();
        }

        public async Task AddUserLoginAsync(UserLogin login)
        {
            _context.UserLogins.Add(login);
            await _context.SaveChangesAsync();
        }
    }
}