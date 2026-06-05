using WMS.Domain.Models;

namespace WMS.Domain.Interfaces
{
    public interface IAttendanceRepository
    {
        Task<PagedResult<Attendance>> GetAllAsync(string? search, int? empId, int? month, int? year, string? sortBy, string? sortDirection, int page, int pageSize);
        Task<Attendance?> GetByIdAsync(int id);
        Task<Attendance?> GetTodayByEmployeeAsync(int empId);
        Task<bool> HasCheckedInTodayAsync(int empId);
        Task AddAsync(Attendance attendance);
        Task UpdateAsync(Attendance attendance);
    }
}
