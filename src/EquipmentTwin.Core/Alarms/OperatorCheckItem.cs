namespace EquipmentTwin.Core.Alarms;

/// <summary>
/// 오퍼레이터가 알람 대응 중 눈으로 확인하거나 상태를 확인해야 하는 항목이다.
/// </summary>
public sealed class OperatorCheckItem
{
    public string Id { get; init; } = string.Empty;

    public string Label { get; init; } = string.Empty;

    public bool Required { get; init; }

    public void Validate(string alarmCode, int index)
    {
        if (string.IsNullOrWhiteSpace(Id))
        {
            throw new InvalidOperationException($"Alarm guide '{alarmCode}' check #{index + 1} requires an id.");
        }

        if (string.IsNullOrWhiteSpace(Label))
        {
            throw new InvalidOperationException($"Alarm guide '{alarmCode}' check '{Id}' requires a label.");
        }
    }
}
