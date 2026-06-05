using WMS.Domain.Interfaces;
using WMS.Domain.Models;

namespace WMS.Infrastructure.Services
{
    public class UserLoginService : IUserLoginService
    {
        private readonly IUserLoginRepository _repo;

        public UserLoginService(IUserLoginRepository repo) => _repo = repo;

        public async Task<IEnumerable<UserLogin>> GetAllAsync() =>
            await _repo.GetAllAsync();

        public async Task<bool> CreateAsync(UserLogin userLogin)
        {
            await _repo.AddAsync(userLogin);
            return true;
        }
    }
}
