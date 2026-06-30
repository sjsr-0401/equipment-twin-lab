namespace EquipmentTwin.Core.Templates;

/// <summary>
/// 장비 템플릿 안에 포함되는 모션 축 정의다.
/// </summary>
public sealed class MotionAxisTemplate
{
    public string Name { get; init; } = string.Empty;

    public double InitialPosition { get; init; }

    public bool RequiresHome { get; init; } = true;

    public void Validate(int index)
    {
        if (string.IsNullOrWhiteSpace(Name))
        {
            throw new InvalidOperationException($"Motion axis template #{index + 1} requires a name.");
        }
    }
}
