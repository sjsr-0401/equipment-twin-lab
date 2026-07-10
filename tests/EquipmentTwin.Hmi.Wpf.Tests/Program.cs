using System.IO;
using System.Text.Json;
using EquipmentTwin.Hmi.Wpf.Services;

var tests = new (string Name, Action Body)[]
{
    ("Korean issue report localizes Markdown and preserves JSON contract", KoreanReportPreservesContract),
    ("English issue report keeps English Markdown", EnglishReportKeepsEnglishMarkdown),
    ("Server outbox preserves envelope and payload contract", ServerOutboxPreservesContract)
};

var failures = 0;

foreach (var test in tests)
{
    try
    {
        test.Body();
        Console.WriteLine($"PASS {test.Name}");
    }
    catch (Exception ex)
    {
        failures++;
        Console.WriteLine($"FAIL {test.Name}");
        Console.WriteLine(ex.Message);
    }
}

if (failures > 0)
{
    Console.WriteLine($"{failures} EquipmentTwin.Hmi.Wpf contract test(s) failed.");
    Environment.Exit(1);
}

Console.WriteLine("All EquipmentTwin.Hmi.Wpf contract tests passed.");
return;

static void KoreanReportPreservesContract()
{
    var result = new AlarmIssueReportExportService().Export(CreateRequest("ko"));

    try
    {
        using var json = JsonDocument.Parse(File.ReadAllText(result.JsonPath));
        var request = json.RootElement.GetProperty("request");
        var markdown = File.ReadAllText(result.MarkdownPath);

        AssertEqual("ko", request.GetProperty("language").GetString(), "Korean report language mismatch.");
        AssertEqual("GAS-301", request.GetProperty("alarmCode").GetString(), "Alarm code must remain canonical.");
        AssertEqual("Warning", request.GetProperty("severity").GetString(), "Severity must remain canonical.");
        AssertContains(markdown, "# 알람 이슈 리포트", "Korean Markdown title missing.");
        AssertContains(markdown, "## 알람 요약", "Korean summary heading missing.");
        AssertContains(markdown, "| 필수 | 상태 | 점검 항목 |", "Korean checklist header missing.");
        AssertContains(markdown, "- 심각도: 경고", "Korean severity text missing.");
        AssertDoesNotContain(markdown, "## Alarm summary", "English summary heading remained in Korean Markdown.");
    }
    finally
    {
        DeleteIfExists(result.JsonPath);
        DeleteIfExists(result.MarkdownPath);
    }
}

static void EnglishReportKeepsEnglishMarkdown()
{
    var result = new AlarmIssueReportExportService().Export(CreateRequest("en"));

    try
    {
        using var json = JsonDocument.Parse(File.ReadAllText(result.JsonPath));
        var request = json.RootElement.GetProperty("request");
        var markdown = File.ReadAllText(result.MarkdownPath);

        AssertEqual("en", request.GetProperty("language").GetString(), "English report language mismatch.");
        AssertEqual("Warning", request.GetProperty("severity").GetString(), "English severity mismatch.");
        AssertContains(markdown, "# Alarm Issue Report", "English Markdown title missing.");
        AssertContains(markdown, "## Alarm summary", "English summary heading missing.");
        AssertContains(markdown, "| Required | Status | Check item |", "English checklist header missing.");
        AssertContains(markdown, "- Severity: Warning", "English severity text missing.");
        AssertDoesNotContain(markdown, "# 알람 이슈 리포트", "Korean title appeared in English Markdown.");
    }
    finally
    {
        DeleteIfExists(result.JsonPath);
        DeleteIfExists(result.MarkdownPath);
    }
}

static void ServerOutboxPreservesContract()
{
    var result = new AlarmIssueReportOutboxService().QueueAlarmIssueReport(CreateRequest("ko"));

    try
    {
        using var json = JsonDocument.Parse(File.ReadAllText(result.Path));
        var root = json.RootElement;
        var payload = root.GetProperty("payload");

        AssertEqual("alarm-issue-report", root.GetProperty("messageType").GetString(), "Outbox message type mismatch.");
        AssertEqual("queued", root.GetProperty("status").GetString(), "Outbox status mismatch.");
        AssertEqual("local-demo-outbox", root.GetProperty("target").GetString(), "Outbox target mismatch.");
        AssertEqual("ko", payload.GetProperty("language").GetString(), "Payload language mismatch.");
        AssertEqual("GAS-301", payload.GetProperty("alarmCode").GetString(), "Payload alarm code mismatch.");
        AssertEqual("Warning", payload.GetProperty("severity").GetString(), "Payload severity must remain canonical.");
        AssertEqual(850.0, payload.GetProperty("chamberPressureMtorr").GetDouble(), "Payload pressure mismatch.");
        AssertEqual("queued", result.Status, "Outbox result status mismatch.");
    }
    finally
    {
        DeleteIfExists(result.Path);
    }
}

static AlarmIssueReportExportRequest CreateRequest(string language)
{
    var korean = string.Equals(language, "ko", StringComparison.OrdinalIgnoreCase);

    return new AlarmIssueReportExportRequest(
        language,
        "public-moly-ald-metallization-demo",
        "precursor-dose-timeout",
        "GAS-301",
        korean ? "Gas 공급 단계 제한 시간 초과" : "Gas Delivery Step Timeout",
        "Warning",
        korean
            ? "모의 Precursor/Reactant/Purge 단계가 설정된 제한 시간 안에 완료되지 않았습니다."
            : "A synthetic precursor, reactant, or purge step failed to complete within the demo process timing rule.",
        korean ? "Precursor 주입" : "Dose Precursor",
        8,
        22,
        "2/4",
        850,
        250,
        2.0,
        true,
        false,
        false,
        new[]
        {
            new AlarmIssueReportCheck(
                "active-valve",
                korean
                    ? "알람이 발생했을 때 어떤 Valve가 동작 중이었는지 확인합니다."
                    : "Confirm which valve was active when the alarm occurred.",
                true,
                true)
        },
        new AlarmIssueReportChoice(
            "valve-not-active",
            korean ? "명령된 Valve가 동작하지 않음" : "Expected valve was not active",
            korean
                ? "Issue Report를 저장하고 Sequence Logic의 연결을 검토합니다."
                : "Capture the issue report and review sequence logic.",
            true),
        new[]
        {
            korean
                ? "Valve 상태가 현재 ALD 단계와 일치하지 않습니다."
                : "Valve state does not match the active ALD step."
        },
        new[]
        {
            new AlarmIssueReportTraceEntry("12:34:56.789", "GUIDE", "GAS-301 guide loaded")
        });
}

static void DeleteIfExists(string path)
{
    if (File.Exists(path))
    {
        File.Delete(path);
    }
}

static void AssertContains(string actual, string expected, string message)
{
    if (!actual.Contains(expected, StringComparison.Ordinal))
    {
        throw new InvalidOperationException(message);
    }
}

static void AssertDoesNotContain(string actual, string unexpected, string message)
{
    if (actual.Contains(unexpected, StringComparison.Ordinal))
    {
        throw new InvalidOperationException(message);
    }
}

static void AssertEqual<T>(T expected, T actual, string message)
{
    if (!EqualityComparer<T>.Default.Equals(expected, actual))
    {
        throw new InvalidOperationException($"{message} Expected: {expected}; actual: {actual}.");
    }
}
