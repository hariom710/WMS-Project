using Moq;
using WMS.Domain.Interfaces;
using WMS.Domain.Models;
using WMS.Infrastructure.Services;

namespace WMS.Tests.Services;

public class DepartmentServiceTests
{
    private readonly Mock<IDepartmentRepository> _repo = new();
    private readonly Mock<IActivityLogService> _activityLog = new();
    private readonly DepartmentService _sut;

    public DepartmentServiceTests()
    {
        _sut = new DepartmentService(_repo.Object, _activityLog.Object);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsPagedResult()
    {
        var items = new List<Department> { new() { DepartmentId = 1, DepartmentName = "Engineering" } };
        _repo.Setup(r => r.GetAllAsync(null, null, null, 1, 10))
            .ReturnsAsync(new PagedResult<Department>(items, 1, 1, 10));

        var result = await _sut.GetAllAsync(null, null, null, 1, 10);

        Assert.Single(result.Items);
        Assert.Equal("Engineering", result.Items[0].DepartmentName);
    }

    [Fact]
    public async Task CreateAsync_SetsAuditFieldsAndReturns()
    {
        var dept = new Department { DepartmentName = "New Dept" };
        _repo.Setup(r => r.AddAsync(It.IsAny<Department>())).Returns(Task.CompletedTask);

        var result = await _sut.CreateAsync(dept, "admin");

        Assert.True(result);
        Assert.Equal("admin", dept.CreatedBy);
        _repo.Verify(r => r.AddAsync(dept), Times.Once);
        _activityLog.Verify(l => l.LogAsync("Department", 0, "Create", It.IsAny<string>(), "admin", null, null), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_SetsModifiedFields()
    {
        var dept = new Department { DepartmentId = 1, DepartmentName = "Updated" };
        _repo.Setup(r => r.UpdateAsync(It.IsAny<Department>())).Returns(Task.CompletedTask);

        var result = await _sut.UpdateAsync(1, dept, "admin");

        Assert.True(result);
        Assert.Equal("admin", dept.ModifiedBy);
        Assert.NotNull(dept.ModifiedDate);
    }

    [Fact]
    public async Task SoftDeleteAsync_ReturnsSuccess_WhenExistsAndNoEmployees()
    {
        var dept = new Department { DepartmentId = 1, DepartmentName = "Test" };
        _repo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(dept);
        _repo.Setup(r => r.HasEmployeesAsync(1)).ReturnsAsync(false);

        var (success, message) = await _sut.SoftDeleteAsync(1, "admin");

        Assert.True(success);
        _repo.Verify(r => r.SoftDeleteAsync(dept, "admin"), Times.Once);
    }

    [Fact]
    public async Task SoftDeleteAsync_ReturnsFailure_WhenHasEmployees()
    {
        _repo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new Department { DepartmentId = 1 });
        _repo.Setup(r => r.HasEmployeesAsync(1)).ReturnsAsync(true);

        var (success, message) = await _sut.SoftDeleteAsync(1, "admin");

        Assert.False(success);
        Assert.Contains("employees", message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task SoftDeleteAsync_ReturnsFailure_WhenNotExists()
    {
        _repo.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((Department?)null);

        var (success, message) = await _sut.SoftDeleteAsync(999, "admin");

        Assert.False(success);
        Assert.Contains("not found", message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task RestoreAsync_ReturnsTrue_WhenDeletedExists()
    {
        var deleted = new PagedResult<Department>(
            new List<Department> { new() { DepartmentId = 1 } }, 1, 1, 1000);
        _repo.Setup(r => r.GetDeletedAsync(null, null, null, 1, int.MaxValue)).ReturnsAsync(deleted);

        var result = await _sut.RestoreAsync(1);

        Assert.True(result);
    }

    [Fact]
    public async Task RestoreAsync_ReturnsFalse_WhenNotDeleted()
    {
        var empty = new PagedResult<Department>(new List<Department>(), 0, 1, 1000);
        _repo.Setup(r => r.GetDeletedAsync(null, null, null, 1, int.MaxValue)).ReturnsAsync(empty);

        var result = await _sut.RestoreAsync(999);

        Assert.False(result);
    }
}
