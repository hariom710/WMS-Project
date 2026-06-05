using WMS.Domain.Interfaces;
using WMS.Domain.Models;

namespace WMS.Infrastructure.Services
{
    public class AllocationService : IAllocationService
    {
        private readonly IAllocationRepository _allocationRepo;
        private readonly IEmployeeRepository _employeeRepo;
        private readonly IProjectRepository _projectRepo;
        private readonly IActivityLogService _activityLog;

        public AllocationService(IAllocationRepository allocationRepo, IEmployeeRepository employeeRepo, IProjectRepository projectRepo, IActivityLogService activityLog)
        {
            _allocationRepo = allocationRepo;
            _employeeRepo = employeeRepo;
            _projectRepo = projectRepo;
            _activityLog = activityLog;
        }

        public async Task<PagedResult<ProjectAllocation>> GetAllAsync(string? search, string? sortBy, string? sortDirection, int page, int pageSize) =>
            await _allocationRepo.GetAllAsync(search, sortBy, sortDirection, page, pageSize);

        public async Task<PagedResult<ProjectAllocation>> GetDeletedAsync(string? search, string? sortBy, string? sortDirection, int page, int pageSize) =>
            await _allocationRepo.GetDeletedAsync(search, sortBy, sortDirection, page, pageSize);

        public async Task<(bool success, string message)> CreateAsync(ProjectAllocation allocation, string userEmail)
        {
            if (!await _employeeRepo.ExistsAsync(allocation.EmpId))
                return (false, "Employee does not exist.");

            var project = await _projectRepo.GetByIdAsync(allocation.ProjectId);
            if (project == null) return (false, "Project does not exist.");
            if (project.Status != "Active") return (false, "Cannot allocate to an inactive project.");

            var employee = await _employeeRepo.GetByIdAsync(allocation.EmpId);
            if (employee != null && employee.Status != "Active")
                return (false, "Cannot allocate an inactive employee.");

            if (await _allocationRepo.ExistsActiveAsync(allocation.EmpId, allocation.ProjectId))
                return (false, "Employee already assigned to this project.");

            allocation.CreatedBy = userEmail;
            allocation.CreatedDate = DateTime.UtcNow;
            allocation.Status = true;
            await _allocationRepo.AddAsync(allocation);

            await _activityLog.LogAsync("ProjectAllocation", allocation.AllocationId, "Create",
                $"Allocated employee ID {allocation.EmpId} to project ID {allocation.ProjectId}", userEmail, null, null);

            return (true, "Employee successfully assigned to project!");
        }

        public async Task<(bool success, string message)> SoftDeleteAsync(int id, string deletedBy)
        {
            var allocations = await _allocationRepo.GetAllAsync(null, null, null, 1, int.MaxValue);
            var allocation = allocations.Items.FirstOrDefault(a => a.AllocationId == id);
            if (allocation == null) return (false, "Allocation not found.");
            await _allocationRepo.SoftDeleteAsync(allocation, deletedBy);

            await _activityLog.LogAsync("ProjectAllocation", id, "Delete",
                $"Removed allocation (employee ID {allocation.EmpId} from project ID {allocation.ProjectId})", deletedBy, null, null);

            return (true, "Allocation removed successfully!");
        }

        public async Task<bool> RestoreAsync(int id)
        {
            var deleted = await _allocationRepo.GetDeletedAsync(null, null, null, 1, int.MaxValue);
            var allocation = deleted.Items.FirstOrDefault(a => a.AllocationId == id);
            if (allocation == null) return false;
            await _allocationRepo.RestoreAsync(allocation);

            await _activityLog.LogAsync("ProjectAllocation", id, "Restore",
                $"Restored allocation (employee ID {allocation.EmpId} to project ID {allocation.ProjectId})", null, null, null);

            return true;
        }
    }
}
