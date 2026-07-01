namespace EquipmentTwin.Core.Processes;

/// <summary>
/// Deterministic public/synthetic ALD process runner.
/// The output is shaped so a Unity scene can replay valve states, chamber state, and film growth.
/// </summary>
public sealed class MolyAldRunner
{
    private const double AtmosphericPressureMtorr = 760000;
    private const double AmbientTemperatureC = 25;

    private readonly ManualClock _clock;

    public MolyAldRunner(ManualClock clock)
    {
        _clock = clock ?? throw new ArgumentNullException(nameof(clock));
    }

    public MolyAldRunResult Run(MolyAldRecipe recipe, string? faultScenarioName = null)
    {
        ArgumentNullException.ThrowIfNull(recipe);

        recipe.Validate();
        var faultScenario = string.IsNullOrWhiteSpace(faultScenarioName)
            ? null
            : recipe.FindFaultScenario(faultScenarioName);

        var steps = new List<MolyAldStepLog>();
        var thickness = 0.0;
        var index = 0;

        AddStep(
            steps,
            ++index,
            MolyAldProcessStep.LoadWafer,
            cycle: null,
            TimeSpan.FromSeconds(2),
            success: true,
            "Wafer loaded into public synthetic ALD module.",
            AtmosphericPressureMtorr,
            AmbientTemperatureC,
            metalPrecursorValveOpen: false,
            reactantValveOpen: false,
            purgeValveOpen: false,
            thickness);

        if (ShouldTrigger(faultScenario, MolyAldFaultKind.PumpDownTimeout))
        {
            AddStep(
                steps,
                ++index,
                MolyAldProcessStep.PumpDown,
                cycle: null,
                TimeSpan.FromSeconds(recipe.PumpDownSeconds),
                success: false,
                faultScenario!.Message,
                chamberPressureMtorr: 25000,
                recipe.WaferTemperatureC,
                metalPrecursorValveOpen: false,
                reactantValveOpen: false,
                purgeValveOpen: false,
                thickness);

            return Fail(recipe, faultScenario, thickness, steps, "Pump-down fault stopped the ALD run.");
        }

        AddStep(
            steps,
            ++index,
            MolyAldProcessStep.PumpDown,
            cycle: null,
            TimeSpan.FromSeconds(recipe.PumpDownSeconds),
            success: true,
            "Chamber reached the synthetic process pressure setpoint.",
            recipe.ChamberPressureMtorr,
            recipe.WaferTemperatureC,
            metalPrecursorValveOpen: false,
            reactantValveOpen: false,
            purgeValveOpen: true,
            thickness);

        if (ShouldTrigger(faultScenario, MolyAldFaultKind.TemperatureNotStable))
        {
            AddStep(
                steps,
                ++index,
                MolyAldProcessStep.StabilizeTemperature,
                cycle: null,
                TimeSpan.FromSeconds(recipe.TemperatureStabilizeSeconds),
                success: false,
                faultScenario!.Message,
                recipe.ChamberPressureMtorr,
                recipe.WaferTemperatureC - 15,
                metalPrecursorValveOpen: false,
                reactantValveOpen: false,
                purgeValveOpen: true,
                thickness);

            return Fail(recipe, faultScenario, thickness, steps, "Temperature stabilization fault stopped the ALD run.");
        }

        AddStep(
            steps,
            ++index,
            MolyAldProcessStep.StabilizeTemperature,
            cycle: null,
            TimeSpan.FromSeconds(recipe.TemperatureStabilizeSeconds),
            success: true,
            "Wafer temperature stabilized at the synthetic setpoint.",
            recipe.ChamberPressureMtorr,
            recipe.WaferTemperatureC,
            metalPrecursorValveOpen: false,
            reactantValveOpen: false,
            purgeValveOpen: true,
            thickness);

        var growthPerCycle = recipe.TargetThicknessAngstrom / recipe.CycleCount;

        for (var cycle = 1; cycle <= recipe.CycleCount; cycle++)
        {
            if (ShouldTrigger(faultScenario, MolyAldFaultKind.PrecursorDoseTimeout, cycle))
            {
                AddStep(
                    steps,
                    ++index,
                    MolyAldProcessStep.DoseMetalPrecursor,
                    cycle,
                    TimeSpan.FromMilliseconds(recipe.MetalPrecursorDoseMilliseconds),
                    success: false,
                    faultScenario!.Message,
                    recipe.ChamberPressureMtorr,
                    recipe.WaferTemperatureC,
                    metalPrecursorValveOpen: true,
                    reactantValveOpen: false,
                    purgeValveOpen: false,
                    thickness);

                return Fail(recipe, faultScenario, thickness, steps, $"Precursor dose fault stopped cycle {cycle}.");
            }

            AddStep(
                steps,
                ++index,
                MolyAldProcessStep.DoseMetalPrecursor,
                cycle,
                TimeSpan.FromMilliseconds(recipe.MetalPrecursorDoseMilliseconds),
                success: true,
                $"Cycle {cycle}: synthetic molybdenum precursor dose completed.",
                recipe.ChamberPressureMtorr,
                recipe.WaferTemperatureC,
                metalPrecursorValveOpen: true,
                reactantValveOpen: false,
                purgeValveOpen: false,
                thickness);

            if (ShouldTrigger(faultScenario, MolyAldFaultKind.PurgeTimeout, cycle))
            {
                AddStep(
                    steps,
                    ++index,
                    MolyAldProcessStep.PurgeAfterPrecursor,
                    cycle,
                    TimeSpan.FromMilliseconds(recipe.PurgeMilliseconds),
                    success: false,
                    faultScenario!.Message,
                    recipe.ChamberPressureMtorr,
                    recipe.WaferTemperatureC,
                    metalPrecursorValveOpen: false,
                    reactantValveOpen: false,
                    purgeValveOpen: true,
                    thickness);

                return Fail(recipe, faultScenario, thickness, steps, $"Purge fault stopped cycle {cycle}.");
            }

            AddStep(
                steps,
                ++index,
                MolyAldProcessStep.PurgeAfterPrecursor,
                cycle,
                TimeSpan.FromMilliseconds(recipe.PurgeMilliseconds),
                success: true,
                $"Cycle {cycle}: precursor purge completed.",
                recipe.ChamberPressureMtorr,
                recipe.WaferTemperatureC,
                metalPrecursorValveOpen: false,
                reactantValveOpen: false,
                purgeValveOpen: true,
                thickness);

            thickness += growthPerCycle;

            AddStep(
                steps,
                ++index,
                MolyAldProcessStep.DoseReactant,
                cycle,
                TimeSpan.FromMilliseconds(recipe.ReactantDoseMilliseconds),
                success: true,
                $"Cycle {cycle}: synthetic reactant dose completed and film increment was applied.",
                recipe.ChamberPressureMtorr,
                recipe.WaferTemperatureC,
                metalPrecursorValveOpen: false,
                reactantValveOpen: true,
                purgeValveOpen: false,
                thickness);

            AddStep(
                steps,
                ++index,
                MolyAldProcessStep.PurgeAfterReactant,
                cycle,
                TimeSpan.FromMilliseconds(recipe.PurgeMilliseconds),
                success: true,
                $"Cycle {cycle}: reactant purge completed.",
                recipe.ChamberPressureMtorr,
                recipe.WaferTemperatureC,
                metalPrecursorValveOpen: false,
                reactantValveOpen: false,
                purgeValveOpen: true,
                thickness);
        }

        AddStep(
            steps,
            ++index,
            MolyAldProcessStep.PostPurge,
            cycle: null,
            TimeSpan.FromMilliseconds(recipe.PurgeMilliseconds),
            success: true,
            "Post-process purge completed.",
            recipe.ChamberPressureMtorr,
            recipe.WaferTemperatureC,
            metalPrecursorValveOpen: false,
            reactantValveOpen: false,
            purgeValveOpen: true,
            thickness);

        AddStep(
            steps,
            ++index,
            MolyAldProcessStep.TransferOut,
            cycle: null,
            TimeSpan.FromSeconds(2),
            success: true,
            "Wafer transferred out of the synthetic process module.",
            AtmosphericPressureMtorr,
            AmbientTemperatureC,
            metalPrecursorValveOpen: false,
            reactantValveOpen: false,
            purgeValveOpen: false,
            thickness);

        AddStep(
            steps,
            ++index,
            MolyAldProcessStep.Complete,
            cycle: null,
            TimeSpan.Zero,
            success: true,
            "Public synthetic molybdenum ALD run completed.",
            AtmosphericPressureMtorr,
            AmbientTemperatureC,
            metalPrecursorValveOpen: false,
            reactantValveOpen: false,
            purgeValveOpen: false,
            thickness);

        return new MolyAldRunResult(
            recipe.Name,
            recipe,
            Success: true,
            MolyAldProcessStep.Complete,
            faultScenario,
            thickness,
            steps,
            "ALD run completed.");
    }

