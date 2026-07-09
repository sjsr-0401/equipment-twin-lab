using System.IO;
using System.Text;
using System.Text.Json;

namespace EquipmentTwin.Hmi.Wpf.Services;

public sealed class AlarmIssueReportOutboxService
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = true
    };

    public AlarmIssueReportOutboxResult QueueAlarmIssueReport(AlarmIssueReportExportRequest payload)
    {
        ArgumentNullException.ThrowIfNull(payload);

        var createdAt = DateTimeOffset.Now;
        var envelopeId = Guid.NewGuid().ToString("N");
        var outputDirectory = Path.Combine(ResolveRepositoryRoot(), "artifacts", "server-outbox");
        Directory.CreateDirectory(outputDirectory);

        var fileBaseName = $"server-outbox-{createdAt:yyyyMMdd-HHmmss}-{envelopeId[..8]}-{SafeFilePart(payload.AlarmCode)}";
        var path = Path.Combine(outputDirectory, $"{fileBaseName}.json");
        var envelope = new AlarmIssueReportOutboxEnvelope(
            envelopeId,
            createdAt,
            "alarm-issue-report",
            "queued",
            "local-demo-outbox",
            payload);

        File.WriteAllText(path, JsonSerializer.Serialize(envelope, JsonOptions), Encoding.UTF8);

        return new AlarmIssueReportOutboxResult(envelopeId, path, envelope.Status);
    }

    private static string ResolveRepositoryRoot()
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

    private static string SafeFilePart(string value)
    {
        var validCharacters = value
            .Where(character => char.IsLetterOrDigit(character) || character is '-' or '_')
            .ToArray();

        return validCharacters.Length == 0 ? "alarm" : new string(validCharacters);
    }

    private sealed record AlarmIssueReportOutboxEnvelope(
        string EnvelopeId,
        DateTimeOffset CreatedAt,
        string MessageType,
        string Status,
        string Target,
        AlarmIssueReportExportRequest Payload);
}

public sealed record AlarmIssueReportOutboxResult(
    string EnvelopeId,
    string Path,
    string Status);
