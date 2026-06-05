using WMS.Domain.Models;

namespace WMS.Domain.Interfaces
{
    public interface IAnnouncementService
    {
        Task<PagedResult<Announcement>> GetAllAsync(string? search, bool? isActive, string? sortBy, string? sortDirection, int page, int pageSize);
        Task<PagedResult<Announcement>> GetDeletedAsync(string? search, string? sortBy, string? sortDirection, int page, int pageSize);
        Task<(bool success, string message)> CreateAsync(Announcement announcement, string userEmail);
        Task<(bool success, string message)> UpdateAsync(int id, Announcement announcement, string modifiedBy);
        Task<(bool success, string message)> SoftDeleteAsync(int id, string deletedBy);
        Task<bool> RestoreAsync(int id);
    }
}
