using Microsoft.EntityFrameworkCore;
using WMS.Domain.Interfaces;
using WMS.Domain.Models;
using WMS.Infrastructure.Data;

namespace WMS.Infrastructure.Repositories
{
    public class AllocationRepository : IAllocationRepository
    {
        private readonly WMSDbContext _context;

        public AllocationRepository(WMSDbContext context) => _context = context;

        public async Task<PagedResult<ProjectAllocation>> GetAllAsync(
            string? search, string? sortBy, string? sortDirection, int page, int pageSize)
        {
            var query = _context.ProjectAllocations
                .AsNoTracking()
                .Include(a => a.Employee)
                .Include(a => a.Project)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                var term = search.ToLower();
                query = query.Where(a =>
                    (a.Employee != null && (a.Employee.FirstName.ToLower().Contains(term) || a.Employee.LastName.ToLower().Contains(term))) ||
                    (a.Project != null && a.Project.ProjectName.ToLower().Contains(term)));
            }

            var totalCount = await query.CountAsync();

            query = sortBy?.ToLower() switch
            {
                "employee" or "employeename" => sortDirection?.ToLower() == "desc"
                    ? query.OrderByDescending(a => a.Employee!.FirstName)
                    : query.OrderBy(a => a.Employee!.FirstName),
                "project" or "projectname" => sortDirection?.ToLower() == "desc"
                    ? query.OrderByDescending(a => a.Project!.ProjectName)
                    : query.OrderBy(a => a.Project!.ProjectName),
                "assignedon" => sortDirection?.ToLower() == "desc"
                    ? query.OrderByDescending(a => a.AssignedOn)
                    : query.OrderBy(a => a.AssignedOn),
                _ => query.OrderByDescending(a => a.CreatedDate)
            };

            var items = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
            return new PagedResult<ProjectAllocation>(items, totalCount, page, pageSize);
        }

        public async Task<PagedResult<ProjectAllocation>> GetDeletedAsync(
            string? search, string? sortBy, string? sortDirection, int page, int pageSize)
        {
            var query = _context.ProjectAllocations.AsNoTracking().IgnoreQueryFilters()
                .Include(a => a.Employee)
                .Include(a => a.Project)
                .Where(a => a.IsDeleted)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                var term = search.ToLower();
                query = query.Where(a =>
                    (a.Employee != null && (a.Employee.FirstName.ToLower().Contains(term) || a.Employee.LastName.ToLower().Contains(term))) ||
                    (a.Project != null && a.Project.ProjectName.ToLower().Contains(term)));
            }

            var totalCount = await query.CountAsync();

            query = sortBy?.ToLower() switch
            {
                "employee" or "employeename" => sortDirection?.ToLower() == "desc"
                    ? query.OrderByDescending(a => a.Employee!.FirstName)
                    : query.OrderBy(a => a.Employee!.FirstName),
                "project" or "projectname" => sortDirection?.ToLower() == "desc"
                    ? query.OrderByDescending(a => a.Project!.ProjectName)
                    : query.OrderBy(a => a.Project!.ProjectName),
                "deleteddate" => sortDirection?.ToLower() == "desc"
                    ? query.OrderByDescending(a => a.DeletedDate)
                    : query.OrderBy(a => a.DeletedDate),
                _ => query.OrderByDescending(a => a.DeletedDate)
            };

            var items = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
            return new PagedResult<ProjectAllocation>(items, totalCount, page, pageSize);
        }

        public async Task<bool> ExistsActiveAsync(int empId, int projectId) =>
            await _context.ProjectAllocations.AnyAsync(
                a => a.EmpId == empId && a.ProjectId == projectId && a.Status);

        public async Task AddAsync(ProjectAllocation allocation)
        {
            _context.ProjectAllocations.Add(allocation);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(ProjectAllocation allocation)
        {
            _context.Entry(allocation).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task SoftDeleteAsync(ProjectAllocation allocation, string? deletedBy)
        {
            allocation.IsDeleted = true;
            allocation.DeletedBy = deletedBy;
            allocation.DeletedDate = DateTime.UtcNow;
            _context.Entry(allocation).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task RestoreAsync(ProjectAllocation allocation)
        {
            allocation.IsDeleted = false;
            allocation.DeletedBy = null;
            allocation.DeletedDate = null;
            _context.Entry(allocation).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(ProjectAllocation allocation)
        {
            _context.ProjectAllocations.Remove(allocation);
            await _context.SaveChangesAsync();
        }
    }
}
