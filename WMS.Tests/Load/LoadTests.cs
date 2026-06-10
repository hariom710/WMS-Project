using System.Diagnostics;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;

namespace WMS.Tests.Load;

public class LoadTests : IClassFixture<TestWebApplicationFactory>
{
    private readonly HttpClient _client;
    private readonly string _token;

    public LoadTests(TestWebApplicationFactory factory)
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
    public async Task Concurrent10Users_Dashboard_NoErrors()
    {
        await RunConcurrentTest(10, "/api/dashboard/summary");
    }

    [Fact]
    public async Task Concurrent25Users_Dashboard_NoErrors()
    {
        await RunConcurrentTest(25, "/api/dashboard/summary");
    }

    [Fact]
    public async Task Concurrent50Users_Dashboard_NoErrors()
    {
        await RunConcurrentTest(50, "/api/dashboard/summary");
    }

    [Fact]
    public async Task Concurrent10Users_Employees_NoErrors()
    {
        await RunConcurrentTest(10, "/api/employees?pageSize=10");
    }

    [Fact]
    public async Task Concurrent25Users_Employees_NoErrors()
    {
        await RunConcurrentTest(25, "/api/employees?pageSize=10");
    }

    [Fact]
    public async Task Concurrent50Users_Employees_NoErrors()
    {
        await RunConcurrentTest(50, "/api/employees?pageSize=10");
    }

    [Fact]
    public async Task Concurrent10Users_Leaves_NoErrors()
    {
        await RunConcurrentTest(10, "/api/leaves?pageSize=10");
    }

    [Fact]
    public async Task Concurrent25Users_Attendance_NoErrors()
    {
        await RunConcurrentTest(25, "/api/attendance?pageSize=10");
    }

    [Fact]
    public async Task Concurrent50Users_Roles_NoErrors()
    {
        await RunConcurrentTest(50, "/api/roles");
    }

    [Fact]
    public async Task Concurrent10Users_Reports_Excel_NoErrors()
    {
        await RunConcurrentTest(10, "/api/Reports/employees/excel");
    }

    [Fact]
    public async Task Concurrent25Users_Reports_Pdf_NoErrors()
    {
        await RunConcurrentTest(25, "/api/Reports/dashboard/pdf");
    }

    private async Task RunConcurrentTest(int userCount, string endpoint)
    {
        var stopwatch = Stopwatch.StartNew();
        var tasks = new Task<HttpResponseMessage?>[userCount];

        for (int i = 0; i < userCount; i++)
        {
            tasks[i] = Task.Run(async () =>
            {
                try
                {
                    return await _client.SendAsync(AuthGet(endpoint));
                }
                catch
                {
                    return null;
                }
            });
        }

        var results = await Task.WhenAll(tasks);
        stopwatch.Stop();

        var successCount = results.Count(r => r != null && (r.StatusCode == System.Net.HttpStatusCode.OK || r.StatusCode == System.Net.HttpStatusCode.BadRequest));
        var failCount = userCount - successCount;
        var avgMs = stopwatch.ElapsedMilliseconds / (double)userCount;

        Assert.True(failCount == 0, $"{failCount}/{userCount} requests failed");
        Assert.True(avgMs < 5000, $"Average response time {avgMs:F0}ms exceeds 5s threshold");
    }
}
