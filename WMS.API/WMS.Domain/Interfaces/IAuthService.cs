namespace WMS.Domain.Interfaces
{
    public interface IAuthService
    {
        Task<(bool success, string? token, string? username, string? role, int? roleId)> LoginAsync(string username, string password);
        Task<bool> ChangePasswordAsync(string username, string oldPassword, string newPassword);
        Task<(bool success, string message)> SetupDefaultAdminAsync();
    }
}
