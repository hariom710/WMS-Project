using WMS.Domain.Models;

namespace WMS.Domain.Interfaces
{
    public interface IAnnouncementRepository
    {
        Task<PagedResult<Announcement>> GetAllAsync(string? search, bool? isActive, string? sortBy, string? sortDirection, int page, int pageSize);
        Task<PagedResult<Announcement>> GetDeletedAsync(string? search, string? sortBy, string? sortDirection, int page, int pageSize);
        Task AddAsync(Announcement announcement);
        Task UpdateAsync(Announcement announcement);
        Task SoftDeleteAsync(Announcement announcement, string? deletedBy);
        Task RestoreAsync(Announcement announcement);
        Task DeleteAsync(Announcement announcement);
    }
}
