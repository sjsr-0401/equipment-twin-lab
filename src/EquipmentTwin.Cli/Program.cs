using System.Text;
using EquipmentTwin.Core;
using EquipmentTwin.Core.Scenarios;

return Run(args);

static int Run(string[] args)
{
    if (args.Length == 0 || args.Contains("--help") || args.Contains("-h"))
    {
        PrintUsage();
        return 0;
    }

    try
    {
        var options = CliOptions.Parse(args);

        return options.Mode switch
        {
            CliMode.Run => RunSingleScenario(options),
            CliMode.Batch => RunBatch(options),
            _ => throw new InvalidOperationException($"Unsupported CLI mode '{options.Mode}'.")
        };
    }
    catch (Exception ex)
    {
        Console.Error.WriteLine($"ERROR: {ex.Message}");
        Console.Error.WriteLine();
        PrintUsage();
        return 2;
    }
}

static int RunSingleScenario(CliOptions options)
{
    var run = ExecuteScenario(options.ScenarioPath, options);

    if (run.Result is not null)
    {
        PrintResult(run.Result, run.Runner!);
        return run.Result.Success ? 0 : 1;
    }

    Console.Error.WriteLine($"Scenario failed before execution: {run.ErrorMessage}");
    return 1;
}

static int RunBatch(CliOptions options)
{
    var scenarioPaths = ResolveScenarioPaths(options.ScenarioPath);
    var runs = scenarioPaths
        .Select(path => ExecuteScenario(path, options))
        .ToArray();

    PrintBatchResult(runs);

    if (!string.IsNullOrWhiteSpace(options.ReportPath))
    {
        WriteMarkdownReport(options.ReportPath, runs, options);
        Console.WriteLine();
        Console.WriteLine($"Report: {options.ReportPath}");
    }

    return runs.All(run => run.Success) ? 0 : 1;
}

static ScenarioCliRun ExecuteScenario(string scenarioPath, CliOptions options)
{
    try
    {
        var json = File.ReadAllText(scenarioPath);
        var scenario = EquipmentScenario.FromJson(json);
        var timeoutPolicy = options.UseDefaultTimeouts ? StateTimeoutPolicy.CreateDefaultMvpPolicy() : null;
        var runner = ScenarioRunner.CreateDefault(options.InitialUtc, timeoutPolicy);
        var result = runner.Run(scenario);

        return new ScenarioCliRun(scenarioPath, runner, result, null);
    }
    catch (Exception ex)
    {
        return new ScenarioCliRun(scenarioPath, null, null, ex.Message);
    }
}

static IReadOnlyList<string> ResolveScenarioPaths(string sourcePath)
{
    if (File.Exists(sourcePath))
    {
        return new[] { sourcePath };
    }

    if (!Directory.Exists(sourcePath))
    {
        throw new FileNotFoundException($"Scenario path was not found: {sourcePath}");
    }

    var paths = Directory
        .GetFiles(sourcePath, "*.json", SearchOption.TopDirectoryOnly)
        .OrderBy(path => path, StringComparer.OrdinalIgnoreCase)
        .ToArray();

    if (paths.Length == 0)
    {
        throw new InvalidOperationException($"Scenario directory contains no .json files: {sourcePath}");
    }

    return paths;
}

static void PrintUsage()
{
    Console.WriteLine(
        """
        EquipmentTwin.Cli

        Usage:
          dotnet run --project src/EquipmentTwin.Cli -- <scenario.json> [--default-timeouts] [--initial-utc <iso-utc>]
          dotnet run --project src/EquipmentTwin.Cli -- run <scenario.json> [--default-timeouts] [--initial-utc <iso-utc>]
          dotnet run --project src/EquipmentTwin.Cli -- batch <scenario-directory-or-json> [--default-timeouts] [--report <report.md>] [--initial-utc <iso-utc>]

        Examples:
          dotnet run --project src/EquipmentTwin.Cli -- scenarios/normal-cycle.json
          dotnet run --project src/EquipmentTwin.Cli -- scenarios/loading-timeout.json --default-timeouts
          dotnet run --project src/EquipmentTwin.Cli -- batch scenarios --default-timeouts --report artifacts/scenario-report.md

        Options:
          --default-timeouts       Use the default MVP timeout policy.
          --report <path>          Write a Markdown batch report.
          --initial-utc <value>    Set initial UTC time. Example: 2026-06-25T00:00:00Z
          -h, --help               Show this help.
        """);
}

static void PrintResult(ScenarioRunResult result, ScenarioRunner runner)
{
    Console.WriteLine($"Scenario: {result.ScenarioName}");
    Console.WriteLine($"Result:   {(result.Success ? "PASS" : "FAIL")}");
    Console.WriteLine($"Final:    {result.FinalState}");
    Console.WriteLine();

    foreach (var step in result.Steps)
    {
        var status = step.Success ? "PASS" : "FAIL";
        Console.WriteLine($"{step.Index:00}. {status} {step.Name} [{step.Action}] -> {step.StateAfterStep}");

        if (!string.IsNullOrWhiteSpace(step.Message))
        {
            Console.WriteLine($"    {step.Message}");
        }

        foreach (var error in step.ValidationErrors)
        {
            Console.WriteLine($"    ERROR: {error}");
        }
    }

    Console.WriteLine();
    Console.WriteLine("Signals:");
    foreach (var signal in runner.Cell.Io.Snapshot())
    {
        Console.WriteLine($"  {signal.Name,-30} {signal.Direction,-6} {signal.Value}");
    }
}

