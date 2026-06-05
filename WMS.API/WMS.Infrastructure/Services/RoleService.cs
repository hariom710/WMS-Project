using WMS.Domain.Interfaces;
using WMS.Domain.Models;

namespace WMS.Infrastructure.Services
{
    public class RoleService : IRoleService
    {
        private readonly IRoleRepository _repo;

        public RoleService(IRoleRepository repo) => _repo = repo;

        public async Task<IEnumerable<Role>> GetAllAsync() =>
            await _repo.GetAllAsync();

        public async Task<bool> CreateAsync(Role role)
        {
            await _repo.AddAsync(role);
            return true;
        }

        public async Task<bool> UpdateAsync(int id, Role role)
        {
            if (id != role.RoleId) return false;
            await _repo.UpdateAsync(role);
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var role = await _repo.GetByIdAsync(id);
            if (role == null) return false;
            await _repo.DeleteAsync(role);
            return true;
        }
    }
}
