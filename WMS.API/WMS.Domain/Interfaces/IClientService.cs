using WMS.Domain.Models;

namespace WMS.Domain.Interfaces
{
    public interface IClientService
    {
        Task<PagedResult<Client>> GetAllAsync(string? search, string? sortBy, string? sortDirection, int page, int pageSize);
        Task<PagedResult<Client>> GetDeletedAsync(string? search, string? sortBy, string? sortDirection, int page, int pageSize);
        Task<bool> CreateAsync(Client client, string? createdBy);
        Task<bool> UpdateAsync(int id, Client client, string? modifiedBy);
        Task<bool> SoftDeleteAsync(int id, string? deletedBy);
        Task<bool> RestoreAsync(int id);
    }
}