static void PrintBatchResult(IReadOnlyList<ScenarioCliRun> runs)
{
    var passed = runs.Count(run => run.Success);
    var failed = runs.Count - passed;

    Console.WriteLine("Scenario batch");
    Console.WriteLine($"Result: {(failed == 0 ? "PASS" : "FAIL")}");
    Console.WriteLine($"Passed: {passed}");
    Console.WriteLine($"Failed: {failed}");
    Console.WriteLine();

    foreach (var run in runs)
    {
        var status = run.Success ? "PASS" : "FAIL";
        var scenarioName = run.Result?.ScenarioName ?? Path.GetFileNameWithoutExtension(run.ScenarioPath);
        var finalState = run.Result?.FinalState.ToString() ?? "NotRun";

        Console.WriteLine($"{status} {scenarioName} -> {finalState}");

        if (!run.Success)
        {
            foreach (var error in run.Errors)
            {
                Console.WriteLine($"  ERROR: {error}");
            }
        }
    }
}

static void WriteMarkdownReport(string reportPath, IReadOnlyList<ScenarioCliRun> runs, CliOptions options)
{
    var directory = Path.GetDirectoryName(reportPath);
    if (!string.IsNullOrWhiteSpace(directory))
    {
        Directory.CreateDirectory(directory);
    }

    File.WriteAllText(reportPath, BuildMarkdownReport(runs, options), Encoding.UTF8);
}

static string BuildMarkdownReport(IReadOnlyList<ScenarioCliRun> runs, CliOptions options)
{
    var passed = runs.Count(run => run.Success);
    var failed = runs.Count - passed;
    var builder = new StringBuilder();

    builder.AppendLine("# Equipment Twin Scenario Batch Report");
    builder.AppendLine();
    builder.AppendLine($"- Generated UTC: `{DateTimeOffset.UtcNow:O}`");
    builder.AppendLine($"- Initial scenario UTC: `{options.InitialUtc:O}`");
    builder.AppendLine($"- Default timeouts: `{options.UseDefaultTimeouts}`");
    builder.AppendLine($"- Total: `{runs.Count}`");
    builder.AppendLine($"- Passed: `{passed}`");
    builder.AppendLine($"- Failed: `{failed}`");
    builder.AppendLine();

    builder.AppendLine("## Summary");
    builder.AppendLine();
    builder.AppendLine("| Scenario | Result | Final State | Active Alarm | Clear Condition | Motion Axes | File |");
    builder.AppendLine("|---|---:|---|---|---|---|---|");
    foreach (var run in runs)
    {
        var scenarioName = EscapeMarkdownTable(run.Result?.ScenarioName ?? Path.GetFileNameWithoutExtension(run.ScenarioPath));
        var status = run.Success ? "PASS" : "FAIL";
        var finalState = run.Result?.FinalState.ToString() ?? "NotRun";
        var activeAlarm = EscapeMarkdownTable(DescribeActiveAlarm(run));
        var clearCondition = EscapeMarkdownTable(DescribeClearCondition(run));
        var motionAxes = EscapeMarkdownTable(DescribeMotionAxes(run));
        var fileName = EscapeMarkdownTable(run.ScenarioPath);
        builder.AppendLine($"| {scenarioName} | {status} | {finalState} | {activeAlarm} | {clearCondition} | {motionAxes} | `{fileName}` |");
    }

    builder.AppendLine();
    builder.AppendLine("## Details");
    builder.AppendLine();

    foreach (var run in runs)
    {
        var scenarioName = run.Result?.ScenarioName ?? Path.GetFileNameWithoutExtension(run.ScenarioPath);
        builder.AppendLine($"### {scenarioName}");
        builder.AppendLine();
        builder.AppendLine($"- File: `{run.ScenarioPath}`");
        builder.AppendLine($"- Result: `{(run.Success ? "PASS" : "FAIL")}`");
        builder.AppendLine($"- Final state: `{run.Result?.FinalState.ToString() ?? "NotRun"}`");
        builder.AppendLine($"- Active alarm: `{DescribeActiveAlarm(run)}`");
        builder.AppendLine($"- Clear condition: `{DescribeClearCondition(run)}`");
        builder.AppendLine($"- Motion axes: `{DescribeMotionAxes(run)}`");

        if (run.Errors.Count > 0)
        {
            builder.AppendLine("- Errors:");
            foreach (var error in run.Errors)
            {
                builder.AppendLine($"  - {error}");
            }
        }

        if (run.Result is not null)
        {
            builder.AppendLine();
            builder.AppendLine("| Step | Result | Action | State | Message |");
            builder.AppendLine("|---:|---:|---|---|---|");
            foreach (var step in run.Result.Steps)
            {
                var status = step.Success ? "PASS" : "FAIL";
                var message = EscapeMarkdownTable(step.Message);
                builder.AppendLine($"| {step.Index} | {status} | {step.Action} | {step.StateAfterStep} | {message} |");
            }
        }

        builder.AppendLine();
    }

    return builder.ToString();
}

