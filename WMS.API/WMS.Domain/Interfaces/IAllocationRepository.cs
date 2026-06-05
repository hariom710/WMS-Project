using WMS.Domain.Models;

namespace WMS.Domain.Interfaces
{
    public interface IAllocationRepository
    {
        Task<PagedResult<ProjectAllocation>> GetAllAsync(string? search, string? sortBy, string? sortDirection, int page, int pageSize);
        Task<PagedResult<ProjectAllocation>> GetDeletedAsync(string? search, string? sortBy, string? sortDirection, int page, int pageSize);
        Task<bool> ExistsActiveAsync(int empId, int projectId);
        Task AddAsync(ProjectAllocation allocation);
        Task UpdateAsync(ProjectAllocation allocation);
        Task SoftDeleteAsync(ProjectAllocation allocation, string? deletedBy);
        Task RestoreAsync(ProjectAllocation allocation);
        Task DeleteAsync(ProjectAllocation allocation);
    }
}
