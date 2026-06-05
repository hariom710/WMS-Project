using WMS.Domain.Interfaces;
using WMS.Domain.Models;

namespace WMS.Infrastructure.Services
{
    public class AnnouncementService : IAnnouncementService
    {
        private readonly IAnnouncementRepository _announcementRepo;
        private readonly IEmployeeRepository _employeeRepo;
        private readonly IActivityLogService _activityLog;

        public AnnouncementService(IAnnouncementRepository announcementRepo, IEmployeeRepository employeeRepo, IActivityLogService activityLog)
        {
            _announcementRepo = announcementRepo;
            _employeeRepo = employeeRepo;
            _activityLog = activityLog;
        }

        public async Task<PagedResult<Announcement>> GetAllAsync(string? search, bool? isActive, string? sortBy, string? sortDirection, int page, int pageSize) =>
            await _announcementRepo.GetAllAsync(search, isActive, sortBy, sortDirection, page, pageSize);

        public async Task<PagedResult<Announcement>> GetDeletedAsync(string? search, string? sortBy, string? sortDirection, int page, int pageSize) =>
            await _announcementRepo.GetDeletedAsync(search, sortBy, sortDirection, page, pageSize);

        public async Task<(bool success, string message)> CreateAsync(Announcement announcement, string userEmail)
        {
            announcement.Title = announcement.Title?.Trim();
            announcement.Message = announcement.Message?.Trim();

            var employee = await _employeeRepo.GetByEmailAsync(userEmail);
            announcement.CreatedByEmployeeId = employee?.EmployeeId ?? 1;
            announcement.CreatedBy = userEmail;
            announcement.CreatedDate = DateTime.UtcNow;
            await _announcementRepo.AddAsync(announcement);

            await _activityLog.LogAsync("Announcement", announcement.AnnouncementId, "Create",
                $"Posted announcement: {announcement.Title}", userEmail, null, null);

            return (true, "Announcement posted successfully!");
        }

        public async Task<(bool success, string message)> UpdateAsync(int id, Announcement announcement, string modifiedBy)
        {
            announcement.Title = announcement.Title?.Trim();
            announcement.Message = announcement.Message?.Trim();
            announcement.ModifiedBy = modifiedBy;
            announcement.ModifiedDate = DateTime.UtcNow;
            await _announcementRepo.UpdateAsync(announcement);

            await _activityLog.LogAsync("Announcement", id, "Update",
                $"Updated announcement: {announcement.Title}", modifiedBy, null, null);

            return (true, "Announcement updated!");
        }

        public async Task<(bool success, string message)> SoftDeleteAsync(int id, string deletedBy)
        {
            var announcements = await _announcementRepo.GetAllAsync(null, null, null, null, 1, int.MaxValue);
            var announcement = announcements.Items.FirstOrDefault(a => a.AnnouncementId == id);
            if (announcement == null) return (false, "Announcement not found.");
            await _announcementRepo.SoftDeleteAsync(announcement, deletedBy);

            await _activityLog.LogAsync("Announcement", id, "Delete",
                $"Deleted announcement: {announcement.Title}", deletedBy, null, null);

            return (true, "Announcement deleted successfully!");
        }

        public async Task<bool> RestoreAsync(int id)
        {
            var deleted = await _announcementRepo.GetDeletedAsync(null, null, null, 1, int.MaxValue);
            var announcement = deleted.Items.FirstOrDefault(a => a.AnnouncementId == id);
            if (announcement == null) return false;
            await _announcementRepo.RestoreAsync(announcement);

            await _activityLog.LogAsync("Announcement", id, "Restore",
                $"Restored announcement: {announcement.Title}", null, null, null);

            return true;
        }
    }
}
