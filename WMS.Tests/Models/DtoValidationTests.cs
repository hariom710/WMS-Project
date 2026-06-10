using System.ComponentModel.DataAnnotations;
using WMS.Application.DTOs;

namespace WMS.Tests.Models;

public class DtoValidationTests
{
    private static List<ValidationResult> Validate(object obj)
    {
        var results = new List<ValidationResult>();
        var ctx = new ValidationContext(obj);
        Validator.TryValidateObject(obj, ctx, results, true);
        return results;
    }

    // --- CreateEmployeeDto ---
    [Fact]
    public void CreateEmployeeDto_Valid_NoErrors()
    {
        var dto = new CreateEmployeeDto
        {
            FirstName = "Rahul", LastName = "Sharma", Email = "rahul@test.com",
            PhoneNumber = "9876543210", Gender = "M",
            DateOfBirth = new DateTime(1990, 1, 1), DateOfJoining = new DateTime(2022, 1, 1),
            DepartmentId = 1, RoleId = 1
        };
        Assert.Empty(Validate(dto));
    }

    [Theory]
    [InlineData("")]
    public void CreateEmployeeDto_EmptyFirstName_Fails(string name)
    {
        var dto = new CreateEmployeeDto { FirstName = name, LastName = "Sharma", Email = "t@t.com", PhoneNumber = "9876543210", DepartmentId = 1, RoleId = 1 };
        var errors = Validate(dto);
        Assert.Contains(errors, e => e.ErrorMessage!.Contains("First name"));
    }

    [Theory]
    [InlineData("123")]
    [InlineData("Rahul@#$")]
    public void CreateEmployeeDto_InvalidFirstNameChars(string name)
    {
        var dto = new CreateEmployeeDto { FirstName = name, LastName = "Sharma", Email = "t@t.com", PhoneNumber = "9876543210", DepartmentId = 1, RoleId = 1 };
        var errors = Validate(dto);
        Assert.Contains(errors, e => e.ErrorMessage!.Contains("alphabets"));
    }

    [Theory]
    [InlineData("not-email")]
    [InlineData("test@")]
    public void CreateEmployeeDto_InvalidEmail(string email)
    {
        var dto = new CreateEmployeeDto { FirstName = "Rahul", LastName = "Sharma", Email = email, PhoneNumber = "9876543210", DepartmentId = 1, RoleId = 1 };
        var errors = Validate(dto);
        Assert.Contains(errors, e => e.ErrorMessage!.Contains("email"));
    }

    [Theory]
    [InlineData("12345")]
    [InlineData("12345678901")]
    [InlineData("abcdefghij")]
    public void CreateEmployeeDto_InvalidPhone(string phone)
    {
        var dto = new CreateEmployeeDto { FirstName = "Rahul", LastName = "Sharma", Email = "t@t.com", PhoneNumber = phone, DepartmentId = 1, RoleId = 1 };
        var errors = Validate(dto);
        Assert.Contains(errors, e => e.ErrorMessage!.Contains("10 digits"));
    }

    // --- LoginRequestDto ---
    [Fact]
    public void LoginRequestDto_EmptyUsername_Fails()
    {
        var dto = new LoginRequestDto { Username = "", Password = "pass" };
        var errors = Validate(dto);
        Assert.NotEmpty(errors);
    }

    [Fact]
    public void LoginRequestDto_EmptyPassword_Fails()
    {
        var dto = new LoginRequestDto { Username = "admin", Password = "" };
        var errors = Validate(dto);
        Assert.NotEmpty(errors);
    }

    // --- ChangePasswordRequestDto ---
    [Fact]
    public void ChangePasswordRequestDto_ShortPassword_Fails()
    {
        var dto = new ChangePasswordRequestDto { Username = "admin", OldPassword = "old", NewPassword = "12345" };
        var errors = Validate(dto);
        Assert.Contains(errors, e => e.ErrorMessage!.Contains("6"));
    }

    [Fact]
    public void ChangePasswordRequestDto_Valid_NoErrors()
    {
        var dto = new ChangePasswordRequestDto { Username = "admin", OldPassword = "old", NewPassword = "newpass123" };
        Assert.Empty(Validate(dto));
    }

    // --- PagedResult ---
    [Fact]
    public void PagedResult_CalculatesTotalPages()
    {
        var result = new Domain.Models.PagedResult<int>(new[] { 1, 2, 3 }, 25, 1, 10);
        Assert.Equal(3, result.TotalPages);
        Assert.True(result.HasNext);
        Assert.False(result.HasPrevious);
    }

    [Fact]
    public void PagedResult_Page2_HasPrevious()
    {
        var result = new Domain.Models.PagedResult<int>(new[] { 1 }, 25, 2, 10);
        Assert.True(result.HasPrevious);
        Assert.True(result.HasNext);
    }

    [Fact]
    public void PagedResult_LastPage_NoNext()
    {
        var result = new Domain.Models.PagedResult<int>(new[] { 1 }, 25, 3, 10);
        Assert.False(result.HasNext);
        Assert.True(result.HasPrevious);
    }

    // --- ApiResponse ---
    [Fact]
    public void ApiResponse_Ok_SetsSuccessTrue()
    {
        var resp = API.Helpers.ApiResponse<string>.Ok("test");
        Assert.True(resp.Success);
        Assert.Equal("test", resp.Data);
        Assert.Equal("Success", resp.Message);
    }

    [Fact]
    public void ApiResponse_Ok_WithPagination()
    {
        var pagination = new API.Helpers.PaginationInfo { Page = 1, PageSize = 10, TotalCount = 50 };
        var resp = API.Helpers.ApiResponse<string[]>.Ok(new[] { "a" }, pagination);
        Assert.True(resp.Success);
        Assert.NotNull(resp.Pagination);
        Assert.Equal(5, resp.Pagination.TotalPages);
    }

    [Fact]
    public void ApiResponse_Fail_SetsSuccessFalse()
    {
        var resp = API.Helpers.ApiResponse<string>.Fail("Error occurred");
        Assert.False(resp.Success);
        Assert.Equal("Error occurred", resp.Message);
    }
}
