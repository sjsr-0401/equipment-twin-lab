namespace EquipmentTwin.Core.Processes;

public sealed record MolyAldStepLog(
    int Index,
    MolyAldProcessStep Step,
    int? Cycle,
    DateTimeOffset StartedAtUtc,
    DateTimeOffset CompletedAtUtc,
    bool Success,
    string Message,
    double ChamberPressureMtorr,
    double WaferTemperatureC,
    bool MetalPrecursorValveOpen,
    bool ReactantValveOpen,
    bool PurgeValveOpen,
    double EstimatedThicknessAngstrom)
{
    public TimeSpan Duration => CompletedAtUtc - StartedAtUtc;
}
