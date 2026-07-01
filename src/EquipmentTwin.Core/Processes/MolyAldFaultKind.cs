namespace EquipmentTwin.Core.Processes;

/// <summary>
/// Faults are intentionally generic so the public demo does not encode real equipment alarms.
/// </summary>
public enum MolyAldFaultKind
{
    PumpDownTimeout,
    TemperatureNotStable,
    PrecursorDoseTimeout,
    PurgeTimeout
}
