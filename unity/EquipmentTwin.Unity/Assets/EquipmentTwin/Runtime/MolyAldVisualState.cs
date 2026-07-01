namespace EquipmentTwin.Unity.Processes
{
    public sealed class MolyAldVisualState
    {
        public MolyAldVisualState(
            string recipeName,
            string stepName,
            int stepIndex,
            int totalSteps,
            int cycle,
            bool hasCycle,
            bool hasFault,
            bool metalPrecursorOpen,
            bool reactantOpen,
            bool purgeOpen,
            float pressureRatio,
            float vacuumRatio,
            float temperatureRatio,
            float thicknessRatio,
            float chamberPressureMtorr,
            float waferTemperatureC,
            float estimatedThicknessAngstrom,
            string stepLabel,
            string valueLabel)
        {
            RecipeName = recipeName;
            StepName = stepName;
            StepIndex = stepIndex;
            TotalSteps = totalSteps;
            Cycle = cycle;
            HasCycle = hasCycle;
            HasFault = hasFault;
            MetalPrecursorOpen = metalPrecursorOpen;
            ReactantOpen = reactantOpen;
            PurgeOpen = purgeOpen;
            PressureRatio = pressureRatio;
            VacuumRatio = vacuumRatio;
            TemperatureRatio = temperatureRatio;
            ThicknessRatio = thicknessRatio;
            ChamberPressureMtorr = chamberPressureMtorr;
            WaferTemperatureC = waferTemperatureC;
            EstimatedThicknessAngstrom = estimatedThicknessAngstrom;
            StepLabel = stepLabel;
            ValueLabel = valueLabel;
        }

        public string RecipeName { get; }

        public string StepName { get; }

        public int StepIndex { get; }

        public int TotalSteps { get; }

        public int Cycle { get; }

        public bool HasCycle { get; }

        public bool HasFault { get; }

        public bool MetalPrecursorOpen { get; }

        public bool ReactantOpen { get; }

        public bool PurgeOpen { get; }

        public float PressureRatio { get; }

        public float VacuumRatio { get; }

        public float TemperatureRatio { get; }

        public float ThicknessRatio { get; }

        public float ChamberPressureMtorr { get; }

        public float WaferTemperatureC { get; }

        public float EstimatedThicknessAngstrom { get; }

        public string StepLabel { get; }

        public string ValueLabel { get; }
    }
}
