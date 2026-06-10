using Moq;
using WMS.Domain.Interfaces;
using WMS.Domain.Models;
using WMS.Infrastructure.Services;

namespace WMS.Tests.Services;

public class AnnouncementServiceTests
{
    private readonly Mock<IAnnouncementRepository> _repo = new();
    private readonly Mock<IEmployeeRepository> _empRepo = new();
    private readonly Mock<IActivityLogService> _activityLog = new();
    private readonly AnnouncementService _sut;

    public AnnouncementServiceTests()
    {
        _sut = new AnnouncementService(_repo.Object, _empRepo.Object, _activityLog.Object);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsPagedResult()
    {
        var items = new List<Announcement> { new() { AnnouncementId = 1, Title = "Test" } };
        _repo.Setup(r => r.GetAllAsync(null, null, null, null, 1, 10))
            .ReturnsAsync(new PagedResult<Announcement>(items, 1, 1, 10));

        var result = await _sut.GetAllAsync(null, null, null, null, 1, 10);

        Assert.Single(result.Items);
    }

    [Fact]
    public async Task CreateAsync_SetsAuditFields()
    {
        _empRepo.Setup(r => r.GetByEmailAsync("admin@wms.com"))
            .ReturnsAsync(new Employee { EmployeeId = 1 });
        _repo.Setup(r => r.AddAsync(It.IsAny<Announcement>())).Returns(Task.CompletedTask);

        var ann = new Announcement { Title = "New Notice Title", Message = "A new notice with more than ten characters" };
        var (success, _) = await _sut.CreateAsync(ann, "admin@wms.com");

        Assert.True(success);
        Assert.Equal("admin@wms.com", ann.CreatedBy);
        Assert.Equal(1, ann.CreatedByEmployeeId);
    }

    [Fact]
    public async Task UpdateAsync_AlwaysSucceeds()
    {
        _repo.Setup(r => r.UpdateAsync(It.IsAny<Announcement>())).Returns(Task.CompletedTask);

        var ann = new Announcement { AnnouncementId = 1, Title = "Updated Title", Message = "Updated message" };
        var (success, _) = await _sut.UpdateAsync(1, ann, "admin");

        Assert.True(success);
        Assert.Equal("admin", ann.ModifiedBy);
        Assert.NotNull(ann.ModifiedDate);
    }

    [Fact]
    public async Task SoftDeleteAsync_Success_WhenExists()
    {
        var all = new PagedResult<Announcement>(
            new List<Announcement> { new() { AnnouncementId = 1, Title = "Test" } }, 1, 1, int.MaxValue);
        _repo.Setup(r => r.GetAllAsync(null, null, null, null, 1, int.MaxValue)).ReturnsAsync(all);
        _repo.Setup(r => r.SoftDeleteAsync(It.IsAny<Announcement>(), It.IsAny<string?>())).Returns(Task.CompletedTask);

        var (success, _) = await _sut.SoftDeleteAsync(1, "admin");

        Assert.True(success);
    }

    [Fact]
    public async Task SoftDeleteAsync_Failure_WhenNotExists()
    {
        var empty = new PagedResult<Announcement>(new List<Announcement>(), 0, 1, int.MaxValue);
        _repo.Setup(r => r.GetAllAsync(null, null, null, null, 1, int.MaxValue)).ReturnsAsync(empty);

        var (success, _) = await _sut.SoftDeleteAsync(999, "admin");

        Assert.False(success);
    }

    [Fact]
    public async Task RestoreAsync_True_WhenDeletedExists()
    {
        var deleted = new PagedResult<Announcement>(
            new List<Announcement> { new() { AnnouncementId = 1, Title = "Test" } }, 1, 1, 1000);
        _repo.Setup(r => r.GetDeletedAsync(null, null, null, 1, int.MaxValue)).ReturnsAsync(deleted);
        _repo.Setup(r => r.RestoreAsync(It.IsAny<Announcement>())).Returns(Task.CompletedTask);

        var result = await _sut.RestoreAsync(1);

        Assert.True(result);
    }

    [Fact]
    public async Task RestoreAsync_False_WhenNotDeleted()
    {
        var empty = new PagedResult<Announcement>(new List<Announcement>(), 0, 1, 1000);
        _repo.Setup(r => r.GetDeletedAsync(null, null, null, 1, int.MaxValue)).ReturnsAsync(empty);

        var result = await _sut.RestoreAsync(999);

        Assert.False(result);
    }
}
