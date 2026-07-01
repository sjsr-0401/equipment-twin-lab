namespace EquipmentTwin.Core.Processes;

/// <summary>
/// Public/synthetic molybdenum ALD process phases.
/// This is not a vendor recipe. It models the software-control shape of a deposition cycle.
/// </summary>
public enum MolyAldProcessStep
{
    Idle,
    LoadWafer,
    PumpDown,
    StabilizeTemperature,
    DoseMetalPrecursor,
    PurgeAfterPrecursor,
    DoseReactant,
    PurgeAfterReactant,
    PostPurge,
    TransferOut,
    Complete,
    Alarmed
}