    private static MolyAldRunResult Fail(
        MolyAldRecipe recipe,
        MolyAldFaultScenario faultScenario,
        double thickness,
        IReadOnlyList<MolyAldStepLog> steps,
        string message)
    {
        return new MolyAldRunResult(
            recipe.Name,
            recipe,
            Success: false,
            MolyAldProcessStep.Alarmed,
            faultScenario,
            thickness,
            steps,
            message);
    }

    private void AddStep(
        List<MolyAldStepLog> steps,
        int index,
        MolyAldProcessStep step,
        int? cycle,
        TimeSpan duration,
        bool success,
        string message,
        double chamberPressureMtorr,
        double waferTemperatureC,
        bool metalPrecursorValveOpen,
        bool reactantValveOpen,
        bool purgeValveOpen,
        double estimatedThicknessAngstrom)
    {
        var startedAtUtc = _clock.UtcNow;
        _clock.Advance(duration);
        var completedAtUtc = _clock.UtcNow;

        steps.Add(new MolyAldStepLog(
            index,
            step,
            cycle,
            startedAtUtc,
            completedAtUtc,
            success,
            message,
            chamberPressureMtorr,
            waferTemperatureC,
            metalPrecursorValveOpen,
            reactantValveOpen,
            purgeValveOpen,
            estimatedThicknessAngstrom));
    }

    private static bool ShouldTrigger(
        MolyAldFaultScenario? faultScenario,
        MolyAldFaultKind kind,
        int? cycle = null)
    {
        if (faultScenario is null || faultScenario.Kind != kind)
        {
            return false;
        }

        if (faultScenario.TriggerCycle is null)
        {
            return true;
        }

        return cycle == faultScenario.TriggerCycle.Value;
    }
}
