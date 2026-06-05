using Microsoft.EntityFrameworkCore;
using WMS.Domain.Interfaces;
using WMS.Domain.Models;
using WMS.Infrastructure.Data;

namespace WMS.Infrastructure.Repositories
{
    public class ClientRepository : IClientRepository
    {
        private readonly WMSDbContext _context;

        public ClientRepository(WMSDbContext context) => _context = context;

        public async Task<PagedResult<Client>> GetAllAsync(
            string? search, string? sortBy, string? sortDirection, int page, int pageSize)
        {
            var query = _context.Clients.AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                var term = search.ToLower();
                query = query.Where(c =>
                    c.ClientName.ToLower().Contains(term) ||
                    (c.ClientLocation != null && c.ClientLocation.ToLower().Contains(term)));
            }

            var totalCount = await query.CountAsync();

            query = sortBy?.ToLower() switch
            {
                "name" or "clientname" => sortDirection?.ToLower() == "desc"
                    ? query.OrderByDescending(c => c.ClientName)
                    : query.OrderBy(c => c.ClientName),
                "location" => sortDirection?.ToLower() == "desc"
                    ? query.OrderByDescending(c => c.ClientLocation)
                    : query.OrderBy(c => c.ClientLocation),
                "status" => sortDirection?.ToLower() == "desc"
                    ? query.OrderByDescending(c => c.Status)
                    : query.OrderBy(c => c.Status),
                _ => query.OrderBy(c => c.ClientName)
            };

            var items = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
            return new PagedResult<Client>(items, totalCount, page, pageSize);
        }

        public async Task<PagedResult<Client>> GetDeletedAsync(
            string? search, string? sortBy, string? sortDirection, int page, int pageSize)
        {
            var query = _context.Clients.IgnoreQueryFilters().Where(c => c.IsDeleted).AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                var term = search.ToLower();
                query = query.Where(c =>
                    c.ClientName.ToLower().Contains(term) ||
                    (c.ClientLocation != null && c.ClientLocation.ToLower().Contains(term)));
            }

            var totalCount = await query.CountAsync();

            query = sortBy?.ToLower() switch
            {
                "name" or "clientname" => sortDirection?.ToLower() == "desc"
                    ? query.OrderByDescending(c => c.ClientName)
                    : query.OrderBy(c => c.ClientName),
                "deleteddate" => sortDirection?.ToLower() == "desc"
                    ? query.OrderByDescending(c => c.DeletedDate)
                    : query.OrderBy(c => c.DeletedDate),
                _ => query.OrderByDescending(c => c.DeletedDate)
            };

            var items = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
            return new PagedResult<Client>(items, totalCount, page, pageSize);
        }

        public async Task<Client?> GetByIdAsync(int id) =>
            await _context.Clients.FindAsync(id);

        public async Task<bool> ExistsDuplicateAsync(string name, string phone, int? excludeId = null) =>
            await _context.Clients.AnyAsync(c =>
                c.ClientName.ToLower() == name.ToLower() &&
                c.ClientPhoneNumber == phone &&
                (!excludeId.HasValue || c.ClientId != excludeId.Value));

        public async Task AddAsync(Client client)
        {
            _context.Clients.Add(client);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Client client)
        {
            _context.Entry(client).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task SoftDeleteAsync(Client client, string? deletedBy)
        {
            client.IsDeleted = true;
            client.DeletedBy = deletedBy;
            client.DeletedDate = DateTime.UtcNow;
            _context.Entry(client).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task RestoreAsync(Client client)
        {
            client.IsDeleted = false;
            client.DeletedBy = null;
            client.DeletedDate = null;
            _context.Entry(client).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Client client)
        {
            _context.Clients.Remove(client);
            await _context.SaveChangesAsync();
        }
    }
}
