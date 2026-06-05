using WMS.Domain.Models;

namespace WMS.Domain.Interfaces
{
    public interface IDepartmentService
    {
        Task<PagedResult<Department>> GetAllAsync(string? search, string? sortBy, string? sortDirection, int page, int pageSize);
        Task<PagedResult<Department>> GetDeletedAsync(string? search, string? sortBy, string? sortDirection, int page, int pageSize);
        Task<bool> CreateAsync(Department department, string? createdBy);
        Task<bool> UpdateAsync(int id, Department department, string? modifiedBy);
        Task<(bool success, string message)> SoftDeleteAsync(int id, string? deletedBy);
        Task<bool> RestoreAsync(int id);
    }
}
