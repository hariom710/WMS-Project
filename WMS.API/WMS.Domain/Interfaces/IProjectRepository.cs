using WMS.Domain.Models;

namespace WMS.Domain.Interfaces
{
    public interface IProjectRepository
    {
        Task<PagedResult<Project>> GetAllAsync(string? search, string? status, int? clientId, string? sortBy, string? sortDirection, int page, int pageSize);
        Task<PagedResult<Project>> GetDeletedAsync(string? search, string? sortBy, string? sortDirection, int page, int pageSize);
        Task<Project?> GetByIdAsync(int id);
        Task<bool> ExistsByNameAsync(string name, int? excludeId = null);
        Task<bool> HasAllocationsAsync(int id);
        Task AddAsync(Project project);
        Task UpdateAsync(Project project);
        Task SoftDeleteAsync(Project project, string? deletedBy);
        Task RestoreAsync(Project project);
        Task DeleteAsync(Project project);
    }
}
