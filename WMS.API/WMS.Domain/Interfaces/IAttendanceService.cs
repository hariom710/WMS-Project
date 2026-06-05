using WMS.Domain.Models;

namespace WMS.Domain.Interfaces
{
    public interface IAttendanceService
    {
        Task<PagedResult<Attendance>> GetAllAsync(string? search, int? empId, int? month, int? year, string? sortBy, string? sortDirection, int page, int pageSize);
        Task<(bool success, string message)> CreateAsync(Attendance attendance);
        Task<(bool success, string message)> UpdateAsync(int id, Attendance attendance);
        Task<(bool success, string message)> CheckInAsync(string workMode, string userEmail);
        Task<(bool success, string message)> CheckOutAsync(string userEmail);
    }
}
