using Moq;
using WMS.Domain.Interfaces;
using WMS.Domain.Models;
using WMS.Infrastructure.Services;

namespace WMS.Tests.Services;

public class AllocationServiceTests
{
    private readonly Mock<IAllocationRepository> _repo = new();
    private readonly Mock<IEmployeeRepository> _empRepo = new();
    private readonly Mock<IProjectRepository> _projRepo = new();
    private readonly Mock<IActivityLogService> _activityLog = new();
    private readonly AllocationService _sut;

    public AllocationServiceTests()
    {
        _sut = new AllocationService(_repo.Object, _empRepo.Object, _projRepo.Object, _activityLog.Object);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsPagedResult()
    {
        var items = new List<ProjectAllocation> { new() { AllocationId = 1, EmpId = 1 } };
        _repo.Setup(r => r.GetAllAsync(null, null, null, 1, 10))
            .ReturnsAsync(new PagedResult<ProjectAllocation>(items, 1, 1, 10));

        var result = await _sut.GetAllAsync(null, null, null, 1, 10);

        Assert.Single(result.Items);
    }

    [Fact]
    public async Task CreateAsync_Success_WhenValid()
    {
        _empRepo.Setup(r => r.ExistsAsync(1)).ReturnsAsync(true);
        _projRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new Project { ProjectId = 1, Status = "Active" });
        _empRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new Employee { EmployeeId = 1, Status = "Active" });
        _repo.Setup(r => r.ExistsActiveAsync(1, 1)).ReturnsAsync(false);
        _repo.Setup(r => r.AddAsync(It.IsAny<ProjectAllocation>())).Returns(Task.CompletedTask);

        var alloc = new ProjectAllocation { EmpId = 1, ProjectId = 1 };
        var (success, message) = await _sut.CreateAsync(alloc, "admin@wms.com");

        Assert.True(success);
        Assert.Equal("admin@wms.com", alloc.CreatedBy);
    }

    [Fact]
    public async Task CreateAsync_Failure_WhenEmployeeNotExists()
    {
        _empRepo.Setup(r => r.ExistsAsync(999)).ReturnsAsync(false);

        var alloc = new ProjectAllocation { EmpId = 999, ProjectId = 1 };
        var (success, message) = await _sut.CreateAsync(alloc, "admin@wms.com");

        Assert.False(success);
        Assert.Contains("Employee", message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task CreateAsync_Failure_WhenProjectNotExists()
    {
        _empRepo.Setup(r => r.ExistsAsync(1)).ReturnsAsync(true);
        _projRepo.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((Project?)null);

        var alloc = new ProjectAllocation { EmpId = 1, ProjectId = 999 };
        var (success, message) = await _sut.CreateAsync(alloc, "admin@wms.com");

        Assert.False(success);
        Assert.Contains("Project", message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task CreateAsync_Failure_WhenDuplicateAllocation()
    {
        _empRepo.Setup(r => r.ExistsAsync(1)).ReturnsAsync(true);
        _projRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new Project { ProjectId = 1, Status = "Active" });
        _empRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new Employee { EmployeeId = 1, Status = "Active" });
        _repo.Setup(r => r.ExistsActiveAsync(1, 1)).ReturnsAsync(true);

        var alloc = new ProjectAllocation { EmpId = 1, ProjectId = 1 };
        var (success, message) = await _sut.CreateAsync(alloc, "admin@wms.com");

        Assert.False(success);
        Assert.Contains("already", message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task SoftDeleteAsync_Success_WhenAllocExists()
    {
        var allocs = new PagedResult<ProjectAllocation>(
            new List<ProjectAllocation> { new() { AllocationId = 1, EmpId = 1, ProjectId = 1 } }, 1, 1, int.MaxValue);
        _repo.Setup(r => r.GetAllAsync(null, null, null, 1, int.MaxValue)).ReturnsAsync(allocs);
        _repo.Setup(r => r.SoftDeleteAsync(It.IsAny<ProjectAllocation>(), It.IsAny<string?>())).Returns(Task.CompletedTask);

        var (success, message) = await _sut.SoftDeleteAsync(1, "admin");

        Assert.True(success);
    }

    [Fact]
    public async Task SoftDeleteAsync_Failure_WhenNotExists()
    {
        var empty = new PagedResult<ProjectAllocation>(new List<ProjectAllocation>(), 0, 1, int.MaxValue);
        _repo.Setup(r => r.GetAllAsync(null, null, null, 1, int.MaxValue)).ReturnsAsync(empty);

        var (success, _) = await _sut.SoftDeleteAsync(999, "admin");

        Assert.False(success);
    }

    [Fact]
    public async Task RestoreAsync_True_WhenDeletedExists()
    {
        var deleted = new PagedResult<ProjectAllocation>(
            new List<ProjectAllocation> { new() { AllocationId = 1, EmpId = 1, ProjectId = 1 } }, 1, 1, 1000);
        _repo.Setup(r => r.GetDeletedAsync(null, null, null, 1, int.MaxValue)).ReturnsAsync(deleted);
        _repo.Setup(r => r.RestoreAsync(It.IsAny<ProjectAllocation>())).Returns(Task.CompletedTask);

        var result = await _sut.RestoreAsync(1);

        Assert.True(result);
    }

    [Fact]
    public async Task RestoreAsync_False_WhenNotDeleted()
    {
        var empty = new PagedResult<ProjectAllocation>(new List<ProjectAllocation>(), 0, 1, 1000);
        _repo.Setup(r => r.GetDeletedAsync(null, null, null, 1, int.MaxValue)).ReturnsAsync(empty);

        var result = await _sut.RestoreAsync(999);

        Assert.False(result);
    }
}
