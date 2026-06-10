using Moq;
using WMS.Domain.Interfaces;
using WMS.Domain.Models;
using WMS.Infrastructure.Services;

namespace WMS.Tests.Services;

public class EmployeeServiceTests
{
    private readonly Mock<IEmployeeRepository> _repo = new();
    private readonly Mock<IRoleRepository> _roleRepo = new();
    private readonly Mock<IUserLoginRepository> _userLoginRepo = new();
    private readonly Mock<IActivityLogService> _activityLog = new();
    private readonly EmployeeService _sut;

    public EmployeeServiceTests()
    {
        _sut = new EmployeeService(_repo.Object, _roleRepo.Object, _userLoginRepo.Object, _activityLog.Object);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsPagedResult()
    {
        var items = new List<Employee> { new() { EmployeeId = 1, FirstName = "Test" } };
        _repo.Setup(r => r.GetAllAsync(null, null, null, null, null, 1, 10))
            .ReturnsAsync(new PagedResult<Employee>(items, 1, 1, 10));

        var result = await _sut.GetAllAsync(null, null, null, null, null, 1, 10);

        Assert.Single(result.Items);
        Assert.Equal(1, result.TotalCount);
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsEmployee_WhenExists()
    {
        var emp = new Employee { EmployeeId = 1, FirstName = "Rahul" };
        _repo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(emp);

        var result = await _sut.GetByIdAsync(1);

        Assert.NotNull(result);
        Assert.Equal("Rahul", result!.FirstName);
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsNull_WhenNotExists()
    {
        _repo.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((Employee?)null);

        var result = await _sut.GetByIdAsync(999);

        Assert.Null(result);
    }

    [Fact]
    public async Task CreateWithLoginAsync_SetsAuditFieldsAndReturnsEmployee()
    {
        var emp = new Employee { FirstName = "New", LastName = "User", Email = "new@wms.com" };
        _repo.Setup(r => r.AddAsync(It.IsAny<Employee>())).Callback<Employee>(e => e.EmployeeId = 10)
            .Returns(Task.CompletedTask);

        var result = await _sut.CreateWithLoginAsync(emp, "admin");

        Assert.Equal(10, result.EmployeeId);
        Assert.Equal("admin", result.CreatedBy);
        _repo.Verify(r => r.AddAsync(It.IsAny<Employee>()), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_ReturnsTrue_WhenExists()
    {
        var emp = new Employee { EmployeeId = 1, FirstName = "Updated" };
        _repo.Setup(r => r.ExistsAsync(1)).ReturnsAsync(true);
        _repo.Setup(r => r.UpdateAsync(It.IsAny<Employee>())).Returns(Task.CompletedTask);

        var result = await _sut.UpdateAsync(1, emp, "admin");

        Assert.True(result);
        Assert.Equal("admin", emp.ModifiedBy);
        Assert.NotNull(emp.ModifiedDate);
    }

    [Fact]
    public async Task UpdateAsync_ReturnsFalse_WhenNotExists()
    {
        _repo.Setup(r => r.ExistsAsync(999)).ReturnsAsync(false);

        var result = await _sut.UpdateAsync(999, new Employee(), "admin");

        Assert.False(result);
    }

    [Fact]
    public async Task SoftDeleteAsync_ReturnsTrue_WhenExists()
    {
        var emp = new Employee { EmployeeId = 1 };
        _repo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(emp);

        var result = await _sut.SoftDeleteAsync(1, "admin");

        Assert.True(result);
        _repo.Verify(r => r.SoftDeleteAsync(emp, "admin"), Times.Once);
    }

    [Fact]
    public async Task SoftDeleteAsync_ReturnsFalse_WhenNotExists()
    {
        _repo.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((Employee?)null);

        var result = await _sut.SoftDeleteAsync(999, "admin");

        Assert.False(result);
    }

    [Fact]
    public async Task RestoreAsync_ReturnsTrue_WhenDeletedExists()
    {
        var deleted = new PagedResult<Employee>(
            new List<Employee> { new() { EmployeeId = 1 } }, 1, 1, 1000);
        _repo.Setup(r => r.GetDeletedAsync(null, null, null, 1, int.MaxValue)).ReturnsAsync(deleted);
        _repo.Setup(r => r.RestoreAsync(It.IsAny<Employee>())).Returns(Task.CompletedTask);

        var result = await _sut.RestoreAsync(1);

        Assert.True(result);
    }

    [Fact]
    public async Task RestoreAsync_ReturnsFalse_WhenNotDeleted()
    {
        var empty = new PagedResult<Employee>(new List<Employee>(), 0, 1, 1000);
        _repo.Setup(r => r.GetDeletedAsync(null, null, null, 1, int.MaxValue)).ReturnsAsync(empty);

        var result = await _sut.RestoreAsync(999);

        Assert.False(result);
    }

    [Fact]
    public async Task GetCountAsync_ReturnsCount()
    {
        _repo.Setup(r => r.GetCountAsync()).ReturnsAsync(42);

        var result = await _sut.GetCountAsync();

        Assert.Equal(42, result);
    }
}
