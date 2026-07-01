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
                break;

            case InspectionMode.DatasetCamera:
            case InspectionMode.UnityCamera:
                if (InspectionResult is null)
                {
                    throw new InvalidOperationException($"Product recipe '{Name}' requires inspectionResult when inspectionMode is {InspectionMode}.");
                }

                InspectionResult.Validate(Name);
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
}
