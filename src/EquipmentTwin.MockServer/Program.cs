using System.Text;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);
builder.WebHost.UseUrls("http://127.0.0.1:5088");

var app = builder.Build();

app.MapGet("/health", () => Results.Ok(new
{
    status = "ok",
    service = "EquipmentTwin.MockServer"
}));

app.MapPost("/alarm-issue-report", async (HttpRequest request) =>
{
    using var reader = new StreamReader(request.Body, Encoding.UTF8);
    var body = await reader.ReadToEndAsync();
    if (string.IsNullOrWhiteSpace(body))
    {
        return Results.BadRequest(new
        {
            status = "rejected",
            reason = "empty request body"
        });
    }

    var receivedAt = DateTimeOffset.Now;
    var outputDirectory = Path.Combine(ResolveRepositoryRoot(), "artifacts", "mock-server-received");
    Directory.CreateDirectory(outputDirectory);

    var fileName = $"received-alarm-issue-report-{receivedAt:yyyyMMdd-HHmmss-fff}.json";
    var outputPath = Path.Combine(outputDirectory, fileName);
    var envelope = new
    {
        receivedAt,
        source = "EquipmentTwin.MockServer",
        endpoint = "/alarm-issue-report",
        contentType = request.ContentType ?? "unknown",
        body = TryFormatJson(body)
    };

    await File.WriteAllTextAsync(
        outputPath,
        JsonSerializer.Serialize(envelope, new JsonSerializerOptions { WriteIndented = true }),
        Encoding.UTF8);

    Console.WriteLine($"[{receivedAt:HH:mm:ss}] received alarm issue report -> {outputPath}");

    return Results.Ok(new
    {
        status = "received",
        path = outputPath
    });
});

Console.WriteLine("EquipmentTwin.MockServer listening on http://127.0.0.1:5088");
Console.WriteLine("POST alarm issue reports to http://127.0.0.1:5088/alarm-issue-report");

await app.RunAsync();

static string ResolveRepositoryRoot()
{
    var current = new DirectoryInfo(AppContext.BaseDirectory);
    while (current != null)
    {
        if (File.Exists(Path.Combine(current.FullName, "EquipmentTwinLab.sln")))
        {
            return current.FullName;
        }

        current = current.Parent;
    }

    return AppContext.BaseDirectory;
}

static JsonElement? TryParseJson(string body)
{
    try
    {
        return JsonSerializer.Deserialize<JsonElement>(body);
    }
    catch (JsonException)
    {
        return null;
    }
}

static object TryFormatJson(string body)
{
    return TryParseJson(body) is { } json ? json : body;
}
