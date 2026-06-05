using WMS.Domain.Models;

namespace WMS.Domain.Interfaces
{
    public interface IRoleService
    {
        Task<IEnumerable<Role>> GetAllAsync();
        Task<bool> CreateAsync(Role role);
        Task<bool> UpdateAsync(int id, Role role);
        Task<bool> DeleteAsync(int id);
    }
}
