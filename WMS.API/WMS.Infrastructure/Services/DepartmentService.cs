using WMS.Domain.Interfaces;
using WMS.Domain.Models;

namespace WMS.Infrastructure.Services
{
    public class DepartmentService : IDepartmentService
    {
        private readonly IDepartmentRepository _repo;
        private readonly IActivityLogService _activityLog;

        public DepartmentService(IDepartmentRepository repo, IActivityLogService activityLog)
        {
            _repo = repo;
            _activityLog = activityLog;
        }

        public async Task<PagedResult<Department>> GetAllAsync(string? search, string? sortBy, string? sortDirection, int page, int pageSize) =>
            await _repo.GetAllAsync(search, sortBy, sortDirection, page, pageSize);

        public async Task<PagedResult<Department>> GetDeletedAsync(string? search, string? sortBy, string? sortDirection, int page, int pageSize) =>
            await _repo.GetDeletedAsync(search, sortBy, sortDirection, page, pageSize);

        public async Task<bool> CreateAsync(Department department, string? createdBy)
        {
            department.DepartmentName = department.DepartmentName?.Trim();
            if (department.Description != null) department.Description = department.Description.Trim();
            department.CreatedBy = createdBy;
            department.CreatedDate = DateTime.UtcNow;
            await _repo.AddAsync(department);

            await _activityLog.LogAsync("Department", department.DepartmentId, "Create",
                $"Created department {department.DepartmentName}", createdBy, null, null);

            return true;
        }

        public async Task<bool> UpdateAsync(int id, Department department, string? modifiedBy)
        {
            department.DepartmentName = department.DepartmentName?.Trim();
            if (department.Description != null) department.Description = department.Description.Trim();
            department.ModifiedBy = modifiedBy;
            department.ModifiedDate = DateTime.UtcNow;
            await _repo.UpdateAsync(department);

            await _activityLog.LogAsync("Department", id, "Update",
                $"Updated department {department.DepartmentName}", modifiedBy, null, null);

            return true;
        }

        public async Task<(bool success, string message)> SoftDeleteAsync(int id, string? deletedBy)
        {
            var dept = await _repo.GetByIdAsync(id);
            if (dept == null) return (false, "Department not found.");
            if (await _repo.HasEmployeesAsync(id)) return (false, "Cannot delete department with assigned employees.");
            await _repo.SoftDeleteAsync(dept, deletedBy);

            await _activityLog.LogAsync("Department", id, "Delete",
                $"Deleted department {dept.DepartmentName}", deletedBy, null, null);

            return (true, "Department deleted successfully!");
        }

        public async Task<bool> RestoreAsync(int id)
        {
            var deleted = await _repo.GetDeletedAsync(null, null, null, 1, int.MaxValue);
            var dept = deleted.Items.FirstOrDefault(d => d.DepartmentId == id);
            if (dept == null) return false;
            await _repo.RestoreAsync(dept);

            await _activityLog.LogAsync("Department", id, "Restore",
                $"Restored department {dept.DepartmentName}", null, null, null);

            return true;
        }
    }
}
