using Microsoft.EntityFrameworkCore;
using WMS.Domain.Interfaces;
using WMS.Domain.Models;
using WMS.Infrastructure.Data;

namespace WMS.Infrastructure.Repositories
{
    public class LeaveRepository : ILeaveRepository
    {
        private readonly WMSDbContext _context;

        public LeaveRepository(WMSDbContext context) => _context = context;

        public async Task<PagedResult<Leave>> GetAllAsync(
            string? search, string? status,
            string? sortBy, string? sortDirection, int page, int pageSize)
        {
            var query = _context.Leaves.AsNoTracking().Include(l => l.Employee).AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                var term = search.ToLower();
                query = query.Where(l =>
                    (l.Employee != null && (l.Employee.FirstName.ToLower().Contains(term) || l.Employee.LastName.ToLower().Contains(term))) ||
                    l.LeaveType.ToLower().Contains(term));
            }

            if (!string.IsNullOrWhiteSpace(status))
                query = query.Where(l => l.Status.ToLower() == status.ToLower());

            var totalCount = await query.CountAsync();

            query = sortBy?.ToLower() switch
            {
                "employee" or "employeename" => sortDirection?.ToLower() == "desc"
                    ? query.OrderByDescending(l => l.Employee!.FirstName)
                    : query.OrderBy(l => l.Employee!.FirstName),
                "leavetype" or "type" => sortDirection?.ToLower() == "desc"
                    ? query.OrderByDescending(l => l.LeaveType)
                    : query.OrderBy(l => l.LeaveType),
                "fromdate" => sortDirection?.ToLower() == "desc"
                    ? query.OrderByDescending(l => l.FromDate)
                    : query.OrderBy(l => l.FromDate),
                "status" => sortDirection?.ToLower() == "desc"
                    ? query.OrderByDescending(l => l.Status)
                    : query.OrderBy(l => l.Status),
                _ => query.OrderByDescending(l => l.CreatedDate)
            };

            var items = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
            return new PagedResult<Leave>(items, totalCount, page, pageSize);
        }

        public async Task<PagedResult<Leave>> GetDeletedAsync(
            string? search, string? sortBy, string? sortDirection, int page, int pageSize)
        {
            var query = _context.Leaves.AsNoTracking().IgnoreQueryFilters().Include(l => l.Employee).Where(l => l.IsDeleted).AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                var term = search.ToLower();
                query = query.Where(l =>
                    (l.Employee != null && (l.Employee.FirstName.ToLower().Contains(term) || l.Employee.LastName.ToLower().Contains(term))) ||
                    l.LeaveType.ToLower().Contains(term));
            }

            var totalCount = await query.CountAsync();

            query = sortBy?.ToLower() switch
            {
                "employee" or "employeename" => sortDirection?.ToLower() == "desc"
                    ? query.OrderByDescending(l => l.Employee!.FirstName)
                    : query.OrderBy(l => l.Employee!.FirstName),
                "deleteddate" => sortDirection?.ToLower() == "desc"
                    ? query.OrderByDescending(l => l.DeletedDate)
                    : query.OrderBy(l => l.DeletedDate),
                _ => query.OrderByDescending(l => l.DeletedDate)
            };

            var items = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
            return new PagedResult<Leave>(items, totalCount, page, pageSize);
        }

        public async Task<IEnumerable<Leave>> GetPendingAsync() =>
            await _context.Leaves.AsNoTracking().Include(l => l.Employee)
                .Where(l => l.Status == "Pending")
                .OrderBy(l => l.CreatedDate).ToListAsync();

        public async Task<Leave?> GetByIdAsync(int id) =>
            await _context.Leaves.FindAsync(id);

        public async Task<bool> HasOverlappingLeaveAsync(int empId, DateTime fromDate, DateTime toDate, int? excludeId = null) =>
            await _context.Leaves.AnyAsync(l =>
                l.EmpId == empId &&
                l.Status != "Rejected" &&
                l.FromDate <= toDate &&
                l.ToDate >= fromDate &&
                (!excludeId.HasValue || l.LeaveId != excludeId.Value));

        public async Task AddAsync(Leave leave)
        {
            _context.Leaves.Add(leave);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Leave leave)
        {
            _context.Entry(leave).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task SoftDeleteAsync(Leave leave, string? deletedBy)
        {
            leave.IsDeleted = true;
            leave.DeletedBy = deletedBy;
            leave.DeletedDate = DateTime.UtcNow;
            _context.Entry(leave).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task RestoreAsync(Leave leave)
        {
            leave.IsDeleted = false;
            leave.DeletedBy = null;
            leave.DeletedDate = null;
            _context.Entry(leave).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Leave leave)
        {
            _context.Leaves.Remove(leave);
            await _context.SaveChangesAsync();
        }
    }
}
