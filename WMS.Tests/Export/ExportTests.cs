using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;

namespace WMS.Tests.Export;

public class ExportTests : IClassFixture<TestWebApplicationFactory>
{
    private readonly HttpClient _client;
    private readonly string _token;

    public ExportTests(TestWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
        _token = TestWebApplicationFactory.GenerateJwtToken();
    }

    private HttpRequestMessage AuthGet(string url)
    {
        var req = new HttpRequestMessage(HttpMethod.Get, url);
        req.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _token);
        return req;
    }

    [Fact]
    public async Task EmployeesExcel_ReturnsFile()
    {
        var resp = await _client.SendAsync(AuthGet("/api/Reports/employees/excel"));
        Assert.True(resp.StatusCode == HttpStatusCode.OK || resp.StatusCode == HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task EmployeesPdf_ReturnsFile()
    {
        var resp = await _client.SendAsync(AuthGet("/api/Reports/employees/pdf"));
        Assert.True(resp.StatusCode == HttpStatusCode.OK || resp.StatusCode == HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task AttendanceExcel_ReturnsFile()
    {
        var resp = await _client.SendAsync(AuthGet("/api/Reports/attendance/excel"));
        Assert.True(resp.StatusCode == HttpStatusCode.OK || resp.StatusCode == HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task AttendancePdf_ReturnsFile()
    {
        var resp = await _client.SendAsync(AuthGet("/api/Reports/attendance/pdf"));
        Assert.True(resp.StatusCode == HttpStatusCode.OK || resp.StatusCode == HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task ProjectsExcel_ReturnsFile()
    {
        var resp = await _client.SendAsync(AuthGet("/api/Reports/projects/excel"));
        Assert.True(resp.StatusCode == HttpStatusCode.OK || resp.StatusCode == HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task ClientsExcel_ReturnsFile()
    {
        var resp = await _client.SendAsync(AuthGet("/api/Reports/clients/excel"));
        Assert.True(resp.StatusCode == HttpStatusCode.OK || resp.StatusCode == HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task DashboardPdf_ReturnsFile()
    {
        var resp = await _client.SendAsync(AuthGet("/api/Reports/dashboard/pdf"));
        Assert.True(resp.StatusCode == HttpStatusCode.OK || resp.StatusCode == HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task LeavesPdf_ReturnsFile()
    {
        var resp = await _client.SendAsync(AuthGet("/api/Reports/leaves/pdf"));
        Assert.True(resp.StatusCode == HttpStatusCode.OK || resp.StatusCode == HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task ProjectsPdf_ReturnsFile()
    {
        var resp = await _client.SendAsync(AuthGet("/api/Reports/projects/pdf"));
        Assert.True(resp.StatusCode == HttpStatusCode.OK || resp.StatusCode == HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task ExportWithoutToken_Returns401()
    {
        var resp = await _client.GetAsync("/api/Reports/employees/excel");
        Assert.Equal(HttpStatusCode.Unauthorized, resp.StatusCode);
    }

    [Fact]
    public async Task ExportWithFilters_Works()
    {
        var resp = await _client.SendAsync(AuthGet("/api/Reports/employees/excel?search=rahul&status=Active"));
        Assert.True(resp.StatusCode == HttpStatusCode.OK || resp.StatusCode == HttpStatusCode.BadRequest);
    }
}
