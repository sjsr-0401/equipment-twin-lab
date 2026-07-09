using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace EquipmentTwin.Hmi.Wpf.Services;

public sealed class MockServerPayloadSender
{
    private static readonly HttpClient Client = new()
    {
        Timeout = TimeSpan.FromSeconds(5)
    };

    private readonly Uri endpoint = new("http://127.0.0.1:5088/alarm-issue-report");

    public async Task<MockServerSendResult> SendAlarmIssueReportAsync(string payloadPath)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(payloadPath);

        var json = await File.ReadAllTextAsync(payloadPath, Encoding.UTF8);
        using var content = new StringContent(json, Encoding.UTF8);
        content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
        using var response = await Client.PostAsync(endpoint, content);
        var responseBody = await response.Content.ReadAsStringAsync();

        return new MockServerSendResult(
            response.IsSuccessStatusCode,
            (int)response.StatusCode,
            endpoint.ToString(),
            responseBody);
    }
}

public sealed record MockServerSendResult(
    bool Success,
    int StatusCode,
    string Endpoint,
    string ResponseBody);
