using Microsoft.EntityFrameworkCore;
using WMS.Domain.Interfaces;
using WMS.Domain.Models;
using WMS.Infrastructure.Data;

namespace WMS.Infrastructure.Repositories
{
    public class EmployeeRepository : IEmployeeRepository
    {
        private readonly WMSDbContext _context;

        public EmployeeRepository(WMSDbContext context) => _context = context;

        public async Task<PagedResult<Employee>> GetAllAsync(
            string? search, string? department, string? status,
            string? sortBy, string? sortDirection, int page, int pageSize)
        {
            var query = _context.Employees
                .Include(e => e.Department)
                .Include(e => e.Role)
                .AsQueryable();

            ApplySearchFilter(ref query, search, department, status);
            return await ApplyPaginationAsync(query, sortBy, sortDirection, page, pageSize);
        }

        public async Task<PagedResult<Employee>> GetDeletedAsync(
            string? search, string? sortBy, string? sortDirection, int page, int pageSize)
        {
            var query = _context.Employees
                .IgnoreQueryFilters()
                .Include(e => e.Department)
                .Include(e => e.Role)
                .Where(e => e.IsDeleted)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                var term = search.ToLower();
                query = query.Where(e =>
                    e.FirstName.ToLower().Contains(term) ||
                    e.LastName.ToLower().Contains(term) ||
                    e.Email.ToLower().Contains(term));
            }

            var totalCount = await query.CountAsync();

            query = sortBy?.ToLower() switch
            {
                "lastname" or "name" => sortDirection?.ToLower() == "desc"
                    ? query.OrderByDescending(e => e.LastName)
                    : query.OrderBy(e => e.LastName),
                "email" => sortDirection?.ToLower() == "desc"
                    ? query.OrderByDescending(e => e.Email)
                    : query.OrderBy(e => e.Email),
                "deleteddate" => sortDirection?.ToLower() == "desc"
                    ? query.OrderByDescending(e => e.DeletedDate)
                    : query.OrderBy(e => e.DeletedDate),
                _ => query.OrderByDescending(e => e.DeletedDate)
            };

            var items = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
            return new PagedResult<Employee>(items, totalCount, page, pageSize);
        }

        public async Task<Employee?> GetByIdAsync(int id) =>
            await _context.Employees.AsNoTracking()
                .Include(e => e.Department)
                .Include(e => e.Role)
                .FirstOrDefaultAsync(e => e.EmployeeId == id);

        public async Task<Employee?> GetByEmailAsync(string email) =>
            await _context.Employees.FirstOrDefaultAsync(e => e.Email == email);

        public async Task<bool> ExistsByEmailAsync(string email, int? excludeId = null) =>
            await _context.Employees.AnyAsync(e =>
                e.Email == email &&
                (!excludeId.HasValue || e.EmployeeId != excludeId.Value));

        public async Task<bool> ExistsByPhoneAsync(string phone, int? excludeId = null) =>
            await _context.Employees.AnyAsync(e =>
                e.PhoneNumber == phone &&
                (!excludeId.HasValue || e.EmployeeId != excludeId.Value));

        public async Task<bool> ExistsAsync(int id) =>
            await _context.Employees.AnyAsync(e => e.EmployeeId == id);

        public async Task<bool> HasAttendanceAsync(int id) =>
            await _context.Attendances.AnyAsync(a => a.EmpId == id);

        public async Task<int> GetCountAsync() =>
            await _context.Employees.CountAsync();

        public async Task AddAsync(Employee employee)
        {
            _context.Employees.Add(employee);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Employee employee)
        {
            _context.Entry(employee).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task SoftDeleteAsync(Employee employee, string? deletedBy)
        {
            employee.IsDeleted = true;
            employee.DeletedBy = deletedBy;
            employee.DeletedDate = DateTime.UtcNow;
            _context.Entry(employee).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task RestoreAsync(Employee employee)
        {
            employee.IsDeleted = false;
            employee.DeletedBy = null;
            employee.DeletedDate = null;
            _context.Entry(employee).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Employee employee)
        {
            _context.Employees.Remove(employee);
            await _context.SaveChangesAsync();
        }

        private void ApplySearchFilter(ref IQueryable<Employee> query, string? search, string? department, string? status)
        {
            if (!string.IsNullOrWhiteSpace(search))
            {
                var term = search.ToLower();
                query = query.Where(e =>
                    e.FirstName.ToLower().Contains(term) ||
                    e.LastName.ToLower().Contains(term) ||
                    e.Email.ToLower().Contains(term));
            }

            if (!string.IsNullOrWhiteSpace(department))
                query = query.Where(e => e.Department != null && e.Department.DepartmentName.ToLower() == department.ToLower());

            if (!string.IsNullOrWhiteSpace(status))
                query = query.Where(e => e.Status.ToLower() == status.ToLower());
        }

        private async Task<PagedResult<Employee>> ApplyPaginationAsync(
            IQueryable<Employee> query, string? sortBy, string? sortDirection, int page, int pageSize)
        {
            var totalCount = await query.CountAsync();

            query = sortBy?.ToLower() switch
            {
                "lastname" or "name" => sortDirection?.ToLower() == "desc"
                    ? query.OrderByDescending(e => e.LastName)
                    : query.OrderBy(e => e.LastName),
                "email" => sortDirection?.ToLower() == "desc"
                    ? query.OrderByDescending(e => e.Email)
                    : query.OrderBy(e => e.Email),
                "department" => sortDirection?.ToLower() == "desc"
                    ? query.OrderByDescending(e => e.Department!.DepartmentName)
                    : query.OrderBy(e => e.Department!.DepartmentName),
                "status" => sortDirection?.ToLower() == "desc"
                    ? query.OrderByDescending(e => e.Status)
                    : query.OrderBy(e => e.Status),
                "dateofjoining" => sortDirection?.ToLower() == "desc"
                    ? query.OrderByDescending(e => e.DateOfJoining)
                    : query.OrderBy(e => e.DateOfJoining),
                _ => query.OrderBy(e => e.FirstName)
            };

            var items = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
            return new PagedResult<Employee>(items, totalCount, page, pageSize);
        }
    }
}
