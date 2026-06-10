using Microsoft.EntityFrameworkCore;
using WMS.Domain.Interfaces;
using WMS.Domain.Models;
using WMS.Infrastructure.Data;

namespace WMS.Infrastructure.Repositories
{
    public class DepartmentRepository : IDepartmentRepository
    {
        private readonly WMSDbContext _context;

        public DepartmentRepository(WMSDbContext context) => _context = context;

        public async Task<PagedResult<Department>> GetAllAsync(
            string? search, string? sortBy, string? sortDirection, int page, int pageSize)
        {
            var query = _context.Departments.AsNoTracking().AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
                query = query.Where(d => d.DepartmentName.ToLower().Contains(search.ToLower()));

            var totalCount = await query.CountAsync();

            query = sortBy?.ToLower() switch
            {
                "name" or "departmentname" => sortDirection?.ToLower() == "desc"
                    ? query.OrderByDescending(d => d.DepartmentName)
                    : query.OrderBy(d => d.DepartmentName),
                "createddate" => sortDirection?.ToLower() == "desc"
                    ? query.OrderByDescending(d => d.CreatedDate)
                    : query.OrderBy(d => d.CreatedDate),
                _ => query.OrderBy(d => d.DepartmentName)
            };

            var items = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
            return new PagedResult<Department>(items, totalCount, page, pageSize);
        }

        public async Task<PagedResult<Department>> GetDeletedAsync(
            string? search, string? sortBy, string? sortDirection, int page, int pageSize)
        {
            var query = _context.Departments.AsNoTracking().IgnoreQueryFilters().Where(d => d.IsDeleted).AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
                query = query.Where(d => d.DepartmentName.ToLower().Contains(search.ToLower()));

            var totalCount = await query.CountAsync();

            query = sortBy?.ToLower() switch
            {
                "name" or "departmentname" => sortDirection?.ToLower() == "desc"
                    ? query.OrderByDescending(d => d.DepartmentName)
                    : query.OrderBy(d => d.DepartmentName),
                "deleteddate" => sortDirection?.ToLower() == "desc"
                    ? query.OrderByDescending(d => d.DeletedDate)
                    : query.OrderBy(d => d.DeletedDate),
                _ => query.OrderByDescending(d => d.DeletedDate)
            };

            var items = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
            return new PagedResult<Department>(items, totalCount, page, pageSize);
        }

        public async Task<Department?> GetByIdAsync(int id) =>
            await _context.Departments.FindAsync(id);

        public async Task<bool> ExistsByNameAsync(string name, int? excludeId = null) =>
            await _context.Departments.AnyAsync(d =>
                d.DepartmentName.ToLower() == name.ToLower() &&
                (!excludeId.HasValue || d.DepartmentId != excludeId.Value));

        public async Task<bool> HasEmployeesAsync(int id) =>
            await _context.Employees.AnyAsync(e => e.DepartmentId == id);

        public async Task AddAsync(Department department)
        {
            _context.Departments.Add(department);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Department department)
        {
            _context.Entry(department).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task SoftDeleteAsync(Department department, string? deletedBy)
        {
            department.IsDeleted = true;
            department.DeletedBy = deletedBy;
            department.DeletedDate = DateTime.UtcNow;
            _context.Entry(department).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task RestoreAsync(Department department)
        {
            department.IsDeleted = false;
            department.DeletedBy = null;
            department.DeletedDate = null;
            _context.Entry(department).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Department department)
        {
            _context.Departments.Remove(department);
            await _context.SaveChangesAsync();
        }
    }
}
