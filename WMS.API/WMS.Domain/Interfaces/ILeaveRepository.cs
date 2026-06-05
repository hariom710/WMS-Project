using WMS.Domain.Models;

namespace WMS.Domain.Interfaces
{
    public interface ILeaveRepository
    {
        Task<PagedResult<Leave>> GetAllAsync(string? search, string? status, string? sortBy, string? sortDirection, int page, int pageSize);
        Task<PagedResult<Leave>> GetDeletedAsync(string? search, string? sortBy, string? sortDirection, int page, int pageSize);
        Task<IEnumerable<Leave>> GetPendingAsync();
        Task<Leave?> GetByIdAsync(int id);
        Task<bool> HasOverlappingLeaveAsync(int empId, DateTime fromDate, DateTime toDate, int? excludeId = null);
        Task AddAsync(Leave leave);
        Task UpdateAsync(Leave leave);
        Task SoftDeleteAsync(Leave leave, string? deletedBy);
        Task RestoreAsync(Leave leave);
        Task DeleteAsync(Leave leave);
    }
}
