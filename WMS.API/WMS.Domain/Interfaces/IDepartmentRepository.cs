using WMS.Domain.Models;

namespace WMS.Domain.Interfaces
{
    public interface IDepartmentRepository
    {
        Task<PagedResult<Department>> GetAllAsync(string? search, string? sortBy, string? sortDirection, int page, int pageSize);
        Task<PagedResult<Department>> GetDeletedAsync(string? search, string? sortBy, string? sortDirection, int page, int pageSize);
        Task<Department?> GetByIdAsync(int id);
        Task<bool> ExistsByNameAsync(string name, int? excludeId = null);
        Task<bool> HasEmployeesAsync(int id);
        Task AddAsync(Department department);
        Task UpdateAsync(Department department);
        Task SoftDeleteAsync(Department department, string? deletedBy);
        Task RestoreAsync(Department department);
        Task DeleteAsync(Department department);
    }
}
