namespace EquipmentTwin.Core.Processes;

public sealed record MolyAldTimelineStep(
    int Index,
    string Step,
    int? Cycle,
    DateTimeOffset StartedAtUtc,
    DateTimeOffset CompletedAtUtc,
    double DurationMilliseconds,
    bool Success,
    string Message,
    double ChamberPressureMtorr,
    double WaferTemperatureC,
    MolyAldTimelineValves Valves,
    double EstimatedThicknessAngstrom)
{
    public static MolyAldTimelineStep FromStepLog(MolyAldStepLog step)
    {
        ArgumentNullException.ThrowIfNull(step);

        return new MolyAldTimelineStep(
            step.Index,
            step.Step.ToString(),
            step.Cycle,
            step.StartedAtUtc,
            step.CompletedAtUtc,
            step.Duration.TotalMilliseconds,
            step.Success,
            step.Message,
            step.ChamberPressureMtorr,
            step.WaferTemperatureC,
            new MolyAldTimelineValves(
                step.MetalPrecursorValveOpen,
                step.ReactantValveOpen,
                step.PurgeValveOpen),
            step.EstimatedThicknessAngstrom);
    }
}
