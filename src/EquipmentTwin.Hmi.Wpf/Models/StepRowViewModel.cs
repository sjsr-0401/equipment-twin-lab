using EquipmentTwin.Core.Processes;

namespace EquipmentTwin.Hmi.Wpf.Models;

public sealed class StepRowViewModel
{
    public StepRowViewModel(MolyAldTimelineStep step)
    {
        Index = step.Index;
        Step = DisplayStepName(step.Step);
        Cycle = step.Cycle?.ToString() ?? "-";
        DurationMs = step.DurationMilliseconds.ToString("0");
        Pressure = $"{step.ChamberPressureMtorr:0} mTorr";
        Temperature = $"{step.WaferTemperatureC:0} C";
        Film = $"{step.EstimatedThicknessAngstrom:0.0} A";
        Valve = ActiveValveText(step);
        Result = step.Success ? "PASS" : "FAIL";
        Message = step.Message;
    }

    public int Index { get; }

    public string Step { get; }

    public string Cycle { get; }

    public string DurationMs { get; }

    public string Pressure { get; }

    public string Temperature { get; }

    public string Film { get; }

    public string Valve { get; }

    public string Result { get; }

    public string Message { get; }

    public static string DisplayStepName(string stepName)
    {
        if (string.Equals(stepName, "DoseMetalPrecursor", StringComparison.OrdinalIgnoreCase))
        {
            return "Dose Precursor";
        }

        if (string.Equals(stepName, "PurgeAfterPrecursor", StringComparison.OrdinalIgnoreCase))
        {
            return "Purge Precursor";
        }

        if (string.Equals(stepName, "PurgeAfterReactant", StringComparison.OrdinalIgnoreCase))
        {
            return "Purge Reactant";
        }

        if (string.Equals(stepName, "StabilizeTemperature", StringComparison.OrdinalIgnoreCase))
        {
            return "Stabilize Temp";
        }

        return SplitCamelCase(stepName);
    }

    private static string ActiveValveText(MolyAldTimelineStep step)
    {
        if (step.Valves.MetalPrecursor)
        {
            return "Precursor";
        }

        if (step.Valves.Reactant)
        {
            return "Reactant";
        }

        if (step.Valves.Purge)
        {
            return "Purge";
        }

        return "None";
    }

    private static string SplitCamelCase(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return "-";
        }

        return System.Text.RegularExpressions.Regex.Replace(value, "([a-z])([A-Z])", "$1 $2");
    }
}
