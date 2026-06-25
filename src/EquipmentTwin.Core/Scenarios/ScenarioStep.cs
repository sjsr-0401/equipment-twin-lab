namespace EquipmentTwin.Core.Scenarios;

/// <summary>
/// 장비 시나리오의 한 단계다.
/// Action에 따라 Signal, Value, AdvanceMilliseconds 같은 필드가 필요할 수 있다.
/// </summary>
public sealed class ScenarioStep
{
    public string Name { get; init; } = string.Empty;

    public ScenarioStepAction Action { get; init; }

    public string? Signal { get; init; }

    public bool? Value { get; init; }

    public int? AdvanceMilliseconds { get; init; }

    public EquipmentState? ExpectState { get; init; }

    public Dictionary<string, bool> ExpectSignals { get; init; } = new();

    public void Validate(int index)
    {
        if (string.IsNullOrWhiteSpace(Name))
        {
            throw new InvalidOperationException($"Scenario step #{index + 1} requires a name.");
        }

        switch (Action)
        {
            case ScenarioStepAction.SetInput:
                RequireSignal(index);
                if (Value is null)
                {
                    throw new InvalidOperationException($"Scenario step '{Name}' requires a boolean value.");
                }
                break;

            case ScenarioStepAction.AdvanceTime:
                if (AdvanceMilliseconds is null or <= 0)
                {
                    throw new InvalidOperationException($"Scenario step '{Name}' requires positive advanceMilliseconds.");
                }
                break;

            case ScenarioStepAction.ExpectState:
                if (ExpectState is null)
                {
                    throw new InvalidOperationException($"Scenario step '{Name}' requires expectState.");
                }
                break;

            case ScenarioStepAction.ExpectSignal:
                RequireSignal(index);
                if (Value is null)
                {
                    throw new InvalidOperationException($"Scenario step '{Name}' requires a boolean value.");
                }
                break;
        }
    }

    private void RequireSignal(int index)
    {
        if (string.IsNullOrWhiteSpace(Signal))
        {
            throw new InvalidOperationException($"Scenario step #{index + 1} requires a signal name.");
        }
    }
}
