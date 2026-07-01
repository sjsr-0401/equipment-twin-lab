namespace EquipmentTwin.Core.Processes;

public sealed class MolyAldFaultScenario
{
    public string Name { get; init; } = string.Empty;

    public MolyAldFaultKind Kind { get; init; }

    public int? TriggerCycle { get; init; }

    public string Message { get; init; } = string.Empty;

    public void Validate(string recipeName, int index, int cycleCount)
    {
        if (string.IsNullOrWhiteSpace(Name))
        {
            throw new InvalidOperationException($"Moly ALD recipe '{recipeName}' fault scenario #{index + 1} requires a name.");
        }

        if (string.IsNullOrWhiteSpace(Message))
        {
            throw new InvalidOperationException($"Moly ALD fault scenario '{Name}' requires a message.");
        }

        switch (Kind)
        {
            case MolyAldFaultKind.PumpDownTimeout:
            case MolyAldFaultKind.TemperatureNotStable:
                if (TriggerCycle is not null)
                {
                    throw new InvalidOperationException($"Moly ALD fault scenario '{Name}' must not specify triggerCycle for setup faults.");
                }
                break;

            case MolyAldFaultKind.PrecursorDoseTimeout:
            case MolyAldFaultKind.PurgeTimeout:
                if (TriggerCycle is not null && (TriggerCycle < 1 || TriggerCycle > cycleCount))
                {
                    throw new InvalidOperationException($"Moly ALD fault scenario '{Name}' triggerCycle must be between 1 and {cycleCount}.");
                }
                break;

            default:
                throw new InvalidOperationException($"Moly ALD fault scenario '{Name}' has unsupported kind '{Kind}'.");
        }
    }
}
