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
        var json = File.ReadAllText(options.ScenarioPath);
        var scenario = EquipmentScenario.FromJson(json);
        var timeoutPolicy = options.UseDefaultTimeouts ? StateTimeoutPolicy.CreateDefaultMvpPolicy() : null;
        var runner = ScenarioRunner.CreateDefault(options.InitialUtc, timeoutPolicy);
        var result = runner.Run(scenario);

        PrintResult(result, runner);
        return result.Success ? 0 : 1;
    }
    catch (Exception ex)
    {
        Console.Error.WriteLine($"ERROR: {ex.Message}");
        Console.Error.WriteLine();
        PrintUsage();
        return 2;
    }
}

static void PrintUsage()
{
    Console.WriteLine(
        """
        EquipmentTwin.Cli

        Usage:
          dotnet run --project src/EquipmentTwin.Cli -- <scenario.json> [--default-timeouts] [--initial-utc <iso-utc>]
          dotnet run --project src/EquipmentTwin.Cli -- run <scenario.json> [--default-timeouts] [--initial-utc <iso-utc>]

        Examples:
          dotnet run --project src/EquipmentTwin.Cli -- scenarios/normal-cycle.json
          dotnet run --project src/EquipmentTwin.Cli -- scenarios/loading-timeout.json --default-timeouts

        Options:
          --default-timeouts       Use the default MVP timeout policy.
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

internal sealed record CliOptions(
    string ScenarioPath,
    bool UseDefaultTimeouts,
    DateTimeOffset InitialUtc)
{
    public static CliOptions Parse(string[] args)
    {
        var position = 0;
        if (args[position].Equals("run", StringComparison.OrdinalIgnoreCase))
        {
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

        while (position < args.Length)
        {
            var arg = args[position];

            switch (arg)
            {
                case "--default-timeouts":
                    useDefaultTimeouts = true;
                    position++;
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

        if (!File.Exists(scenarioPath))
        {
            throw new FileNotFoundException($"Scenario file was not found: {scenarioPath}");
        }

        return new CliOptions(scenarioPath, useDefaultTimeouts, initialUtc);
    }
}
