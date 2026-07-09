namespace EquipmentTwin.Core.Alarms;

/// <summary>
/// 작업자가 확인 결과를 고른 뒤 다음 조치를 안내하기 위한 선택지다.
/// 실제 회사 작업지시서가 아니라 공개 데모용 일반화 모델이다.
/// </summary>
public sealed class AlarmGuideChoice
{
    public string Id { get; init; } = string.Empty;

    public string Label { get; init; } = string.Empty;

    public string NextAction { get; init; } = string.Empty;

    public bool RequiresEngineer { get; init; }

    public void Validate(string alarmCode, int index)
    {
        if (string.IsNullOrWhiteSpace(Id))
        {
            throw new InvalidOperationException($"Alarm guide '{alarmCode}' choice #{index + 1} requires an id.");
        }

        if (string.IsNullOrWhiteSpace(Label))
        {
            throw new InvalidOperationException($"Alarm guide '{alarmCode}' choice '{Id}' requires a label.");
        }

        if (string.IsNullOrWhiteSpace(NextAction))
        {
            throw new InvalidOperationException($"Alarm guide '{alarmCode}' choice '{Id}' requires a nextAction.");
        }
    }
}
