using WMS.Domain.Models;

namespace WMS.Domain.Interfaces
{
    public interface IEmployeeRepository
    {
        Task<PagedResult<Employee>> GetAllAsync(string? search, string? department, string? status, string? sortBy, string? sortDirection, int page, int pageSize);
        Task<PagedResult<Employee>> GetDeletedAsync(string? search, string? sortBy, string? sortDirection, int page, int pageSize);
        Task<Employee?> GetByIdAsync(int id);
        Task<Employee?> GetByEmailAsync(string email);
        Task<bool> ExistsByEmailAsync(string email, int? excludeId = null);
        Task<bool> ExistsByPhoneAsync(string phone, int? excludeId = null);
        Task<bool> ExistsAsync(int id);
        Task<bool> HasAttendanceAsync(int id);
        Task<int> GetCountAsync();
        Task AddAsync(Employee employee);
        Task UpdateAsync(Employee employee);
        Task SoftDeleteAsync(Employee employee, string? deletedBy);
        Task RestoreAsync(Employee employee);
        Task DeleteAsync(Employee employee);
    }
}
