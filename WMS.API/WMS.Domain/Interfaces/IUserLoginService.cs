using WMS.Domain.Models;

namespace WMS.Domain.Interfaces
{
    public interface IUserLoginService
    {
        Task<IEnumerable<UserLogin>> GetAllAsync();
        Task<bool> CreateAsync(UserLogin userLogin);
    }
}
