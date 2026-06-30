namespace EquipmentTwin.Core.Templates;

/// <summary>
/// 시뮬레이션에서 recipe가 만들어낼 검사 결과 정의다.
/// 실제 카메라가 붙기 전까지는 이 데이터가 가상 검사 결과 역할을 한다.
/// </summary>
public sealed class InspectionResultSpec
{
    public InspectionOutcome Outcome { get; init; } = InspectionOutcome.Pass;

    public string DefectCode { get; init; } = string.Empty;

    public string Message { get; init; } = string.Empty;

    public Dictionary<string, double> Measurements { get; init; } = new(StringComparer.OrdinalIgnoreCase);

    public void Validate(string recipeName)
    {
        switch (Outcome)
        {
            case InspectionOutcome.Pass:
                if (!string.IsNullOrWhiteSpace(DefectCode))
                {
                    throw new InvalidOperationException($"Inspection result for recipe '{recipeName}' must not contain defectCode when outcome is Pass.");
                }
                break;

            case InspectionOutcome.Fail:
                if (string.IsNullOrWhiteSpace(DefectCode))
                {
                    throw new InvalidOperationException($"Inspection result for recipe '{recipeName}' requires defectCode when outcome is Fail.");
                }
                break;

            default:
                throw new InvalidOperationException($"Inspection result for recipe '{recipeName}' has unsupported outcome '{Outcome}'.");
        }

        foreach (var measurement in Measurements)
        {
            if (string.IsNullOrWhiteSpace(measurement.Key))
            {
                throw new InvalidOperationException($"Inspection result for recipe '{recipeName}' contains a blank measurement name.");
            }

            if (double.IsNaN(measurement.Value) || double.IsInfinity(measurement.Value))
            {
                throw new InvalidOperationException($"Inspection result for recipe '{recipeName}' contains invalid measurement '{measurement.Key}'.");
            }
        }
    }
}
