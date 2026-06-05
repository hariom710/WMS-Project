namespace WMS.Domain.Interfaces
{
    public interface ICurrentUserService
    {
        string? Username { get; }
        string? Role { get; }
        int? RoleId { get; }
    }
}
