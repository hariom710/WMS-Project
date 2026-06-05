using WMS.Domain.Interfaces;
using WMS.Domain.Models;

namespace WMS.Infrastructure.Services
{
    public class ProjectService : IProjectService
    {
        private readonly IProjectRepository _repo;
        private readonly IActivityLogService _activityLog;

        public ProjectService(IProjectRepository repo, IActivityLogService activityLog)
        {
            _repo = repo;
            _activityLog = activityLog;
        }

        public async Task<PagedResult<Project>> GetAllAsync(string? search, string? status, int? clientId, string? sortBy, string? sortDirection, int page, int pageSize) =>
            await _repo.GetAllAsync(search, status, clientId, sortBy, sortDirection, page, pageSize);

        public async Task<PagedResult<Project>> GetDeletedAsync(string? search, string? sortBy, string? sortDirection, int page, int pageSize) =>
            await _repo.GetDeletedAsync(search, sortBy, sortDirection, page, pageSize);

        public async Task<bool> CreateAsync(Project project, string? createdBy)
        {
            project.ProjectName = project.ProjectName?.Trim();
            if (project.ClientId == 0) project.ClientId = null;
            project.CreatedBy = createdBy;
            project.CreatedDate = DateTime.UtcNow;
            await _repo.AddAsync(project);

            await _activityLog.LogAsync("Project", project.ProjectId, "Create",
                $"Created project {project.ProjectName}", createdBy, null, null);

            return true;
        }

        public async Task<bool> UpdateAsync(int id, Project project, string? modifiedBy)
        {
            project.ProjectName = project.ProjectName?.Trim();
            if (project.ClientId == 0) project.ClientId = null;
            project.ModifiedBy = modifiedBy;
            project.ModifiedDate = DateTime.UtcNow;
            await _repo.UpdateAsync(project);

            await _activityLog.LogAsync("Project", id, "Update",
                $"Updated project {project.ProjectName}", modifiedBy, null, null);

            return true;
        }

        public async Task<(bool success, string message)> SoftDeleteAsync(int id, string? deletedBy)
        {
            var project = await _repo.GetByIdAsync(id);
            if (project == null) return (false, "Project not found.");
            if (await _repo.HasAllocationsAsync(id)) return (false, "Cannot delete project with active allocations.");
            await _repo.SoftDeleteAsync(project, deletedBy);

            await _activityLog.LogAsync("Project", id, "Delete",
                $"Deleted project {project.ProjectName}", deletedBy, null, null);

            return (true, "Project deleted successfully!");
        }

        public async Task<bool> RestoreAsync(int id)
        {
            var deleted = await _repo.GetDeletedAsync(null, null, null, 1, int.MaxValue);
            var project = deleted.Items.FirstOrDefault(p => p.ProjectId == id);
            if (project == null) return false;
            await _repo.RestoreAsync(project);

            await _activityLog.LogAsync("Project", id, "Restore",
                $"Restored project {project.ProjectName}", null, null, null);

            return true;
        }
    }
}
