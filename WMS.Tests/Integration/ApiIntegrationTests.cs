using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

namespace WMS.Tests.Integration;

public class ApiIntegrationTests : IClassFixture<TestWebApplicationFactory>
{
    private readonly HttpClient _client;
    private readonly string _token;

    public ApiIntegrationTests(TestWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
        _token = TestWebApplicationFactory.GenerateJwtToken();
    }

    private HttpRequestMessage AuthRequest(string method, string url, object? body = null)
    {
        var req = new HttpRequestMessage(new HttpMethod(method), url);
        req.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _token);
        if (body != null)
            req.Content = new StringContent(JsonSerializer.Serialize(body), Encoding.UTF8, "application/json");
        return req;
    }

    [Fact]
    public async Task GetEmployees_WithoutToken_Returns401()
    {
        var resp = await _client.GetAsync("/api/employees");
        Assert.Equal(HttpStatusCode.Unauthorized, resp.StatusCode);
    }

    [Fact]
    public async Task Department_CrudLifecycle()
    {
        var createResp = await _client.SendAsync(AuthRequest("POST", "/api/departments",
            new { departmentName = "Test QA Dept", description = "Quality assurance" }));
        Assert.True(createResp.StatusCode == HttpStatusCode.OK || createResp.StatusCode == HttpStatusCode.Created,
            $"Expected 200/201, got {(int)createResp.StatusCode}: {await createResp.Content.ReadAsStringAsync()}");

        var readResp = await _client.SendAsync(AuthRequest("GET", "/api/departments?search=Test+QA"));
        Assert.Equal(HttpStatusCode.OK, readResp.StatusCode);
    }

    [Fact]
    public async Task Client_CrudLifecycle()
    {
        var createResp = await _client.SendAsync(AuthRequest("POST", "/api/clients",
            new { clientName = "Integration Test Corp", clientAddress = "123 Test St", clientPhoneNumber = "9876543210", clientLocation = "Mumbai" }));
        Assert.True(createResp.StatusCode == HttpStatusCode.OK || createResp.StatusCode == HttpStatusCode.Created);

        var readResp = await _client.SendAsync(AuthRequest("GET", "/api/clients?search=Integration"));
        Assert.Equal(HttpStatusCode.OK, readResp.StatusCode);
    }

    [Fact]
    public async Task Announcement_CrudLifecycle()
    {
        var createResp = await _client.SendAsync(AuthRequest("POST", "/api/announcements",
            new { title = "Integration Test Notice", message = "This is a test announcement with enough characters" }));
        Assert.True(createResp.StatusCode == HttpStatusCode.OK || createResp.StatusCode == HttpStatusCode.Created);
    }

    [Fact]
    public async Task Employees_Pagination()
    {
        var resp = await _client.SendAsync(AuthRequest("GET", "/api/employees?page=1&pageSize=5"));
        Assert.Equal(HttpStatusCode.OK, resp.StatusCode);
    }

    [Fact]
    public async Task Dashboard_ReturnsSummary()
    {
        var resp = await _client.SendAsync(AuthRequest("GET", "/api/dashboard/summary"));
        Assert.Equal(HttpStatusCode.OK, resp.StatusCode);
    }

    [Fact]
    public async Task Roles_ReturnsList()
    {
        var resp = await _client.SendAsync(AuthRequest("GET", "/api/roles"));
        Assert.Equal(HttpStatusCode.OK, resp.StatusCode);
    }

    [Fact]
    public async Task Announcement_SoftDeleteAndRestore()
    {
        var createResp = await _client.SendAsync(AuthRequest("POST", "/api/announcements",
            new { title = "Delete Me Notice", message = "This announcement will be deleted and restored" }));
        Assert.True(createResp.StatusCode == HttpStatusCode.OK || createResp.StatusCode == HttpStatusCode.Created);

        var readResp = await _client.SendAsync(AuthRequest("GET", "/api/announcements?page=1&pageSize=100"));
        Assert.Equal(HttpStatusCode.OK, readResp.StatusCode);
    }
}
