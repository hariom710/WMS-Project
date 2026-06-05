using Microsoft.EntityFrameworkCore;
using WMS.Domain.Interfaces;
using WMS.Domain.Models;
using WMS.Infrastructure.Data;

namespace WMS.Infrastructure.Repositories
{
    public class ProjectRepository : IProjectRepository
    {
        private readonly WMSDbContext _context;

        public ProjectRepository(WMSDbContext context) => _context = context;

        public async Task<PagedResult<Project>> GetAllAsync(
            string? search, string? status, int? clientId,
            string? sortBy, string? sortDirection, int page, int pageSize)
        {
            var query = _context.Projects.Include(p => p.Client).AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
                query = query.Where(p => p.ProjectName.ToLower().Contains(search.ToLower()));

            if (!string.IsNullOrWhiteSpace(status))
                query = query.Where(p => p.Status.ToLower() == status.ToLower());

            if (clientId.HasValue)
                query = query.Where(p => p.ClientId == clientId.Value);

            var totalCount = await query.CountAsync();

            query = sortBy?.ToLower() switch
            {
                "name" or "projectname" => sortDirection?.ToLower() == "desc"
                    ? query.OrderByDescending(p => p.ProjectName)
                    : query.OrderBy(p => p.ProjectName),
                "status" => sortDirection?.ToLower() == "desc"
                    ? query.OrderByDescending(p => p.Status)
                    : query.OrderBy(p => p.Status),
                "startdate" => sortDirection?.ToLower() == "desc"
                    ? query.OrderByDescending(p => p.StartDate)
                    : query.OrderBy(p => p.StartDate),
                _ => query.OrderByDescending(p => p.ProjectId)
            };

            var items = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
            return new PagedResult<Project>(items, totalCount, page, pageSize);
        }

        public async Task<PagedResult<Project>> GetDeletedAsync(
            string? search, string? sortBy, string? sortDirection, int page, int pageSize)
        {
            var query = _context.Projects.IgnoreQueryFilters().Include(p => p.Client).Where(p => p.IsDeleted).AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
                query = query.Where(p => p.ProjectName.ToLower().Contains(search.ToLower()));

            var totalCount = await query.CountAsync();

            query = sortBy?.ToLower() switch
            {
                "name" or "projectname" => sortDirection?.ToLower() == "desc"
                    ? query.OrderByDescending(p => p.ProjectName)
                    : query.OrderBy(p => p.ProjectName),
                "deleteddate" => sortDirection?.ToLower() == "desc"
                    ? query.OrderByDescending(p => p.DeletedDate)
                    : query.OrderBy(p => p.DeletedDate),
                _ => query.OrderByDescending(p => p.DeletedDate)
            };

            var items = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
            return new PagedResult<Project>(items, totalCount, page, pageSize);
        }

        public async Task<Project?> GetByIdAsync(int id) =>
            await _context.Projects.Include(p => p.Client).FirstOrDefaultAsync(p => p.ProjectId == id);

        public async Task<bool> ExistsByNameAsync(string name, int? excludeId = null) =>
            await _context.Projects.AnyAsync(p =>
                p.ProjectName.ToLower() == name.ToLower() &&
                (!excludeId.HasValue || p.ProjectId != excludeId.Value));

        public async Task<bool> HasAllocationsAsync(int id) =>
            await _context.ProjectAllocations.AnyAsync(a => a.ProjectId == id);

        public async Task AddAsync(Project project)
        {
            _context.Projects.Add(project);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Project project)
        {
            _context.Entry(project).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task SoftDeleteAsync(Project project, string? deletedBy)
        {
            project.IsDeleted = true;
            project.DeletedBy = deletedBy;
            project.DeletedDate = DateTime.UtcNow;
            _context.Entry(project).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task RestoreAsync(Project project)
        {
            project.IsDeleted = false;
            project.DeletedBy = null;
            project.DeletedDate = null;
            _context.Entry(project).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Project project)
        {
            _context.Projects.Remove(project);
            await _context.SaveChangesAsync();
        }
    }
}
