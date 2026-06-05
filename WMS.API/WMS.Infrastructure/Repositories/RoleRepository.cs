using Microsoft.EntityFrameworkCore;
using WMS.Domain.Interfaces;
using WMS.Domain.Models;
using WMS.Infrastructure.Data;

namespace WMS.Infrastructure.Repositories
{
    public class RoleRepository : IRoleRepository
    {
        private readonly WMSDbContext _context;

        public RoleRepository(WMSDbContext context) => _context = context;

        public async Task<IEnumerable<Role>> GetAllAsync() =>
            await _context.Roles.AsNoTracking().ToListAsync();

        public async Task<Role?> GetByIdAsync(int id) =>
            await _context.Roles.FindAsync(id);

        public async Task<Role?> GetByNameAsync(string name) =>
            await _context.Roles.FirstOrDefaultAsync(r => r.RoleName == name);

        public async Task<bool> AnyAsync() =>
            await _context.Roles.AnyAsync();

        public async Task AddAsync(Role role)
        {
            _context.Roles.Add(role);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Role role)
        {
            _context.Entry(role).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Role role)
        {
            _context.Roles.Remove(role);
            await _context.SaveChangesAsync();
        }
    }
}
