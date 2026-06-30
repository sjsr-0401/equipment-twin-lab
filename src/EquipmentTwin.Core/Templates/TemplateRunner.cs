using EquipmentTwin.Core.Motion;

namespace EquipmentTwin.Core.Templates;

/// <summary>
/// EquipmentTemplate와 ProductRecipe를 실제 가상 모션 실행으로 바꿔주는 최소 실행기다.
/// </summary>
public sealed class TemplateRunner
{
    private readonly ManualClock _clock;
    private readonly TemplateRunnerOptions _options;

    public TemplateRunner(
        ManualClock clock,
        TemplateRunnerOptions? options = null)
    {
        _clock = clock ?? throw new ArgumentNullException(nameof(clock));
        _options = options ?? new TemplateRunnerOptions();
        _options.Validate();
    }

    public TemplateRunResult RunRecipe(
        EquipmentTemplate template,
        string recipeName,
        string? faultScenarioName = null)
    {
        ArgumentNullException.ThrowIfNull(template);

        template.Validate();
        var recipe = template.FindProductRecipe(recipeName);
        var faultScenario = string.IsNullOrWhiteSpace(faultScenarioName)
            ? null
            : template.FindFaultScenario(faultScenarioName);
        var axes = template.CreateMotionAxes(_clock);
        var commandLog = new List<TemplateMotionCommandLog>();

        foreach (var axisTemplate in template.MotionAxes)
        {
            var axis = axes[axisTemplate.Name];
            Add(commandLog, "ServoOn", axis, axis.ServoOn());

            if (axisTemplate.RequiresHome)
            {
                Add(commandLog, "StartHome", axis, axis.StartHome(_options.HomeDuration));
                _clock.Advance(_options.HomeDuration);
                Add(commandLog, "PollHome", axis, axis.Poll());
            }
        }

        foreach (var axisTarget in recipe.AxisTargets.OrderBy(target => target.Key, StringComparer.OrdinalIgnoreCase))
        {
            var axis = axes[axisTarget.Key];
            Add(commandLog, "StartMove", axis, axis.StartMove(axisTarget.Value, _options.MoveDuration));

            if (ShouldInjectFault(faultScenario, axis.Name))
            {
                InjectFault(commandLog, axis, faultScenario!);
                break;
            }

            _clock.Advance(_options.MoveDuration);
            Add(commandLog, "PollMove", axis, axis.Poll());
        }

        var inspectionResult = CreateInspectionResult(recipe, axes, commandLog);

        return new TemplateRunResult(template.Name, recipe, faultScenario, inspectionResult, axes, commandLog);
    }

    private static InspectionResult? CreateInspectionResult(
        ProductRecipe recipe,
        IReadOnlyDictionary<string, MotionAxis> axes,
        IReadOnlyList<TemplateMotionCommandLog> commandLog)
    {
        if (recipe.InspectionMode == InspectionMode.None)
        {
            return null;
        }

        var motionSucceeded = commandLog.All(log => log.Result.Accepted)
            && axes.Values.All(axis => axis.LastAlarm is null);

        return motionSucceeded ? InspectionResult.FromRecipe(recipe) : null;
    }

    private static bool ShouldInjectFault(FaultScenario? faultScenario, string axisName)
    {
        return faultScenario is not null
            && string.Equals(faultScenario.Axis, axisName, StringComparison.OrdinalIgnoreCase);
    }

    private void InjectFault(
        List<TemplateMotionCommandLog> commandLog,
        MotionAxis axis,
        FaultScenario faultScenario)
    {
        switch (faultScenario.Kind)
        {
            case FaultKind.MotionTimeout:
                _clock.Advance(TimeSpan.FromMilliseconds(faultScenario.ElapsedMilliseconds!.Value));
                Add(
                    commandLog,
                    "FaultMotionTimeout",
                    axis,
                    axis.CheckTimeout(TimeSpan.FromMilliseconds(faultScenario.TimeoutMilliseconds!.Value)));
                break;

            case FaultKind.ServoAlarm:
                Add(
                    commandLog,
                    "FaultServoAlarm",
                    axis,
                    axis.TriggerServoAlarm(faultScenario.Message));
                break;

            default:
                throw new InvalidOperationException($"Unsupported fault kind '{faultScenario.Kind}'.");
        }
    }

    private static void Add(
        List<TemplateMotionCommandLog> commandLog,
        string step,
        MotionAxis axis,
        MotionCommandResult result)
    {
        commandLog.Add(new TemplateMotionCommandLog(step, axis.Name, result));
    }
}
