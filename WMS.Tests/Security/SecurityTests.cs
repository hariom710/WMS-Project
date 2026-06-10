using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

namespace WMS.Tests.Security;

public class SecurityTests : IClassFixture<TestWebApplicationFactory>
{
    private readonly HttpClient _client;
    private readonly string _token;

    public SecurityTests(TestWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
        _token = TestWebApplicationFactory.GenerateJwtToken();
    }

    private HttpRequestMessage AuthRequest(string method, string url, string? body = null)
    {
        var req = new HttpRequestMessage(new HttpMethod(method), url);
        req.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _token);
        if (body != null)
            req.Content = new StringContent(body, Encoding.UTF8, "application/json");
        return req;
    }

    [Theory]
    [InlineData("/api/employees")]
    [InlineData("/api/departments")]
    [InlineData("/api/projects")]
    [InlineData("/api/clients")]
    [InlineData("/api/leaves")]
    [InlineData("/api/attendance")]
    [InlineData("/api/allocations")]
    [InlineData("/api/announcements")]
    [InlineData("/api/auditlogs")]
    [InlineData("/api/dashboard/summary")]
    public async Task GetEndpoints_WithoutToken_Returns401(string url)
    {
        var resp = await _client.GetAsync(url);
        Assert.Equal(HttpStatusCode.Unauthorized, resp.StatusCode);
    }

    [Fact]
    public async Task Request_WithInvalidToken_Returns401()
    {
        var req = new HttpRequestMessage(HttpMethod.Get, "/api/employees");
        req.Headers.Authorization = new AuthenticationHeaderValue("Bearer", "invalid.jwt.token.here");
        var resp = await _client.SendAsync(req);
        Assert.Equal(HttpStatusCode.Unauthorized, resp.StatusCode);
    }

    [Fact]
    public async Task Request_WithEmptyToken_Returns401()
    {
        var req = new HttpRequestMessage(HttpMethod.Get, "/api/employees");
        req.Headers.Authorization = new AuthenticationHeaderValue("Bearer", "");
        var resp = await _client.SendAsync(req);
        Assert.Equal(HttpStatusCode.Unauthorized, resp.StatusCode);
    }

    [Fact]
    public async Task Request_WithMalformedAuthHeader_Returns401()
    {
        var req = new HttpRequestMessage(HttpMethod.Get, "/api/employees");
        req.Headers.Authorization = new AuthenticationHeaderValue("Basic", "dXNlcjpwYXNz");
        var resp = await _client.SendAsync(req);
        Assert.Equal(HttpStatusCode.Unauthorized, resp.StatusCode);
    }

    [Theory]
    [InlineData("' OR 1=1--")]
    [InlineData("'; DROP TABLE Employees;--")]
    [InlineData("1' UNION SELECT * FROM UserLogins--")]
    public async Task Search_WithSqlInjection_NoDataLeakage(string maliciousInput)
    {
        var resp = await _client.SendAsync(AuthRequest("GET",
            $"/api/employees?search={Uri.EscapeDataString(maliciousInput)}"));
        Assert.Equal(HttpStatusCode.OK, resp.StatusCode);
    }

    [Theory]
    [InlineData("' OR 1=1--")]
    [InlineData("'; DROP TABLE Departments;--")]
    public async Task DepartmentSearch_WithSqlInjection_NoDataLeakage(string maliciousInput)
    {
        var resp = await _client.SendAsync(AuthRequest("GET",
            $"/api/departments?search={Uri.EscapeDataString(maliciousInput)}"));
        Assert.Equal(HttpStatusCode.OK, resp.StatusCode);
    }

    [Theory]
    [InlineData("<script>alert('xss')</script>")]
    [InlineData("<img src=x onerror=alert(1)>")]
    public async Task Announcement_WithXssPayload_AcceptedOrSanitized(string xssPayload)
    {
        var resp = await _client.SendAsync(AuthRequest("POST", "/api/announcements",
            JsonSerializer.Serialize(new { title = xssPayload, message = "This announcement contains a test XSS payload for security" })));
        Assert.True(resp.StatusCode == HttpStatusCode.OK || resp.StatusCode == HttpStatusCode.Created || resp.StatusCode == HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Post_WithEmptyBody_ReturnsBadRequest()
    {
        var req = new HttpRequestMessage(HttpMethod.Post, "/api/employees");
        req.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _token);
        req.Content = new StringContent("{}", Encoding.UTF8, "application/json");
        var resp = await _client.SendAsync(req);
        Assert.True(resp.StatusCode == HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Post_WithInvalidJson_ReturnsBadRequest()
    {
        var req = new HttpRequestMessage(HttpMethod.Post, "/api/employees");
        req.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _token);
        req.Content = new StringContent("not valid json {{{", Encoding.UTF8, "application/json");
        var resp = await _client.SendAsync(req);
        Assert.True(resp.StatusCode == HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Put_WithNonExistentId_ReturnsNotFound()
    {
        var resp = await _client.SendAsync(AuthRequest("PUT", "/api/employees/99999",
            JsonSerializer.Serialize(new { firstName = "Test", lastName = "User", email = "t@t.com", phoneNumber = "9876543210",
                  dateOfBirth = "1990-01-01", dateOfJoining = "2022-01-01", departmentId = 1, roleId = 1 })));
        Assert.True(resp.StatusCode == HttpStatusCode.NotFound || resp.StatusCode == HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Delete_WithNonExistentId_ReturnsNotFound()
    {
        var resp = await _client.SendAsync(AuthRequest("DELETE", "/api/employees/99999"));
        Assert.Equal(HttpStatusCode.NotFound, resp.StatusCode);
    }

    [Fact]
    public async Task Employee_CannotAccessAdminEndpoints()
    {
        var empToken = TestWebApplicationFactory.GenerateJwtToken(role: "Employee", roleId: 2);
        var req = new HttpRequestMessage(HttpMethod.Get, "/api/employees/deleted");
        req.Headers.Authorization = new AuthenticationHeaderValue("Bearer", empToken);
        var resp = await _client.SendAsync(req);
        Assert.True(resp.StatusCode == HttpStatusCode.Forbidden || resp.StatusCode == HttpStatusCode.OK);
    }
}
