using Moq;
using WMS.Domain.Interfaces;
using WMS.Domain.Models;
using WMS.Infrastructure.Services;

namespace WMS.Tests.Services;

public class ClientServiceTests
{
    private readonly Mock<IClientRepository> _repo = new();
    private readonly Mock<IActivityLogService> _activityLog = new();
    private readonly ClientService _sut;

    public ClientServiceTests()
    {
        _sut = new ClientService(_repo.Object, _activityLog.Object);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsPagedResult()
    {
        var items = new List<Client> { new() { ClientId = 1, ClientName = "Test Corp" } };
        _repo.Setup(r => r.GetAllAsync(null, null, null, 1, 10))
            .ReturnsAsync(new PagedResult<Client>(items, 1, 1, 10));

        var result = await _sut.GetAllAsync(null, null, null, 1, 10);

        Assert.Single(result.Items);
        Assert.Equal("Test Corp", result.Items[0].ClientName);
    }

    [Fact]
    public async Task CreateAsync_SetsAuditFields()
    {
        var client = new Client { ClientName = "New Client" };
        _repo.Setup(r => r.AddAsync(It.IsAny<Client>())).Returns(Task.CompletedTask);

        var result = await _sut.CreateAsync(client, "admin");

        Assert.True(result);
        Assert.Equal("admin", client.CreatedBy);
    }

    [Fact]
    public async Task UpdateAsync_ReturnsTrue_WhenExists()
    {
        _repo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new Client { ClientId = 1 });
        _repo.Setup(r => r.UpdateAsync(It.IsAny<Client>())).Returns(Task.CompletedTask);

        var client = new Client { ClientId = 1, ClientName = "Updated" };
        var result = await _sut.UpdateAsync(1, client, "admin");

        Assert.True(result);
        Assert.Equal("admin", client.ModifiedBy);
    }

    [Fact]
    public async Task UpdateAsync_AlwaysSucceeds()
    {
        _repo.Setup(r => r.UpdateAsync(It.IsAny<Client>())).Returns(Task.CompletedTask);

        var client = new Client { ClientId = 1, ClientName = "Updated" };
        var result = await _sut.UpdateAsync(1, client, "admin");

        Assert.True(result);  // UpdateAsync doesn't check existence
    }

    [Fact]
    public async Task SoftDeleteAsync_ReturnsTrue_WhenExists()
    {
        _repo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new Client { ClientId = 1 });

        var result = await _sut.SoftDeleteAsync(1, "admin");

        Assert.True(result);
    }

    [Fact]
    public async Task SoftDeleteAsync_ReturnsFalse_WhenNotExists()
    {
        _repo.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((Client?)null);

        var result = await _sut.SoftDeleteAsync(999, "admin");

        Assert.False(result);
    }

    [Fact]
    public async Task RestoreAsync_ReturnsTrue_WhenDeletedExists()
    {
        var deleted = new PagedResult<Client>(
            new List<Client> { new() { ClientId = 1 } }, 1, 1, 1000);
        _repo.Setup(r => r.GetDeletedAsync(null, null, null, 1, int.MaxValue)).ReturnsAsync(deleted);

        var result = await _sut.RestoreAsync(1);

        Assert.True(result);
    }

    [Fact]
    public async Task RestoreAsync_ReturnsFalse_WhenNotDeleted()
    {
        var empty = new PagedResult<Client>(new List<Client>(), 0, 1, 1000);
        _repo.Setup(r => r.GetDeletedAsync(null, null, null, 1, int.MaxValue)).ReturnsAsync(empty);

        var result = await _sut.RestoreAsync(999);

        Assert.False(result);
    }
}
