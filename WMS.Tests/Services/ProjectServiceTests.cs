using Moq;
using WMS.Domain.Interfaces;
using WMS.Domain.Models;
using WMS.Infrastructure.Services;

namespace WMS.Tests.Services;

public class ProjectServiceTests
{
    private readonly Mock<IProjectRepository> _repo = new();
    private readonly Mock<IActivityLogService> _activityLog = new();
    private readonly ProjectService _sut;

    public ProjectServiceTests()
    {
        _sut = new ProjectService(_repo.Object, _activityLog.Object);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsPagedResult()
    {
        var items = new List<Project> { new() { ProjectId = 1, ProjectName = "Test Project" } };
        _repo.Setup(r => r.GetAllAsync(null, null, null, null, null, 1, 10))
            .ReturnsAsync(new PagedResult<Project>(items, 1, 1, 10));

        var result = await _sut.GetAllAsync(null, null, null, null, null, 1, 10);

        Assert.Single(result.Items);
    }

    [Fact]
    public async Task CreateAsync_SetsAuditFields()
    {
        var proj = new Project { ProjectName = "New Project", StartDate = DateTime.UtcNow };
        _repo.Setup(r => r.AddAsync(It.IsAny<Project>())).Returns(Task.CompletedTask);

        var result = await _sut.CreateAsync(proj, "admin");

        Assert.True(result);
        Assert.Equal("admin", proj.CreatedBy);
    }

    [Fact]
    public async Task UpdateAsync_ReturnsTrue_WhenExists()
    {
        _repo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new Project { ProjectId = 1 });
        _repo.Setup(r => r.UpdateAsync(It.IsAny<Project>())).Returns(Task.CompletedTask);

        var proj = new Project { ProjectId = 1, ProjectName = "Updated" };
        var result = await _sut.UpdateAsync(1, proj, "admin");

        Assert.True(result);
        Assert.Equal("admin", proj.ModifiedBy);
    }

    [Fact]
    public async Task UpdateAsync_AlwaysSucceeds()
    {
        _repo.Setup(r => r.UpdateAsync(It.IsAny<Project>())).Returns(Task.CompletedTask);

        var proj = new Project { ProjectId = 1, ProjectName = "Updated" };
        var result = await _sut.UpdateAsync(1, proj, "admin");

        Assert.True(result);  // UpdateAsync doesn't check existence
    }

    [Fact]
    public async Task SoftDeleteAsync_Success_WhenExistsNoAllocations()
    {
        _repo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new Project { ProjectId = 1 });
        _repo.Setup(r => r.HasAllocationsAsync(1)).ReturnsAsync(false);

        var (success, message) = await _sut.SoftDeleteAsync(1, "admin");

        Assert.True(success);
    }

    [Fact]
    public async Task SoftDeleteAsync_Failure_WhenHasAllocations()
    {
        _repo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new Project { ProjectId = 1 });
        _repo.Setup(r => r.HasAllocationsAsync(1)).ReturnsAsync(true);

        var (success, message) = await _sut.SoftDeleteAsync(1, "admin");

        Assert.False(success);
        Assert.Contains("allocation", message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task SoftDeleteAsync_Failure_WhenNotExists()
    {
        _repo.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((Project?)null);

        var (success, _) = await _sut.SoftDeleteAsync(999, "admin");

        Assert.False(success);
    }

    [Fact]
    public async Task RestoreAsync_True_WhenDeletedExists()
    {
        var deleted = new PagedResult<Project>(
            new List<Project> { new() { ProjectId = 1 } }, 1, 1, 1000);
        _repo.Setup(r => r.GetDeletedAsync(null, null, null, 1, int.MaxValue)).ReturnsAsync(deleted);

        var result = await _sut.RestoreAsync(1);

        Assert.True(result);
    }

    [Fact]
    public async Task RestoreAsync_False_WhenNotDeleted()
    {
        var empty = new PagedResult<Project>(new List<Project>(), 0, 1, 1000);
        _repo.Setup(r => r.GetDeletedAsync(null, null, null, 1, int.MaxValue)).ReturnsAsync(empty);

        var result = await _sut.RestoreAsync(999);

        Assert.False(result);
    }
}
