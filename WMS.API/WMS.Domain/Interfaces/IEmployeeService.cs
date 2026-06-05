using WMS.Domain.Models;

namespace WMS.Domain.Interfaces
{
    public interface IEmployeeService
    {
        Task<PagedResult<Employee>> GetAllAsync(string? search, string? department, string? status, string? sortBy, string? sortDirection, int page, int pageSize);
        Task<PagedResult<Employee>> GetDeletedAsync(string? search, string? sortBy, string? sortDirection, int page, int pageSize);
        Task<Employee?> GetByIdAsync(int id);
        Task<Employee> CreateWithLoginAsync(Employee employee, string? createdBy);
        Task<bool> UpdateAsync(int id, Employee employee, string? modifiedBy);
        Task<bool> SoftDeleteAsync(int id, string? deletedBy);
        Task<bool> RestoreAsync(int id);
        Task<int> GetCountAsync();
    }
}
