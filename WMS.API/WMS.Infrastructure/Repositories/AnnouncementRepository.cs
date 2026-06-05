using Microsoft.EntityFrameworkCore;
using WMS.Domain.Interfaces;
using WMS.Domain.Models;
using WMS.Infrastructure.Data;

namespace WMS.Infrastructure.Repositories
{
    public class AnnouncementRepository : IAnnouncementRepository
    {
        private readonly WMSDbContext _context;

        public AnnouncementRepository(WMSDbContext context) => _context = context;

        public async Task<PagedResult<Announcement>> GetAllAsync(
            string? search, bool? isActive,
            string? sortBy, string? sortDirection, int page, int pageSize)
        {
            var query = _context.Announcements
                .Include(a => a.CreatedByEmployee)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                var term = search.ToLower();
                query = query.Where(a => a.Title.ToLower().Contains(term) || a.Message.ToLower().Contains(term));
            }

            if (isActive.HasValue)
                query = query.Where(a => a.IsActive == isActive.Value);

            var totalCount = await query.CountAsync();

            query = sortBy?.ToLower() switch
            {
                "title" => sortDirection?.ToLower() == "desc"
                    ? query.OrderByDescending(a => a.Title)
                    : query.OrderBy(a => a.Title),
                "isactive" or "active" => sortDirection?.ToLower() == "desc"
                    ? query.OrderByDescending(a => a.IsActive)
                    : query.OrderBy(a => a.IsActive),
                _ => query.OrderByDescending(a => a.CreatedDate)
            };

            var items = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
            return new PagedResult<Announcement>(items, totalCount, page, pageSize);
        }

        public async Task<PagedResult<Announcement>> GetDeletedAsync(
            string? search, string? sortBy, string? sortDirection, int page, int pageSize)
        {
            var query = _context.Announcements.IgnoreQueryFilters()
                .Include(a => a.CreatedByEmployee)
                .Where(a => a.IsDeleted)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                var term = search.ToLower();
                query = query.Where(a => a.Title.ToLower().Contains(term) || a.Message.ToLower().Contains(term));
            }

            var totalCount = await query.CountAsync();

            query = sortBy?.ToLower() switch
            {
                "title" => sortDirection?.ToLower() == "desc"
                    ? query.OrderByDescending(a => a.Title)
                    : query.OrderBy(a => a.Title),
                "deleteddate" => sortDirection?.ToLower() == "desc"
                    ? query.OrderByDescending(a => a.DeletedDate)
                    : query.OrderBy(a => a.DeletedDate),
                _ => query.OrderByDescending(a => a.DeletedDate)
            };

            var items = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
            return new PagedResult<Announcement>(items, totalCount, page, pageSize);
        }

        public async Task AddAsync(Announcement announcement)
        {
            _context.Announcements.Add(announcement);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Announcement announcement)
        {
            _context.Entry(announcement).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task SoftDeleteAsync(Announcement announcement, string? deletedBy)
        {
            announcement.IsDeleted = true;
            announcement.DeletedBy = deletedBy;
            announcement.DeletedDate = DateTime.UtcNow;
            _context.Entry(announcement).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task RestoreAsync(Announcement announcement)
        {
            announcement.IsDeleted = false;
            announcement.DeletedBy = null;
            announcement.DeletedDate = null;
            _context.Entry(announcement).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Announcement announcement)
        {
            _context.Announcements.Remove(announcement);
            await _context.SaveChangesAsync();
        }
    }
}