static string DescribeActiveAlarm(ScenarioCliRun run)
{
    var alarm = run.Runner?.Cell.StateMachine.LastAlarm;
    if (alarm is null)
    {
        return "None";
    }

    return $"{alarm.Code} ({(int)alarm.Code}): {alarm.Message}";
}

static string DescribeClearCondition(ScenarioCliRun run)
{
    if (run.Runner is null)
    {
        return "NotRun";
    }

    if (run.Runner.Cell.StateMachine.CurrentState != EquipmentState.Alarmed)
    {
        return "Not alarmed";
    }

    var recovery = run.Runner.Cell.CheckAlarmRecoveryCondition();
    var status = recovery.CanClear ? "Clearable" : "Blocked";

    return $"{status}: {recovery.Message}";
}

static string DescribeMotionAxes(ScenarioCliRun run)
{
    if (run.Runner is null || run.Runner.MotionAxes.Count == 0)
    {
        return "None";
    }

    return string.Join(
        ", ",
        run.Runner.MotionAxes.Values
            .OrderBy(axis => axis.Name, StringComparer.OrdinalIgnoreCase)
            .Select(axis =>
            {
                var alarm = axis.LastAlarm is null
                    ? "NoAlarm"
                    : $"{axis.LastAlarm.Code}: {axis.LastAlarm.Message}";

                return $"{axis.Name}: {axis.State} @ {axis.Position} ({alarm})";
            }));
}

static string EscapeMarkdownTable(string value)
{
    return value.Replace("|", "\\|", StringComparison.Ordinal);
}

internal enum CliMode
{
    Run,
    Batch
}

internal sealed record CliOptions(
    CliMode Mode,
    string ScenarioPath,
    bool UseDefaultTimeouts,
    DateTimeOffset InitialUtc,
    string? ReportPath)
{
    public static CliOptions Parse(string[] args)
    {
        var position = 0;
        var mode = CliMode.Run;

        if (args[position].Equals("run", StringComparison.OrdinalIgnoreCase))
        {
            position++;
        }
        else if (args[position].Equals("batch", StringComparison.OrdinalIgnoreCase))
        {
            mode = CliMode.Batch;
            position++;
        }

        if (position >= args.Length)
        {
            throw new ArgumentException("Scenario path is required.");
        }

        var scenarioPath = args[position];
        position++;

        var useDefaultTimeouts = false;
        var initialUtc = new DateTimeOffset(2026, 6, 25, 0, 0, 0, TimeSpan.Zero);
        string? reportPath = null;

        while (position < args.Length)
        {
            var arg = args[position];

            switch (arg)
            {
                case "--default-timeouts":
                    useDefaultTimeouts = true;
                    position++;
                    break;

                case "--report":
                    if (position + 1 >= args.Length)
                    {
                        throw new ArgumentException("--report requires a path.");
                    }

                    reportPath = args[position + 1];
                    position += 2;
                    break;

                case "--initial-utc":
                    if (position + 1 >= args.Length)
                    {
                        throw new ArgumentException("--initial-utc requires a value.");
                    }

                    if (!DateTimeOffset.TryParse(args[position + 1], out initialUtc))
                    {
                        throw new ArgumentException($"Invalid --initial-utc value '{args[position + 1]}'.");
                    }

                    position += 2;
                    break;

                default:
                    throw new ArgumentException($"Unknown argument '{arg}'.");
            }
        }

        if (mode == CliMode.Run && !File.Exists(scenarioPath))
        {
            throw new FileNotFoundException($"Scenario file was not found: {scenarioPath}");
        }

        return new CliOptions(mode, scenarioPath, useDefaultTimeouts, initialUtc, reportPath);
    }
}

internal sealed record ScenarioCliRun(
    string ScenarioPath,
    ScenarioRunner? Runner,
    ScenarioRunResult? Result,
    string? ErrorMessage)
{
    public bool Success => Result?.Success == true && string.IsNullOrWhiteSpace(ErrorMessage);

    public IReadOnlyList<string> Errors
    {
        get
        {
            if (!string.IsNullOrWhiteSpace(ErrorMessage))
            {
                return new[] { ErrorMessage };
            }

            if (Result is null || Result.Success)
            {
                return Array.Empty<string>();
            }

            var errors = Result.FailedSteps
                .SelectMany(step => step.ValidationErrors)
                .Where(error => !string.IsNullOrWhiteSpace(error))
                .ToArray();

            return errors.Length > 0
                ? errors
                : new[] { "Scenario failed without validation details." };
        }
    }
}
