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
        var useKorean = string.Equals(request.Language, "ko", StringComparison.OrdinalIgnoreCase);
        var builder = new StringBuilder();

        builder.AppendLine($"# {Text(useKorean, "Alarm Issue Report", "알람 이슈 리포트")}");
        builder.AppendLine();
        builder.AppendLine($"- {Text(useKorean, "Generated", "생성 시각")}: {document.GeneratedAt:yyyy-MM-dd HH:mm:ss zzz}");
        builder.AppendLine($"- Recipe: {request.RecipeName}");
        builder.AppendLine($"- Fault Scenario: {request.FaultScenario}");
        builder.AppendLine($"- Alarm: {request.AlarmCode} | {request.AlarmTitle}");
        builder.AppendLine($"- {Text(useKorean, "Severity", "심각도")}: {SeverityText(request.Severity, useKorean)}");
        builder.AppendLine();

        builder.AppendLine($"## {Text(useKorean, "Alarm summary", "알람 요약")}");
        builder.AppendLine();
        builder.AppendLine(request.Summary);
        builder.AppendLine();

        builder.AppendLine($"## {Text(useKorean, "Current process snapshot", "현재 공정 Snapshot")}");
        builder.AppendLine();
        builder.AppendLine($"| {Text(useKorean, "Field", "항목")} | {Text(useKorean, "Value", "값")} |");
        builder.AppendLine("| --- | --- |");
        builder.AppendLine($"| Step | {EscapeCell(request.CurrentStepName)} |");
        builder.AppendLine($"| {Text(useKorean, "Step index", "Step 순번")} | {request.StepIndex}/{request.StepCount} |");
        builder.AppendLine($"| Cycle | {EscapeCell(request.CycleText)} |");
        builder.AppendLine($"| {Text(useKorean, "Pressure", "Chamber 압력")} | {request.ChamberPressureMtorr:0} mTorr |");
        builder.AppendLine($"| {Text(useKorean, "Wafer temperature", "Wafer 온도")} | {request.WaferTemperatureC:0} C |");
        builder.AppendLine($"| {Text(useKorean, "Estimated film", "예상 막 두께")} | {request.EstimatedThicknessAngstrom:0.0} A |");
        builder.AppendLine($"| Precursor Valve | {ValveText(request.MetalPrecursorValveOpen)} |");
        builder.AppendLine($"| Reactant Valve | {ValveText(request.ReactantValveOpen)} |");
        builder.AppendLine($"| Purge Valve | {ValveText(request.PurgeValveOpen)} |");
        builder.AppendLine();

        builder.AppendLine($"## {Text(useKorean, "Operator checklist", "작업자 점검 목록")}");
        builder.AppendLine();
        builder.AppendLine($"| ID | {Text(useKorean, "Required", "필수")} | {Text(useKorean, "Status", "상태")} | {Text(useKorean, "Check item", "점검 항목")} |");
        builder.AppendLine("| --- | --- | --- | --- |");
        foreach (var check in request.Checks)
        {
            builder.AppendLine($"| {EscapeCell(check.Id)} | {YesNo(check.Required, useKorean)} | {CheckStatus(check.IsChecked, useKorean)} | {EscapeCell(check.Label)} |");
        }

        builder.AppendLine();
        builder.AppendLine($"## {Text(useKorean, "Selected response", "선택한 대응")}");
        builder.AppendLine();
        if (request.SelectedChoice == null)
        {
            builder.AppendLine(Text(
                useKorean,
                "No response choice was selected before export.",
                "리포트를 저장하기 전에 대응 선택지를 선택하지 않았습니다."));
        }
        else
        {
            builder.AppendLine($"- {Text(useKorean, "Choice", "선택")}: {request.SelectedChoice.Id} | {request.SelectedChoice.Label}");
            builder.AppendLine($"- {Text(useKorean, "Next action", "다음 조치")}: {request.SelectedChoice.NextAction}");
            builder.AppendLine($"- {Text(useKorean, "Requires engineer", "엔지니어 검토 필요")}: {YesNo(request.SelectedChoice.RequiresEngineer, useKorean)}");
        }

        builder.AppendLine();
        builder.AppendLine($"## {Text(useKorean, "Escalation conditions", "엔지니어 검토 요청 조건")}");
        builder.AppendLine();
        foreach (var condition in request.EscalationConditions)
        {
            builder.AppendLine($"- {condition}");
        }

        builder.AppendLine();
        builder.AppendLine("## Engineering Trace");
        builder.AppendLine();
        builder.AppendLine($"| {Text(useKorean, "Time", "시각")} | {Text(useKorean, "Source", "출처")} | {Text(useKorean, "Message", "메시지")} |");
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

    private static string CheckStatus(bool isChecked, bool useKorean)
    {
        return useKorean
            ? isChecked ? "완료" : "대기"
            : isChecked ? "DONE" : "PENDING";
    }

    private static string YesNo(bool value, bool useKorean)
    {
        return useKorean
            ? value ? "예" : "아니요"
            : value ? "YES" : "NO";
    }

    private static string SeverityText(string severity, bool useKorean)
    {
        if (!useKorean)
        {
            return severity;
        }

        return severity switch
        {
            "Critical" => "위험",
            "Warning" => "경고",
            "Info" => "정보",
            _ => severity
        };
    }

    private static string Text(bool useKorean, string english, string korean)
    {
        return useKorean ? korean : english;
    }

    private sealed record AlarmIssueReportFile(
        DateTimeOffset GeneratedAt,
        AlarmIssueReportExportRequest Request);
}

public sealed record AlarmIssueReportExportRequest(
    string Language,
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
