using WMS.Domain.Models;

namespace WMS.Domain.Interfaces
{
    public interface IProjectService
    {
        Task<PagedResult<Project>> GetAllAsync(string? search, string? status, int? clientId, string? sortBy, string? sortDirection, int page, int pageSize);
        Task<PagedResult<Project>> GetDeletedAsync(string? search, string? sortBy, string? sortDirection, int page, int pageSize);
        Task<bool> CreateAsync(Project project, string? createdBy);
        Task<bool> UpdateAsync(int id, Project project, string? modifiedBy);
        Task<(bool success, string message)> SoftDeleteAsync(int id, string? deletedBy);
        Task<bool> RestoreAsync(int id);
    }
}
