namespace EquipmentTwin.Core.Templates;

/// <summary>
/// TemplateRunner가 가상 모션을 실행할 때 사용하는 표준 시간 옵션이다.
/// </summary>
public sealed class TemplateRunnerOptions
{
    public TimeSpan HomeDuration { get; init; } = TimeSpan.FromSeconds(1);

    public TimeSpan MoveDuration { get; init; } = TimeSpan.FromSeconds(2);

    public void Validate()
    {
        if (HomeDuration <= TimeSpan.Zero)
        {
            throw new InvalidOperationException("Template runner home duration must be greater than zero.");
        }

        if (MoveDuration <= TimeSpan.Zero)
        {
            throw new InvalidOperationException("Template runner move duration must be greater than zero.");
        }
    }
}
