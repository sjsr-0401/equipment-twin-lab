using UnityEngine;

namespace EquipmentTwin.Unity.Processes
{
    public static class MolyAldVisualStateMapper
    {
        public static MolyAldVisualState FromTimeline(
            MolyAldTimelineDocumentDto timeline,
            MolyAldTimelineStepDto step,
            float processPressureMtorr,
            float atmospherePressureMtorr,
            float roomTemperatureC,
            float processTemperatureC)
        {
            if (step == null)
            {
                return null;
            }

            var pressureRatio = PressureRatio(step.chamberPressureMtorr, processPressureMtorr, atmospherePressureMtorr);
            var vacuumRatio = 1f - pressureRatio;
            var temperatureRatio = Mathf.InverseLerp(roomTemperatureC, processTemperatureC, step.waferTemperatureC);
            var thicknessRatio = ThicknessRatio(timeline, step);
            var totalSteps = timeline != null && timeline.steps != null ? timeline.steps.Length : 0;
            var hasFault = timeline != null && (!timeline.success || !step.success);
            var valves = step.valves;
            var metalPrecursorOpen = valves != null && valves.metalPrecursor;
            var reactantOpen = valves != null && valves.reactant;
            var purgeOpen = valves != null && valves.purge;
            var cycleText = step.HasCycle ? step.cycle.ToString() : "-";
            var stepLabel = $"{step.index}/{totalSteps} {step.step}";
            var valueLabel =
                $"cycle {cycleText} | vacuum {vacuumRatio:P0} | film {thicknessRatio:P0}\n" +
                $"pressure {step.chamberPressureMtorr:0.#} mTorr | temp {step.waferTemperatureC:0.#} C\n" +
                $"valves {ValveText(metalPrecursorOpen, reactantOpen, purgeOpen)}";

            return new MolyAldVisualState(
                timeline != null ? timeline.recipeName : string.Empty,
                step.step,
                step.index,
                totalSteps,
                step.cycle,
                step.HasCycle,
                hasFault,
                metalPrecursorOpen,
                reactantOpen,
                purgeOpen,
                pressureRatio,
                vacuumRatio,
                temperatureRatio,
                thicknessRatio,
                step.chamberPressureMtorr,
                step.waferTemperatureC,
                step.estimatedThicknessAngstrom,
                stepLabel,
                valueLabel);
        }

        private static float PressureRatio(float pressureMtorr, float processPressureMtorr, float atmospherePressureMtorr)
        {
            var low = Mathf.Min(processPressureMtorr, atmospherePressureMtorr);
            var high = Mathf.Max(processPressureMtorr, atmospherePressureMtorr);
            return Mathf.InverseLerp(low, high, Mathf.Clamp(pressureMtorr, low, high));
        }

        private static float ThicknessRatio(MolyAldTimelineDocumentDto timeline, MolyAldTimelineStepDto step)
        {
            if (timeline == null || timeline.targetThicknessAngstrom <= 0f)
            {
                return 0f;
            }

            return Mathf.Clamp01(step.estimatedThicknessAngstrom / timeline.targetThicknessAngstrom);
        }

        private static string ValveText(bool metalPrecursorOpen, bool reactantOpen, bool purgeOpen)
        {
            return
                $"P:{OnOff(metalPrecursorOpen)} " +
                $"R:{OnOff(reactantOpen)} " +
                $"G:{OnOff(purgeOpen)}";
        }

        private static string OnOff(bool value)
        {
            return value ? "ON" : "OFF";
        }
    }
}
