using EquipmentTwin.Core.Motion;

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

    public string? Axis { get; init; }

    public double? TargetPosition { get; init; }

    public int? DurationMilliseconds { get; init; }

    public int? TimeoutMilliseconds { get; init; }

    public EquipmentState? ExpectState { get; init; }

    public Dictionary<string, bool> ExpectSignals { get; init; } = new();

    public MotionAxisState? ExpectMotionState { get; init; }

    public MotionAxisAlarmCode? ExpectMotionAlarmCode { get; init; }

    public double? ExpectPosition { get; init; }

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

            case ScenarioStepAction.MotionServoOn:
            case ScenarioStepAction.PollMotion:
                RequireAxis(index);
                break;

            case ScenarioStepAction.StartMotionHome:
                RequireAxis(index);
                RequireDuration();
                break;

            case ScenarioStepAction.StartMotionMove:
                RequireAxis(index);
                RequireDuration();
                if (TargetPosition is null)
                {
                    throw new InvalidOperationException($"Scenario step '{Name}' requires targetPosition.");
                }
                break;

            case ScenarioStepAction.CheckMotionTimeout:
                RequireAxis(index);
                if (TimeoutMilliseconds is null or <= 0)
                {
                    throw new InvalidOperationException($"Scenario step '{Name}' requires positive timeoutMilliseconds.");
                }
                break;

            case ScenarioStepAction.ExpectMotionState:
                RequireAxis(index);
                if (ExpectMotionState is null)
                {
                    throw new InvalidOperationException($"Scenario step '{Name}' requires expectMotionState.");
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

    private void RequireAxis(int index)
    {
        if (string.IsNullOrWhiteSpace(Axis))
        {
            throw new InvalidOperationException($"Scenario step #{index + 1} requires an axis name.");
        }
    }

    private void RequireDuration()
    {
        if (DurationMilliseconds is null or <= 0)
        {
            throw new InvalidOperationException($"Scenario step '{Name}' requires positive durationMilliseconds.");
        }
    }
}
