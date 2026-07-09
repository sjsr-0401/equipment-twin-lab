namespace EquipmentTwin.Core.Alarms;

/// <summary>
/// 알람 발생 시 작업자가 확인할 항목과 다음 조치를 담는 작업지시서 모델이다.
/// 실제 현장 문서를 복제하지 않고, 포트폴리오용으로 일반화한 데이터 구조다.
/// </summary>
public sealed class AlarmResponseGuide
{
    public string AlarmCode { get; init; } = string.Empty;

    public string Title { get; init; } = string.Empty;

    public AlarmGuideSeverity Severity { get; init; }

    public string Summary { get; init; } = string.Empty;

    public List<OperatorCheckItem> Checks { get; init; } = new();

    public List<AlarmGuideChoice> Choices { get; init; } = new();

    public List<string> EscalationConditions { get; init; } = new();

    public List<string> SnapshotFields { get; init; } = new();

    public void Validate(int index)
    {
        if (string.IsNullOrWhiteSpace(AlarmCode))
        {
            throw new InvalidOperationException($"Alarm response guide #{index + 1} requires an alarmCode.");
        }

        if (string.IsNullOrWhiteSpace(Title))
        {
            throw new InvalidOperationException($"Alarm response guide '{AlarmCode}' requires a title.");
        }

        if (Severity == AlarmGuideSeverity.Unknown)
        {
            throw new InvalidOperationException($"Alarm response guide '{AlarmCode}' requires a known severity.");
        }

        if (string.IsNullOrWhiteSpace(Summary))
        {
            throw new InvalidOperationException($"Alarm response guide '{AlarmCode}' requires a summary.");
        }

        if (Checks is null || Checks.Count == 0)
        {
            throw new InvalidOperationException($"Alarm response guide '{AlarmCode}' requires at least one operator check.");
        }

        var checkIds = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        for (var checkIndex = 0; checkIndex < Checks.Count; checkIndex++)
        {
            var check = Checks[checkIndex] ?? throw new InvalidOperationException($"Alarm response guide '{AlarmCode}' check #{checkIndex + 1} is empty.");
            check.Validate(AlarmCode, checkIndex);

            if (!checkIds.Add(check.Id))
            {
                throw new InvalidOperationException($"Alarm response guide '{AlarmCode}' contains duplicate check id '{check.Id}'.");
            }
        }

        var choiceIds = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        for (var choiceIndex = 0; choiceIndex < Choices.Count; choiceIndex++)
        {
            var choice = Choices[choiceIndex] ?? throw new InvalidOperationException($"Alarm response guide '{AlarmCode}' choice #{choiceIndex + 1} is empty.");
            choice.Validate(AlarmCode, choiceIndex);

            if (!choiceIds.Add(choice.Id))
            {
                throw new InvalidOperationException($"Alarm response guide '{AlarmCode}' contains duplicate choice id '{choice.Id}'.");
            }
        }

        ValidateTextList(EscalationConditions, AlarmCode, "escalationConditions");
        ValidateTextList(SnapshotFields, AlarmCode, "snapshotFields");
    }

    private static void ValidateTextList(IReadOnlyList<string>? values, string alarmCode, string propertyName)
    {
        if (values is null)
        {
            return;
        }

        for (var index = 0; index < values.Count; index++)
        {
            if (string.IsNullOrWhiteSpace(values[index]))
            {
                throw new InvalidOperationException($"Alarm response guide '{alarmCode}' contains a blank {propertyName} entry.");
            }
        }
    }
}
