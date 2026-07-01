namespace EquipmentTwin.Core.Templates;

/// <summary>
/// 같은 recipe에서 선택할 수 있는 데이터 기반 검사 케이스다.
/// 실제 카메라가 없는 동안 PASS/FAIL 이미지 데이터셋을 고르는 역할을 한다.
/// </summary>
public sealed class InspectionScenario
{
    public string Name { get; init; } = string.Empty;

    public InspectionResultSpec? Result { get; init; }

    public void Validate(string recipeName, int index)
    {
        if (string.IsNullOrWhiteSpace(Name))
        {
            throw new InvalidOperationException($"Inspection scenario #{index + 1} for recipe '{recipeName}' requires a name.");
        }

        if (Result is null)
        {
            throw new InvalidOperationException($"Inspection scenario '{Name}' for recipe '{recipeName}' requires a result.");
        }

        Result.Validate(recipeName);
    }
}
