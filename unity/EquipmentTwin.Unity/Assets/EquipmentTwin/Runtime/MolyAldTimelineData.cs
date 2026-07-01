using System;

namespace EquipmentTwin.Unity.Processes
{
    [Serializable]
    public sealed class MolyAldTimelineDocumentDto
    {
        public string schemaVersion = string.Empty;
        public string source = string.Empty;
        public string recipeName = string.Empty;
        public bool success;
        public string finalStep = string.Empty;
        public string faultScenarioName = string.Empty;
        public int stationCount;
        public int cycleCount;
        public float targetThicknessAngstrom;
        public float estimatedThicknessAngstrom;
        public float totalDurationMilliseconds;
        public string[] publicBasis = new string[0];
        public MolyAldTimelineStepDto[] steps = new MolyAldTimelineStepDto[0];
    }

    [Serializable]
    public sealed class MolyAldTimelineStepDto
    {
        public int index;
        public string step = string.Empty;

        // Unity JsonUtility stores JSON null numbers as default 0.
        // In this schema, cycle 0 means "not inside an ALD cycle".
        public int cycle;

        public string startedAtUtc = string.Empty;
        public string completedAtUtc = string.Empty;
        public float durationMilliseconds;
        public bool success;
        public string message = string.Empty;
        public float chamberPressureMtorr;
        public float waferTemperatureC;
        public MolyAldTimelineValvesDto valves = new MolyAldTimelineValvesDto();
        public float estimatedThicknessAngstrom;

        public bool HasCycle => cycle > 0;
    }

    [Serializable]
    public sealed class MolyAldTimelineValvesDto
    {
        public bool metalPrecursor;
        public bool reactant;
        public bool purge;
    }
}
