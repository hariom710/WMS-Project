using WMS.Domain.Models;

namespace WMS.Domain.Interfaces
{
    public interface IClientRepository
    {
        Task<PagedResult<Client>> GetAllAsync(string? search, string? sortBy, string? sortDirection, int page, int pageSize);
        Task<PagedResult<Client>> GetDeletedAsync(string? search, string? sortBy, string? sortDirection, int page, int pageSize);
        Task<Client?> GetByIdAsync(int id);
        Task<bool> ExistsDuplicateAsync(string name, string phone, int? excludeId = null);
        Task AddAsync(Client client);
        Task UpdateAsync(Client client);
        Task SoftDeleteAsync(Client client, string? deletedBy);
        Task RestoreAsync(Client client);
        Task DeleteAsync(Client client);
    }
}
