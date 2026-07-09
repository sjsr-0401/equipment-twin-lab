using System.IO;
using System.Text;
using System.Text.Json;

namespace EquipmentTwin.Hmi.Wpf.Services;

public sealed class AlarmIssueReportExportService
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = true
    };

    public AlarmIssueReportExportResult Export(AlarmIssueReportExportRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        var generatedAt = DateTimeOffset.Now;
        var outputDirectory = Path.Combine(ResolveRepositoryRoot(), "artifacts", "alarm-reports");
        Directory.CreateDirectory(outputDirectory);

        var fileBaseName = $"alarm-report-{generatedAt:yyyyMMdd-HHmmss}-{SafeFilePart(request.AlarmCode)}";
        var jsonPath = Path.Combine(outputDirectory, $"{fileBaseName}.json");
        var markdownPath = Path.Combine(outputDirectory, $"{fileBaseName}.md");
        var document = new AlarmIssueReportFile(generatedAt, request);

        File.WriteAllText(jsonPath, JsonSerializer.Serialize(document, JsonOptions), Encoding.UTF8);
        File.WriteAllText(markdownPath, BuildMarkdown(document), Encoding.UTF8);

        return new AlarmIssueReportExportResult(jsonPath, markdownPath);
    }

    private static string BuildMarkdown(AlarmIssueReportFile document)
    {
        var request = document.Request;
        var builder = new StringBuilder();

        builder.AppendLine("# Alarm Issue Report");
        builder.AppendLine();
        builder.AppendLine($"- Generated: {document.GeneratedAt:yyyy-MM-dd HH:mm:ss zzz}");
        builder.AppendLine($"- Recipe: {request.RecipeName}");
        builder.AppendLine($"- Fault scenario: {request.FaultScenario}");
        builder.AppendLine($"- Alarm: {request.AlarmCode} | {request.AlarmTitle}");
        builder.AppendLine($"- Severity: {request.Severity}");
        builder.AppendLine();

        builder.AppendLine("## Alarm summary");
        builder.AppendLine();
        builder.AppendLine(request.Summary);
        builder.AppendLine();

        builder.AppendLine("## Current process snapshot");
        builder.AppendLine();
        builder.AppendLine("| Field | Value |");
        builder.AppendLine("| --- | --- |");
        builder.AppendLine($"| Step | {EscapeCell(request.CurrentStepName)} |");
        builder.AppendLine($"| Step index | {request.StepIndex}/{request.StepCount} |");
        builder.AppendLine($"| Cycle | {EscapeCell(request.CycleText)} |");
        builder.AppendLine($"| Pressure | {request.ChamberPressureMtorr:0} mTorr |");
        builder.AppendLine($"| Wafer temperature | {request.WaferTemperatureC:0} C |");
        builder.AppendLine($"| Estimated film | {request.EstimatedThicknessAngstrom:0.0} A |");
        builder.AppendLine($"| Precursor valve | {ValveText(request.MetalPrecursorValveOpen)} |");
        builder.AppendLine($"| Reactant valve | {ValveText(request.ReactantValveOpen)} |");
        builder.AppendLine($"| Purge valve | {ValveText(request.PurgeValveOpen)} |");
        builder.AppendLine();

        builder.AppendLine("## Operator checklist");
        builder.AppendLine();
        builder.AppendLine("| ID | Required | Status | Check item |");
        builder.AppendLine("| --- | --- | --- | --- |");
        foreach (var check in request.Checks)
        {
            builder.AppendLine($"| {EscapeCell(check.Id)} | {YesNo(check.Required)} | {CheckStatus(check.IsChecked)} | {EscapeCell(check.Label)} |");
        }

        builder.AppendLine();
        builder.AppendLine("## Selected response");
        builder.AppendLine();
        if (request.SelectedChoice == null)
        {
            builder.AppendLine("No response choice was selected before export.");
        }
        else
        {
            builder.AppendLine($"- Choice: {request.SelectedChoice.Id} | {request.SelectedChoice.Label}");
            builder.AppendLine($"- Next action: {request.SelectedChoice.NextAction}");
            builder.AppendLine($"- Requires engineer: {YesNo(request.SelectedChoice.RequiresEngineer)}");
        }

        builder.AppendLine();
        builder.AppendLine("## Escalation conditions");
        builder.AppendLine();
        foreach (var condition in request.EscalationConditions)
        {
            builder.AppendLine($"- {condition}");
        }

        builder.AppendLine();
        builder.AppendLine("## Engineering trace");
        builder.AppendLine();
        builder.AppendLine("| Time | Source | Message |");
        builder.AppendLine("| --- | --- | --- |");
        foreach (var entry in request.TraceEntries)
        {
            builder.AppendLine($"| {EscapeCell(entry.Time)} | {EscapeCell(entry.Source)} | {EscapeCell(entry.Message)} |");
        }

        return builder.ToString();
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

    private static string EscapeCell(string value)
    {
        return value
            .Replace("\r", " ", StringComparison.Ordinal)
            .Replace("\n", " ", StringComparison.Ordinal)
            .Replace("|", "\\|", StringComparison.Ordinal);
    }

    private static string ValveText(bool isOpen)
    {
        return isOpen ? "ON" : "OFF";
    }

    private static string CheckStatus(bool isChecked)
    {
        return isChecked ? "DONE" : "PENDING";
    }

    private static string YesNo(bool value)
    {
        return value ? "YES" : "NO";
    }

    private sealed record AlarmIssueReportFile(
        DateTimeOffset GeneratedAt,
        AlarmIssueReportExportRequest Request);
}

public sealed record AlarmIssueReportExportRequest(
    string RecipeName,
    string FaultScenario,
    string AlarmCode,
    string AlarmTitle,
    string Severity,
    string Summary,
    string CurrentStepName,
    int StepIndex,
    int StepCount,
    string CycleText,
    double ChamberPressureMtorr,
    double WaferTemperatureC,
    double EstimatedThicknessAngstrom,
    bool MetalPrecursorValveOpen,
    bool ReactantValveOpen,
    bool PurgeValveOpen,
    IReadOnlyList<AlarmIssueReportCheck> Checks,
    AlarmIssueReportChoice? SelectedChoice,
    IReadOnlyList<string> EscalationConditions,
    IReadOnlyList<AlarmIssueReportTraceEntry> TraceEntries);

public sealed record AlarmIssueReportCheck(
    string Id,
    string Label,
    bool Required,
    bool IsChecked);

public sealed record AlarmIssueReportChoice(
    string Id,
    string Label,
    string NextAction,
    bool RequiresEngineer);

public sealed record AlarmIssueReportTraceEntry(
    string Time,
    string Source,
    string Message);

public sealed record AlarmIssueReportExportResult(
    string JsonPath,
    string MarkdownPath);
