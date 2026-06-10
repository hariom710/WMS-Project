using Moq;
using WMS.Domain.Interfaces;
using WMS.Domain.Models;
using WMS.Infrastructure.Services;

namespace WMS.Tests.Services;

public class LeaveServiceTests
{
    private readonly Mock<ILeaveRepository> _repo = new();
    private readonly Mock<IEmployeeRepository> _empRepo = new();
    private readonly Mock<IActivityLogService> _activityLog = new();
    private readonly LeaveService _sut;

    public LeaveServiceTests()
    {
        _sut = new LeaveService(_repo.Object, _empRepo.Object, _activityLog.Object);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsPagedResult()
    {
        var items = new List<Leave> { new() { LeaveId = 1, LeaveType = "Sick" } };
        _repo.Setup(r => r.GetAllAsync(null, null, null, null, 1, 10))
            .ReturnsAsync(new PagedResult<Leave>(items, 1, 1, 10));

        var result = await _sut.GetAllAsync(null, null, null, null, 1, 10);

        Assert.Single(result.Items);
    }

    [Fact]
    public async Task GetPendingAsync_ReturnsPendingLeaves()
    {
        var pending = new List<Leave> { new() { LeaveId = 1, Status = "Pending" } };
        _repo.Setup(r => r.GetPendingAsync()).ReturnsAsync(pending);

        var result = await _sut.GetPendingAsync();

        Assert.Single(result);
        Assert.Equal("Pending", result.First().Status);
    }

    [Fact]
    public async Task ApproveAsync_Success_WhenLeaveExists()
    {
        var leave = new Leave { LeaveId = 1, Status = "Approved", EmpId = 5 };
        _repo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(leave);
        _empRepo.Setup(r => r.GetByEmailAsync("admin@wms.com"))
            .ReturnsAsync(new Employee { EmployeeId = 1 });

        var (success, message) = await _sut.ApproveAsync(1, "admin@wms.com");

        Assert.True(success);
        Assert.Equal("Approved", leave.Status);
        Assert.Equal(1, leave.ApprovedBy);  // ApprovedBy is EmployeeId (int), not email
    }

    [Fact]
    public async Task ApproveAsync_Failure_WhenNotExists()
    {
        _repo.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((Leave?)null);

        var (success, _) = await _sut.ApproveAsync(999, "admin@wms.com");

        Assert.False(success);
    }

    [Fact]
    public async Task RejectAsync_Success_WhenPending()
    {
        var leave = new Leave { LeaveId = 1, Status = "Pending" };
        _repo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(leave);
        _empRepo.Setup(r => r.GetByEmailAsync("admin@wms.com"))
            .ReturnsAsync(new Employee { EmployeeId = 1 });

        var (success, message) = await _sut.RejectAsync(1, "Not valid reason enough", "admin@wms.com");

        Assert.True(success);
        Assert.Equal("Rejected", leave.Status);
    }

    [Fact]
    public async Task SoftDeleteAsync_Success_WhenExists()
    {
        _repo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new Leave { LeaveId = 1 });

        var (success, _) = await _sut.SoftDeleteAsync(1, "admin@wms.com");

        Assert.True(success);
    }

    [Fact]
    public async Task SoftDeleteAsync_Failure_WhenNotExists()
    {
        _repo.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((Leave?)null);

        var (success, _) = await _sut.SoftDeleteAsync(999, "admin@wms.com");

        Assert.False(success);
    }

    [Fact]
    public async Task RestoreAsync_True_WhenDeletedExists()
    {
        var deleted = new PagedResult<Leave>(
            new List<Leave> { new() { LeaveId = 1 } }, 1, 1, 1000);
        _repo.Setup(r => r.GetDeletedAsync(null, null, null, 1, int.MaxValue)).ReturnsAsync(deleted);

        Assert.True(await _sut.RestoreAsync(1));
    }
}
