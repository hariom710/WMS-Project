using Microsoft.EntityFrameworkCore;
using WMS.Domain.Interfaces;
using WMS.Domain.Models;
using WMS.Infrastructure.Data;

namespace WMS.Infrastructure.Repositories
{
    public class AttendanceRepository : IAttendanceRepository
    {
        private readonly WMSDbContext _context;

        public AttendanceRepository(WMSDbContext context) => _context = context;

        public async Task<PagedResult<Attendance>> GetAllAsync(
            string? search, int? empId, int? month, int? year,
            string? sortBy, string? sortDirection, int page, int pageSize)
        {
            var query = _context.Attendances
                .Include(a => a.Employee)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                var term = search.ToLower();
                query = query.Where(a =>
                    a.Employee != null && (
                        a.Employee.FirstName.ToLower().Contains(term) ||
                        a.Employee.LastName.ToLower().Contains(term)));
            }

            if (empId.HasValue)
                query = query.Where(a => a.EmpId == empId.Value);

            if (month.HasValue && year.HasValue)
            {
                var startDate = new DateTime(year.Value, month.Value, 1);
                var endDate = startDate.AddMonths(1).AddDays(-1);
                query = query.Where(a => a.AttendanceDate.Date >= startDate.Date && a.AttendanceDate.Date <= endDate.Date);
            }

            var totalCount = await query.CountAsync();

            query = sortBy?.ToLower() switch
            {
                "employee" or "employeename" => sortDirection?.ToLower() == "desc"
                    ? query.OrderByDescending(a => a.Employee!.FirstName)
                    : query.OrderBy(a => a.Employee!.FirstName),
                "checkin" => sortDirection?.ToLower() == "desc"
                    ? query.OrderByDescending(a => a.CheckIn)
                    : query.OrderBy(a => a.CheckIn),
                "totalhours" => sortDirection?.ToLower() == "desc"
                    ? query.OrderByDescending(a => a.TotalHours)
                    : query.OrderBy(a => a.TotalHours),
                "workmode" => sortDirection?.ToLower() == "desc"
                    ? query.OrderByDescending(a => a.WorkMode)
                    : query.OrderBy(a => a.WorkMode),
                _ => query.OrderByDescending(a => a.AttendanceDate)
            };

            var items = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResult<Attendance>(items, totalCount, page, pageSize);
        }

        public async Task<Attendance?> GetByIdAsync(int id) =>
            await _context.Attendances.FindAsync(id);

        public async Task<Attendance?> GetTodayByEmployeeAsync(int empId)
        {
            var today = DateTime.Today;
            return await _context.Attendances.FirstOrDefaultAsync(
                a => a.EmpId == empId && a.AttendanceDate.Date == today);
        }

        public async Task<bool> HasCheckedInTodayAsync(int empId)
        {
            var today = DateTime.Today;
            return await _context.Attendances.AnyAsync(
                a => a.EmpId == empId && a.AttendanceDate.Date == today);
        }

        public async Task AddAsync(Attendance attendance)
        {
            _context.Attendances.Add(attendance);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Attendance attendance)
        {
            _context.Entry(attendance).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }
    }
}
