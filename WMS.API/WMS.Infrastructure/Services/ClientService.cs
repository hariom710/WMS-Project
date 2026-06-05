using WMS.Domain.Interfaces;
using WMS.Domain.Models;

namespace WMS.Infrastructure.Services
{
    public class ClientService : IClientService
    {
        private readonly IClientRepository _repo;
        private readonly IActivityLogService _activityLog;

        public ClientService(IClientRepository repo, IActivityLogService activityLog)
        {
            _repo = repo;
            _activityLog = activityLog;
        }

        public async Task<PagedResult<Client>> GetAllAsync(string? search, string? sortBy, string? sortDirection, int page, int pageSize) =>
            await _repo.GetAllAsync(search, sortBy, sortDirection, page, pageSize);

        public async Task<PagedResult<Client>> GetDeletedAsync(string? search, string? sortBy, string? sortDirection, int page, int pageSize) =>
            await _repo.GetDeletedAsync(search, sortBy, sortDirection, page, pageSize);

        public async Task<bool> CreateAsync(Client client, string? createdBy)
        {
            client.ClientName = client.ClientName?.Trim();
            client.ClientPhoneNumber = client.ClientPhoneNumber?.Trim();
            client.ClientLocation = client.ClientLocation?.Trim();
            client.ClientAddress = client.ClientAddress?.Trim();
            client.CreatedBy = createdBy;
            client.CreatedDate = DateTime.UtcNow;
            await _repo.AddAsync(client);

            await _activityLog.LogAsync("Client", client.ClientId, "Create",
                $"Created client {client.ClientName}", createdBy, null, null);

            return true;
        }

        public async Task<bool> UpdateAsync(int id, Client client, string? modifiedBy)
        {
            client.ClientName = client.ClientName?.Trim();
            client.ClientPhoneNumber = client.ClientPhoneNumber?.Trim();
            client.ClientLocation = client.ClientLocation?.Trim();
            client.ClientAddress = client.ClientAddress?.Trim();
            client.ModifiedBy = modifiedBy;
            client.ModifiedDate = DateTime.UtcNow;
            await _repo.UpdateAsync(client);

            await _activityLog.LogAsync("Client", id, "Update",
                $"Updated client {client.ClientName}", modifiedBy, null, null);

            return true;
        }

        public async Task<bool> SoftDeleteAsync(int id, string? deletedBy)
        {
            var client = await _repo.GetByIdAsync(id);
            if (client == null) return false;
            await _repo.SoftDeleteAsync(client, deletedBy);

            await _activityLog.LogAsync("Client", id, "Delete",
                $"Deleted client {client.ClientName}", deletedBy, null, null);

            return true;
        }

        public async Task<bool> RestoreAsync(int id)
        {
            var deleted = await _repo.GetDeletedAsync(null, null, null, 1, int.MaxValue);
            var client = deleted.Items.FirstOrDefault(c => c.ClientId == id);
            if (client == null) return false;
            await _repo.RestoreAsync(client);

            await _activityLog.LogAsync("Client", id, "Restore",
                $"Restored client {client.ClientName}", null, null, null);

            return true;
        }
    }
}
