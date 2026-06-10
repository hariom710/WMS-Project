using Moq;
using WMS.Domain.Interfaces;
using WMS.Domain.Models;
using WMS.Infrastructure.Services;

namespace WMS.Tests.Services;

public class AttendanceServiceTests
{
    private readonly Mock<IAttendanceRepository> _repo = new();
    private readonly Mock<IEmployeeRepository> _empRepo = new();
    private readonly Mock<IActivityLogService> _activityLog = new();
    private readonly AttendanceService _sut;

    public AttendanceServiceTests()
    {
        _sut = new AttendanceService(_repo.Object, _empRepo.Object, _activityLog.Object);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsPagedResult()
    {
        var items = new List<Attendance> { new() { AttendanceId = 1, EmpId = 1 } };
        _repo.Setup(r => r.GetAllAsync(null, null, null, null, null, null, 1, 10))
            .ReturnsAsync(new PagedResult<Attendance>(items, 1, 1, 10));

        var result = await _sut.GetAllAsync(null, null, null, null, null, null, 1, 10);

        Assert.Single(result.Items);
    }

    [Fact]
    public async Task CreateAsync_ReturnsSuccess_WhenValid()
    {
        _empRepo.Setup(r => r.ExistsAsync(1)).ReturnsAsync(true);
        _repo.Setup(r => r.HasCheckedInTodayAsync(1)).ReturnsAsync(false);
        _repo.Setup(r => r.AddAsync(It.IsAny<Attendance>())).Returns(Task.CompletedTask);

        var att = new Attendance { EmpId = 1 };
        var (success, message) = await _sut.CreateAsync(att);

        Assert.True(success);
    }

    [Fact]
    public async Task CreateAsync_Failure_WhenEmployeeNotExists()
    {
        _empRepo.Setup(r => r.ExistsAsync(999)).ReturnsAsync(false);

        var (success, _) = await _sut.CreateAsync(new Attendance { EmpId = 999 });

        Assert.False(success);
    }

    [Fact]
    public async Task CreateAsync_Failure_AlreadyCheckedIn()
    {
        _empRepo.Setup(r => r.ExistsAsync(1)).ReturnsAsync(true);
        _repo.Setup(r => r.HasCheckedInTodayAsync(1)).ReturnsAsync(true);

        var (success, _) = await _sut.CreateAsync(new Attendance { EmpId = 1 });

        Assert.False(success);
    }

    [Fact]
    public async Task UpdateAsync_AlwaysSucceeds()
    {
        _repo.Setup(r => r.UpdateAsync(It.IsAny<Attendance>())).Returns(Task.CompletedTask);

        var (success, message) = await _sut.UpdateAsync(1, new Attendance { AttendanceId = 1 });

        Assert.True(success);  // UpdateAsync always returns true (no existence check)
    }

    [Fact]
    public async Task CheckInAsync_Success()
    {
        _empRepo.Setup(r => r.GetByEmailAsync("test@wms.com"))
            .ReturnsAsync(new Employee { EmployeeId = 1 });
        _repo.Setup(r => r.HasCheckedInTodayAsync(1)).ReturnsAsync(false);
        _repo.Setup(r => r.AddAsync(It.IsAny<Attendance>())).Returns(Task.CompletedTask);

        var (success, message) = await _sut.CheckInAsync("Office", "test@wms.com");

        Assert.True(success);
    }

    [Fact]
    public async Task CheckInAsync_Failure_AlreadyCheckedIn()
    {
        _empRepo.Setup(r => r.GetByEmailAsync("test@wms.com"))
            .ReturnsAsync(new Employee { EmployeeId = 1 });
        _repo.Setup(r => r.HasCheckedInTodayAsync(1)).ReturnsAsync(true);

        var (success, message) = await _sut.CheckInAsync("Office", "test@wms.com");

        Assert.False(success);
        Assert.Contains("already", message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task CheckOutAsync_Success()
    {
        var today = new Attendance { AttendanceId = 1, EmpId = 1, CheckIn = DateTime.UtcNow.AddHours(-8), CheckOut = null };
        _empRepo.Setup(r => r.GetByEmailAsync("test@wms.com"))
            .ReturnsAsync(new Employee { EmployeeId = 1 });
        _repo.Setup(r => r.GetTodayByEmployeeAsync(1)).ReturnsAsync(today);
        _repo.Setup(r => r.UpdateAsync(It.IsAny<Attendance>())).Returns(Task.CompletedTask);

        var (success, _) = await _sut.CheckOutAsync("test@wms.com");

        Assert.True(success);
        Assert.NotNull(today.CheckOut);
        Assert.NotNull(today.TotalHours);
    }

    [Fact]
    public async Task CheckOutAsync_Failure_NoCheckInToday()
    {
        _empRepo.Setup(r => r.GetByEmailAsync("test@wms.com"))
            .ReturnsAsync(new Employee { EmployeeId = 1 });
        _repo.Setup(r => r.GetTodayByEmployeeAsync(1)).ReturnsAsync((Attendance?)null);

        var (success, message) = await _sut.CheckOutAsync("test@wms.com");

        Assert.False(success);
    }
}
