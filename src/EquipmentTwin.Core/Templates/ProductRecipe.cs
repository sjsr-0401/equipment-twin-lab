namespace EquipmentTwin.Core.Templates;

/// <summary>
/// 같은 장비에서 제품별로 달라지는 목표 위치와 검사 방식을 정의한다.
/// </summary>
public sealed class ProductRecipe
{
    public string Name { get; init; } = string.Empty;

    public string ProductCode { get; init; } = string.Empty;

    public InspectionMode InspectionMode { get; init; } = InspectionMode.None;

    public InspectionResultSpec? InspectionResult { get; init; }

    public List<InspectionScenario> InspectionScenarios { get; init; } = new();

    public Dictionary<string, double> AxisTargets { get; init; } = new(StringComparer.OrdinalIgnoreCase);

    public void Validate(int index, IReadOnlySet<string> knownAxisNames)
    {
        if (string.IsNullOrWhiteSpace(Name))
        {
            throw new InvalidOperationException($"Product recipe #{index + 1} requires a name.");
        }

        if (string.IsNullOrWhiteSpace(ProductCode))
        {
            throw new InvalidOperationException($"Product recipe '{Name}' requires a productCode.");
        }

        switch (InspectionMode)
        {
            case InspectionMode.None:
                if (InspectionResult is not null)
                {
                    throw new InvalidOperationException($"Product recipe '{Name}' must not contain inspectionResult when inspectionMode is None.");
                }

                if (InspectionScenarios.Count > 0)
                {
                    throw new InvalidOperationException($"Product recipe '{Name}' must not contain inspectionScenarios when inspectionMode is None.");
                }
                break;

            case InspectionMode.DatasetCamera:
            case InspectionMode.UnityCamera:
                if (InspectionResult is null && InspectionScenarios.Count == 0)
                {
                    throw new InvalidOperationException($"Product recipe '{Name}' requires inspectionResult or inspectionScenarios when inspectionMode is {InspectionMode}.");
                }

                InspectionResult?.Validate(Name);
                ValidateInspectionScenarios();
                break;

            default:
                throw new InvalidOperationException($"Product recipe '{Name}' has unsupported inspectionMode '{InspectionMode}'.");
        }

        if (AxisTargets.Count == 0)
        {
            throw new InvalidOperationException($"Product recipe '{Name}' requires at least one axis target.");
        }

        foreach (var axisName in AxisTargets.Keys)
        {
            if (string.IsNullOrWhiteSpace(axisName))
            {
                throw new InvalidOperationException($"Product recipe '{Name}' contains a blank axis target name.");
            }

            if (!knownAxisNames.Contains(axisName))
            {
                throw new InvalidOperationException($"Product recipe '{Name}' references unknown axis '{axisName}'.");
            }
        }
    }

    public bool TryGetAxisTarget(string axisName, out double targetPosition)
    {
        foreach (var target in AxisTargets)
        {
            if (string.Equals(target.Key, axisName, StringComparison.OrdinalIgnoreCase))
            {
                targetPosition = target.Value;
                return true;
            }
        }

        targetPosition = default;
        return false;
    }

    public InspectionResultSpec ResolveInspectionResult(string? inspectionScenarioName)
    {
        if (InspectionMode == InspectionMode.None)
        {
            throw new InvalidOperationException($"Product recipe '{Name}' does not require inspection.");
        }

        if (string.IsNullOrWhiteSpace(inspectionScenarioName))
        {
            if (InspectionResult is not null)
            {
                return InspectionResult;
            }

            if (InspectionScenarios.Count == 1)
            {
                return InspectionScenarios[0].Result
                    ?? throw new InvalidOperationException($"Inspection scenario '{InspectionScenarios[0].Name}' for recipe '{Name}' requires a result.");
            }

            throw new InvalidOperationException($"Product recipe '{Name}' requires an inspection scenario name.");
        }

        return FindInspectionScenario(inspectionScenarioName).Result
            ?? throw new InvalidOperationException($"Inspection scenario '{inspectionScenarioName}' for recipe '{Name}' requires a result.");
    }

    public InspectionScenario FindInspectionScenario(string inspectionScenarioName)
    {
        if (string.IsNullOrWhiteSpace(inspectionScenarioName))
        {
            throw new ArgumentException("Inspection scenario name is required.", nameof(inspectionScenarioName));
        }

        return InspectionScenarios.FirstOrDefault(scenario =>
            string.Equals(scenario.Name, inspectionScenarioName, StringComparison.OrdinalIgnoreCase))
            ?? throw new InvalidOperationException($"Inspection scenario '{inspectionScenarioName}' was not found in recipe '{Name}'.");
    }

    private void ValidateInspectionScenarios()
    {
        var scenarioNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        for (var index = 0; index < InspectionScenarios.Count; index++)
        {
            var scenario = InspectionScenarios[index];
            scenario.Validate(Name, index);

            if (!scenarioNames.Add(scenario.Name))
            {
                throw new InvalidOperationException($"Product recipe '{Name}' contains duplicate inspection scenario '{scenario.Name}'.");
            }
        }
    }
}
