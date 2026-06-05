using WMS.Domain.Models;

namespace WMS.Domain.Interfaces
{
    public interface IAllocationService
    {
        Task<PagedResult<ProjectAllocation>> GetAllAsync(string? search, string? sortBy, string? sortDirection, int page, int pageSize);
        Task<PagedResult<ProjectAllocation>> GetDeletedAsync(string? search, string? sortBy, string? sortDirection, int page, int pageSize);
        Task<(bool success, string message)> CreateAsync(ProjectAllocation allocation, string userEmail);
        Task<(bool success, string message)> SoftDeleteAsync(int id, string deletedBy);
        Task<bool> RestoreAsync(int id);
    }
}
