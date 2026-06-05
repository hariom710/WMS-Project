using WMS.Domain.Models;

namespace WMS.Domain.Interfaces
{
    public interface IUserLoginRepository
    {
        Task<IEnumerable<UserLogin>> GetAllAsync();
        Task<UserLogin?> GetByUsernameAsync(string username);
        Task AddAsync(UserLogin userLogin);
    }
}
