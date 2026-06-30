using EquipmentTwin.Core.Io;
using EquipmentTwin.Core.Motion;

namespace EquipmentTwin.Core.Scenarios;

/// <summary>
/// JSON 시나리오를 실행해서 가상 IO, 상태머신, Clock/Timeout 흐름을 검증한다.
/// </summary>
public sealed class ScenarioRunner
{
    private readonly StateTimeoutPolicy? _timeoutPolicy;
    private readonly Dictionary<string, MotionAxis> _motionAxes = new(StringComparer.OrdinalIgnoreCase);

    public ScenarioRunner(
        EquipmentCellController cell,
        ManualClock clock,
        StateTimeoutPolicy? timeoutPolicy = null)
    {
        Cell = cell ?? throw new ArgumentNullException(nameof(cell));
        Clock = clock ?? throw new ArgumentNullException(nameof(clock));
        _timeoutPolicy = timeoutPolicy;
    }

    public EquipmentCellController Cell { get; }

    public ManualClock Clock { get; }

    public IReadOnlyDictionary<string, MotionAxis> MotionAxes => _motionAxes;

    public ScenarioRunResult Run(EquipmentScenario scenario)
    {
        ArgumentNullException.ThrowIfNull(scenario);
        scenario.Validate();

        var results = new List<ScenarioStepRunResult>();

        for (var index = 0; index < scenario.Steps.Count; index++)
        {
            var step = scenario.Steps[index];
            var result = RunStep(index, step);
            results.Add(result);

            if (!result.Success)
            {
                break;
            }
        }

        return new ScenarioRunResult(
            scenario.Name,
            results.All(result => result.Success),
            Cell.StateMachine.CurrentState,
            results);
    }

    public static ScenarioRunner CreateDefault(
        DateTimeOffset? initialUtc = null,
        StateTimeoutPolicy? timeoutPolicy = null)
    {
        var clock = new ManualClock(initialUtc ?? DateTimeOffset.UtcNow);
        var io = EquipmentIoMap.CreateDefaultCellIo();
        var stateMachine = new EquipmentStateMachine(clock);
        var cell = new EquipmentCellController(stateMachine, io);

        return new ScenarioRunner(cell, clock, timeoutPolicy);
    }

    private ScenarioStepRunResult RunStep(int index, ScenarioStep step)
    {
        var messages = new List<string>();

        try
        {
            switch (step.Action)
            {
                case ScenarioStepAction.StartCycle:
                    messages.Add(Cell.StartCycle().Message);
                    break;

                case ScenarioStepAction.SetInput:
                    Cell.Io.SetInput(step.Signal!, step.Value!.Value, source: "Scenario");
                    messages.Add($"Input '{step.Signal}' set to '{step.Value}'.");
                    break;

                case ScenarioStepAction.AdvanceTime:
                    Clock.Advance(TimeSpan.FromMilliseconds(step.AdvanceMilliseconds!.Value));
                    messages.Add($"Clock advanced by {step.AdvanceMilliseconds} ms.");
                    break;

                case ScenarioStepAction.PollInputs:
                    messages.Add(Cell.PollInputs(_timeoutPolicy).Message);
                    break;

                case ScenarioStepAction.ClearAlarm:
                    messages.Add(Cell.ClearAlarm().Message);
                    break;

                case ScenarioStepAction.MotionServoOn:
                    messages.Add(GetMotionAxis(step.Axis!).ServoOn().Message);
                    break;

                case ScenarioStepAction.StartMotionHome:
                    messages.Add(GetMotionAxis(step.Axis!).StartHome(
                        TimeSpan.FromMilliseconds(step.DurationMilliseconds!.Value)).Message);
                    break;

                case ScenarioStepAction.StartMotionMove:
                    messages.Add(GetMotionAxis(step.Axis!).StartMove(
                        step.TargetPosition!.Value,
                        TimeSpan.FromMilliseconds(step.DurationMilliseconds!.Value)).Message);
                    break;

                case ScenarioStepAction.PollMotion:
                    messages.Add(GetMotionAxis(step.Axis!).Poll().Message);
                    break;

                case ScenarioStepAction.CheckMotionTimeout:
                    messages.Add(GetMotionAxis(step.Axis!).CheckTimeout(
                        TimeSpan.FromMilliseconds(step.TimeoutMilliseconds!.Value)).Message);
                    break;

                case ScenarioStepAction.ExpectState:
                case ScenarioStepAction.ExpectSignal:
                case ScenarioStepAction.ExpectMotionState:
                    messages.Add("Expectation step evaluated.");
                    break;

                default:
                    throw new InvalidOperationException($"Unsupported scenario action '{step.Action}'.");
            }

            var validationErrors = ValidateExpectations(step);

            return new ScenarioStepRunResult(
                index + 1,
                step.Name,
                step.Action,
                validationErrors.Count == 0,
                Cell.StateMachine.CurrentState,
                string.Join(" ", messages),
                validationErrors);
        }
        catch (Exception ex)
        {
            return new ScenarioStepRunResult(
                index + 1,
                step.Name,
                step.Action,
                false,
                Cell.StateMachine.CurrentState,
                ex.Message,
                new[] { ex.Message });
        }
    }

    private IReadOnlyList<string> ValidateExpectations(ScenarioStep step)
    {
        var errors = new List<string>();

        if (step.ExpectState is not null && Cell.StateMachine.CurrentState != step.ExpectState)
        {
            errors.Add($"Expected state '{step.ExpectState}', actual '{Cell.StateMachine.CurrentState}'.");
        }

        if (step.Action == ScenarioStepAction.ExpectSignal && step.Signal is not null && step.Value is not null)
        {
            AddSignalExpectation(errors, step.Signal, step.Value.Value);
        }

        if (step.Axis is not null)
        {
            AddMotionExpectations(errors, step);
        }

        foreach (var expectedSignal in step.ExpectSignals)
        {
            AddSignalExpectation(errors, expectedSignal.Key, expectedSignal.Value);
        }

        return errors;
    }

    private void AddSignalExpectation(List<string> errors, string signal, bool expectedValue)
    {
        var actualValue = Cell.Io.Read(signal);

        if (actualValue != expectedValue)
        {
            errors.Add($"Expected signal '{signal}' to be '{expectedValue}', actual '{actualValue}'.");
        }
    }

    private MotionAxis GetMotionAxis(string axisName)
    {
        if (!_motionAxes.TryGetValue(axisName, out var axis))
        {
            axis = new MotionAxis(axisName, Clock);
            _motionAxes.Add(axisName, axis);
        }

        return axis;
    }

    private void AddMotionExpectations(List<string> errors, ScenarioStep step)
    {
        var axis = GetMotionAxis(step.Axis!);

        if (step.ExpectMotionState is not null && axis.State != step.ExpectMotionState)
        {
            errors.Add($"Expected motion axis '{axis.Name}' state '{step.ExpectMotionState}', actual '{axis.State}'.");
        }

        if (step.ExpectMotionAlarmCode is not null && axis.LastAlarm?.Code != step.ExpectMotionAlarmCode)
        {
            errors.Add($"Expected motion axis '{axis.Name}' alarm '{step.ExpectMotionAlarmCode}', actual '{axis.LastAlarm?.Code}'.");
        }

        if (step.ExpectPosition is not null && Math.Abs(axis.Position - step.ExpectPosition.Value) > 0.0001)
        {
            errors.Add($"Expected motion axis '{axis.Name}' position '{step.ExpectPosition}', actual '{axis.Position}'.");
        }
    }
}
