using System.IO;
using System.Text;
using System.Text.Json;

namespace EquipmentTwin.Hmi.Wpf.Services;

public sealed class AlarmIssueReportOutboxService
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        PropertyNameCaseInsensitive = true,
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
            AlarmIssueReportOutboxStatuses.Queued,
            "local-demo-outbox",
            0,
            null,
            null,
            null,
            payload);

        return Write(path, envelope);
    }

    public AlarmIssueReportOutboxResult Read(string path)
    {
        var envelope = ReadEnvelope(path);
        return ToResult(path, envelope);
    }

    public AlarmIssueReportOutboxResult BeginSendAttempt(string path)
    {
        var envelope = ReadEnvelope(path);
        switch (envelope.Status)
        {
            case AlarmIssueReportOutboxStatuses.Sent:
                throw new InvalidOperationException("The outbox payload was already sent and cannot be sent again.");
            case AlarmIssueReportOutboxStatuses.Sending:
                throw new InvalidOperationException("The outbox payload already has a send attempt in progress.");
            case AlarmIssueReportOutboxStatuses.Queued:
            case AlarmIssueReportOutboxStatuses.Failed:
                break;
            default:
                throw new InvalidOperationException($"Unsupported outbox status '{envelope.Status}'.");
        }

        var updated = envelope with
        {
            Status = AlarmIssueReportOutboxStatuses.Sending,
            AttemptCount = envelope.AttemptCount + 1,
            LastAttemptAt = DateTimeOffset.Now,
            LastError = null
        };

        return Write(path, updated);
    }

    public AlarmIssueReportOutboxResult MarkSent(string path)
    {
        var envelope = RequireSending(path);
        var updated = envelope with
        {
            Status = AlarmIssueReportOutboxStatuses.Sent,
            SentAt = DateTimeOffset.Now,
            LastError = null
        };

        return Write(path, updated);
    }

    public AlarmIssueReportOutboxResult MarkFailed(string path, string error)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(error);

        var envelope = RequireSending(path);
        var updated = envelope with
        {
            Status = AlarmIssueReportOutboxStatuses.Failed,
            LastError = error.Trim()
        };

        return Write(path, updated);
    }

    private static AlarmIssueReportOutboxEnvelope RequireSending(string path)
    {
        var envelope = ReadEnvelope(path);
        if (!string.Equals(envelope.Status, AlarmIssueReportOutboxStatuses.Sending, StringComparison.Ordinal))
        {
            throw new InvalidOperationException(
                $"Outbox status must be '{AlarmIssueReportOutboxStatuses.Sending}' but was '{envelope.Status}'.");
        }

        return envelope;
    }

    private static AlarmIssueReportOutboxEnvelope ReadEnvelope(string path)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);
        if (!File.Exists(path))
        {
            throw new FileNotFoundException("Outbox payload file was not found.", path);
        }

        var json = File.ReadAllText(path, Encoding.UTF8);
        return JsonSerializer.Deserialize<AlarmIssueReportOutboxEnvelope>(json, JsonOptions)
            ?? throw new InvalidDataException("Outbox payload JSON could not be deserialized.");
    }

    private static AlarmIssueReportOutboxResult Write(
        string path,
        AlarmIssueReportOutboxEnvelope envelope)
    {
        File.WriteAllText(path, JsonSerializer.Serialize(envelope, JsonOptions), Encoding.UTF8);
        return ToResult(path, envelope);
    }

    private static AlarmIssueReportOutboxResult ToResult(
        string path,
        AlarmIssueReportOutboxEnvelope envelope)
    {
        return new AlarmIssueReportOutboxResult(
            envelope.EnvelopeId,
            path,
            envelope.Status,
            envelope.AttemptCount,
            envelope.LastAttemptAt,
            envelope.SentAt,
            envelope.LastError);
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
}

public static class AlarmIssueReportOutboxStatuses
{
    public const string Queued = "queued";
    public const string Sending = "sending";
    public const string Sent = "sent";
    public const string Failed = "failed";
}

public sealed record AlarmIssueReportOutboxEnvelope(
    string EnvelopeId,
    DateTimeOffset CreatedAt,
    string MessageType,
    string Status,
    string Target,
    int AttemptCount,
    DateTimeOffset? LastAttemptAt,
    DateTimeOffset? SentAt,
    string? LastError,
    AlarmIssueReportExportRequest Payload);

public sealed record AlarmIssueReportOutboxResult(
    string EnvelopeId,
    string Path,
    string Status,
    int AttemptCount,
    DateTimeOffset? LastAttemptAt,
    DateTimeOffset? SentAt,
    string? LastError);
