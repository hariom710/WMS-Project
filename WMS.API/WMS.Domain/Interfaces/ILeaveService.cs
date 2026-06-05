using WMS.Domain.Models;

namespace WMS.Domain.Interfaces
{
    public interface ILeaveService
    {
        Task<PagedResult<Leave>> GetAllAsync(string? search, string? status, string? sortBy, string? sortDirection, int page, int pageSize);
        Task<PagedResult<Leave>> GetDeletedAsync(string? search, string? sortBy, string? sortDirection, int page, int pageSize);
        Task<IEnumerable<Leave>> GetPendingAsync();
        Task<(bool success, string message)> CreateAsync(Leave leave, string userEmail);
        Task<(bool success, string message)> ApproveAsync(int id, string userEmail);
        Task<(bool success, string message)> RejectAsync(int id, string? reason, string userEmail);
        Task<(bool success, string message)> SoftDeleteAsync(int id, string userEmail);
        Task<bool> RestoreAsync(int id);
    }
}
