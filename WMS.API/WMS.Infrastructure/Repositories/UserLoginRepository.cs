using Microsoft.EntityFrameworkCore;
using WMS.Domain.Interfaces;
using WMS.Domain.Models;
using WMS.Infrastructure.Data;

namespace WMS.Infrastructure.Repositories
{
    public class UserLoginRepository : IUserLoginRepository
    {
        private readonly WMSDbContext _context;

        public UserLoginRepository(WMSDbContext context) => _context = context;

        public async Task<IEnumerable<UserLogin>> GetAllAsync() =>
            await _context.UserLogins.AsNoTracking().Include(u => u.Role).ToListAsync();

        public async Task<UserLogin?> GetByUsernameAsync(string username) =>
            await _context.UserLogins.Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.Username == username);

        public async Task AddAsync(UserLogin userLogin)
        {
            _context.UserLogins.Add(userLogin);
            await _context.SaveChangesAsync();
        }
    }
}
